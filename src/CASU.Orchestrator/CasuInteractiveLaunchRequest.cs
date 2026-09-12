using System;

namespace CASU.Orchestrator
{
    public sealed class CasuInteractiveLaunchRequest
    {
        public uint TargetSessionId { get; private set; }
        public string ExecutablePath { get; private set; }
        public string Arguments { get; private set; }

        public CasuInteractiveLaunchRequest(
            uint targetSessionId,
            string executablePath,
            string arguments)
        {
            if (targetSessionId == 0 || targetSessionId == 0xFFFFFFFF)
                throw new ArgumentOutOfRangeException("targetSessionId");

            if (String.IsNullOrWhiteSpace(executablePath))
                throw new ArgumentException(
                    "Executable path is required.",
                    "executablePath");

            TargetSessionId = targetSessionId;
            ExecutablePath = executablePath;
            Arguments = arguments ?? String.Empty;
        }
    }
}
