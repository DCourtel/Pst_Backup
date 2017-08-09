using System.Runtime.InteropServices;

namespace SmartSingularity.PstBackupAgent
{
    internal static class ExecutionState
    {
        public const uint ES_CONTINUOUS = 0x80000000;
        public const uint ES_SYSTEM_REQUIRED = 0x00000001;

        [DllImport("kernel32.dll")]
        public static extern uint SetThreadExecutionState(uint esFlags);
    }
}
