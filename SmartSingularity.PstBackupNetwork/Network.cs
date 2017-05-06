using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Logger = SmartSingularity.PstBackupLogger.Logger;

namespace SmartSingularity.PstBackupNetwork
{
    public static class Network
    {
        /// <summary>
        /// Determine whether or not, the destination is on a WAN link. Based on the local IPv4 address, the remote IPv4 address and the subnet mask.
        /// </summary>
        /// <param name="destination">Hostname or IpAddress to Ping.</param>
        /// <returns>Returns false if the IPv4 address of the destination is in the same subnet of the local IPv4 address (based on the subnet mask), returns true other wise. Returns true if the destination doesn't reply to ping query.</returns>
        public static bool IsWanLink(string destination, string additionalSubnets)
        {
            try
            {
                System.Net.NetworkInformation.Ping pingRequest = new System.Net.NetworkInformation.Ping();
                
                string hostName = GetHostName(destination);
                System.Net.NetworkInformation.PingReply pingReply = pingRequest.Send(hostName, 3000);
                if (pingReply.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    System.Net.IPAddress remoteAddress = pingReply.Address;
                    if (remoteAddress.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                        return true;
                    System.Net.IPAddress localAddress = GetLocalAddress(remoteAddress);
                    if (localAddress.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                        return true;
                    System.Net.IPAddress subnetMask = GetSubnetMask(localAddress);
                    if (subnetMask.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                        return true;

                    if (IsInSameSubnet(localAddress, remoteAddress, subnetMask))
                        return false;

                    foreach (string additionalSubnet in additionalSubnets.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (IsInSameSubnet(localAddress, additionalSubnet))
                            return false;
                    }
                }
            }
            catch (Exception) { }

            return true;
        }

        /// <summary>
        /// Get the hostname of a remote host from the fullpath.
        /// </summary>
        /// <param name="destination">The fullpath to a directory on the remote host.</param>
        /// <returns>Return the hostname of the remote host or 'LocalHost' if the destination start by x:\</returns>
        public static string GetHostName(string destination)
        {
            string result = "LocalHost";

            try
            {
                System.Text.RegularExpressions.Regex regExpLocalHost = new System.Text.RegularExpressions.Regex(@"^[A-Za-z]{1}:\\", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (!regExpLocalHost.IsMatch(destination))
                {
                    System.Text.RegularExpressions.Regex regExpIpAddress = new System.Text.RegularExpressions.Regex(@"^\\\\[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+\\", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (regExpIpAddress.IsMatch(destination))
                    {
                        string remoteHost = regExpIpAddress.Match(destination).Captures[0].ToString();
                        return remoteHost.Substring(2, remoteHost.LastIndexOf('\\') - 2);
                    }
                    System.Text.RegularExpressions.Regex regExpRemoteHost = new System.Text.RegularExpressions.Regex(@"^\\\\[A-Za-z0-9\.]+\\", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (regExpRemoteHost.IsMatch(destination))
                    {
                        string remoteHost = regExpRemoteHost.Match(destination).Captures[0].ToString();
                        return remoteHost.Substring(2, remoteHost.LastIndexOf('\\') - 2);
                    }
                }
            }
            catch (Exception) { }

            return result;
        }

        /// <summary>
        /// Get the local IPv4 address used to connect to the specified remote host.
        /// </summary>
        /// <param name="remoteAddress">IPv4 address of the remote host.</param>
        /// <returns>The IPv4 address used to connect to the specified remote host. Return null if no suitable local IPv4 address have been found.</returns>
        /// <exception cref="ArgumentException">Argument Exception if remoteAddress is not an IPv4 Address.</exception>
        public static IPAddress GetLocalAddress(IPAddress remoteAddress)
        {
            IPAddress localAddress = null;

            if (remoteAddress == null || remoteAddress.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                throw new ArgumentException("'remoteAddress' must be an IPv4 address.");

            try
            {
                System.Net.Sockets.UdpClient u = new System.Net.Sockets.UdpClient(remoteAddress.ToString(), 1);
                IPEndPoint ipEndpoint = (IPEndPoint)u.Client.LocalEndPoint;
                localAddress = ((IPEndPoint)u.Client.LocalEndPoint).Address;
                if (localAddress.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                    return null;
            }
            catch (Exception) { }

            return localAddress;
        }

        /// <summary>
        /// Get the subnet mask associated with an IPv4 address.
        /// </summary>
        /// <param name="localAddress">IPv4 address for which you will find the subnet mask.</param>
        /// <returns>The subnet mask associated with the IPv4 address.</returns>
        /// <exception cref="ArgumentException">Argument Exception if localAddress is not an IPv4 address.</exception>
        public static IPAddress GetSubnetMask(IPAddress localAddress)
        {
            if (localAddress == null || localAddress.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                throw new ArgumentException("'localAddress' must be an IPv4 address.");

            try
            {
                foreach (System.Net.NetworkInformation.NetworkInterface nic in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                {
                    System.Net.NetworkInformation.IPInterfaceProperties ipProps = nic.GetIPProperties();
                    foreach (System.Net.NetworkInformation.UnicastIPAddressInformation unicastAddress in ipProps.UnicastAddresses)
                    {
                        if (unicastAddress.Address.Equals(localAddress))
                            return unicastAddress.IPv4Mask;
                    }
                }
            }
            catch (Exception) { }
            return null;
        }

        /// <summary>
        /// Determine whether or not localAddress is in the same subnet than remoteAddress, based on the subnet mask.
        /// </summary>
        /// <param name="localAddress">The local IPv4 address.</param>
        /// <param name="remoteAddress">The remote IPv4 address.</param>
        /// <param name="subnetMask">The IPv4 subnet mask.</param>
        /// <returns>True if both localAddress and remoteAddress are in the same subnet based on the subnet mask, false other wise.</returns>
        /// <exception cref="ArgumentException">All IPAddress must be IPv4 addresses.</exception>
        /// <exception cref="NullException">When failed to get the subnet address for one of the IPv4 address.</exception>
        public static bool IsInSameSubnet(IPAddress localAddress, IPAddress remoteAddress, IPAddress subnetMask)
        {
            IPAddress localSubnet = GetSubnetAddress(localAddress, subnetMask);
            IPAddress remoteSubnet = GetSubnetAddress(remoteAddress, subnetMask);

            return localSubnet.Equals(remoteSubnet);
        }

        /// <summary>
        /// Determine Wheter or not the localAddress is in the same subnet.
        /// </summary>
        /// <param name="localAddress">IPAddress to look for.</param>
        /// <param name="subnet">Subnet into which we want to look for. Must be CIDR formatted</param>
        /// <returns>true if the localAddress is in tha sae subnet, false otherwise.</returns>
        public static bool IsInSameSubnet(IPAddress localAddress, string CIDRsubnet)
        {
            IPAddress subnet = GetIPAddressFromCIDRSubnet(CIDRsubnet);
            IPAddress mask = GetSubnetMaskFromCIDRSubnet(CIDRsubnet);

            return IsInSameSubnet(localAddress, subnet, mask);
        }

        public static IPAddress GetIPAddressFromCIDRSubnet(string CIDRSubnet)
        {
            if (CIDRSubnet.Contains("/"))
            {
                string ipAdd = CIDRSubnet.Substring(0, CIDRSubnet.IndexOf('/'));
                string[] strBytes = ipAdd.Split(new char[] { '.' });
                if (strBytes.Length == 4)
                {
                    byte[] bytes = new byte[strBytes.Length];

                    for (int i = 0; i < strBytes.Length; i++)
                    {
                        if (!byte.TryParse(strBytes[i], out bytes[i]))
                            return null;
                    }
                    return new IPAddress(bytes);
                }
            }

            return null;
        }

        public static IPAddress GetSubnetMaskFromCIDRSubnet(string CIDRSubnet)
        {
            if (CIDRSubnet.Contains("/"))
            {
                int CIDRMask = 0;
                if (!int.TryParse(CIDRSubnet.Substring(CIDRSubnet.IndexOf('/') + 1), out CIDRMask))
                    return null;
                byte[] bytes = new byte[4];
                int index = 0;

                while (CIDRMask >= 8)
                {
                    bytes[index] = 255;
                    CIDRMask -= 8;
                    index++;
                }

                if (CIDRMask > 0)
                {
                    switch (CIDRMask)
                    {
                        case 1:
                            bytes[index] = 128;
                            break;
                        case 2:
                            bytes[index] = 192;
                            break;
                        case 3:
                            bytes[index] = 224;
                            break;
                        case 4:
                            bytes[index] = 240;
                            break;
                        case 5:
                            bytes[index] = 248;
                            break;
                        case 6:
                            bytes[index] = 252;
                            break;
                        case 7:
                            bytes[index] = 254;
                            break;
                    }
                }
                return new IPAddress(bytes);
            }
            return null;
        }

        /// <summary>
        /// Get the subnet address from an IPv4 address and is subnet mask.
        /// </summary>
        /// <param name="ipv4Address">IPv4 address from which you want to get the subnet address.</param>
        /// <param name="subnetMask">the subnet mask associated with the IPv4 address.</param>
        /// <returns>The subnet address or null if failed.</returns>
        /// <exception cref="ArgumentExecption">All argument must IPv4 addresses.</exception>
        public static IPAddress GetSubnetAddress(IPAddress ipv4Address, IPAddress subnetMask)
        {
            if (ipv4Address == null || subnetMask == null || ipv4Address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork || subnetMask.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                throw new ArgumentException("'ipv4Address' and 'subnetMask' must be both IPv4 address");

            try
            {
                byte[] address = ipv4Address.GetAddressBytes();
                byte[] mask = subnetMask.GetAddressBytes();

                if (address.Length == mask.Length)
                {
                    byte[] subnetAddress = new byte[mask.Length];

                    for (int i = 0; i < mask.Length; i++)
                    {
                        subnetAddress[i] = (byte)(address[i] & mask[i]);
                    }
                    return new IPAddress(subnetAddress);
                }
            }
            catch (Exception) { }

            return null;
        }
    }
}
