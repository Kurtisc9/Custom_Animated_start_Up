using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CASU.Runtime
{
    public sealed class CasuSessionTarget
    {
        public uint ActiveConsoleSessionId { get; private set; }
        public int CurrentProcessSessionId { get; private set; }
        public bool HasInteractiveTarget { get; private set; }
        public bool CurrentProcessMatchesTarget { get; private set; }

        public CasuSessionTarget(
            uint activeConsoleSessionId,
            int currentProcessSessionId,
            bool hasInteractiveTarget,
            bool currentProcessMatchesTarget)
        {
            ActiveConsoleSessionId = activeConsoleSessionId;
            CurrentProcessSessionId = currentProcessSessionId;
            HasInteractiveTarget = hasInteractiveTarget;
            CurrentProcessMatchesTarget = currentProcessMatchesTarget;
        }
    }

    public sealed class CasuActiveSessionResolver
    {
        private const uint INVALID_SESSION_ID = 0xFFFFFFFF;

        [DllImport("kernel32.dll")]
        private static extern uint WTSGetActiveConsoleSessionId();

        public CasuSessionTarget Resolve()
        {
            uint activeSession = WTSGetActiveConsoleSessionId();
            int currentSession = Process.GetCurrentProcess().SessionId;

            bool hasInteractiveTarget =
                activeSession != INVALID_SESSION_ID &&
                activeSession != 0;

            bool currentMatches =
                hasInteractiveTarget &&
                currentSession == (int)activeSession;

            return new CasuSessionTarget(
                activeSession,
                currentSession,
                hasInteractiveTarget,
                currentMatches);
        }
    }
}
