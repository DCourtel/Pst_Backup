using System;

namespace SmartSingularity.PstBackupExceptions
{
    public class NotEnoughEstimatedDiskSpace : Exception
    {
        public NotEnoughEstimatedDiskSpace(string destination)
        {
            Destination = destination;
        }

        /// <summary>
        /// Gets or Sets the destination where there is not enough disk space
        /// </summary>
        public string Destination { get; private set; }
    }
}
