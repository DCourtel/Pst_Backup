using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SmartSingularity.PstBackupLogger
{
    public static class Logger
    {
        private static MessageSeverity _minimalSeverity = MessageSeverity.Information;
        private static bool _isLogActivated = false;

        public enum MessageSeverity
        {
            Debug,
            Information,
            Warning,
            Error
        }

        /// <summary>
        /// Gets or Sets the minimal level of severity. If a call to <see cref="Write(int, string, MessageSeverity, EventLogEntryType, string, string)"</see> is made with the "messageSeverity"
        /// below this property, the message will not be add to the Windows events log./> 
        /// </summary>
        public static MessageSeverity MinimalSeverity
        {
            get { return _minimalSeverity; }
            set { _minimalSeverity = value; }
        }

        /// <summary>
        /// Gets or Sets if messages must be add to Windows events log.
        /// </summary>
        public static bool IsLogActivated { get { return _isLogActivated; } set { _isLogActivated = value; } }
        
        /// <summary>
        /// Add a message in the Windows Event Log.
        /// </summary>
        /// <param name="eventID">ID for this event.</param>
        /// <param name="message">Text to write in the event.</param>
        /// <param name="messageSeverity">Severity of the message : Debug, Information, Warning, Error</param>
        /// <param name="eventType">Type of the event. Default value is "Information".</param>
        /// <param name="logName">Name of the log where to write the event. Default value is "Application".</param>
        /// <param name="sourceName">Name of the source of the event. Default value is "Pst Backup".</param>
        public static void Write(int eventID, string message, MessageSeverity messageSeverity, EventLogEntryType eventType = EventLogEntryType.Information, string logName = "Application", string sourceName = "Pst Backup")
        {
            try
            {
                if (IsLogActivated && (messageSeverity >= _minimalSeverity))
                {
                    EventLog eventLog = new EventLog(logName, ".", sourceName);
                    eventLog.WriteEntry(message, eventType, eventID);
                }
            }
            catch (Exception) { }
        }
    }
}
