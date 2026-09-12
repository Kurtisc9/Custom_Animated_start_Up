using System;

namespace CASU.Runtime
{
    public enum CasuRuntimeState
    {
        Starting,
        Running,
        BypassRequested,
        RestartRequested,
        SafeFallback,
        Exited
    }

    public enum CasuExitReason
    {
        None,
        EmergencyBypass,
        Crash,
        WatchdogFallback
    }

    public sealed class CasuRuntimeController
    {
        private const int MAX_RESTARTS = 1;

        private int restartCount;
        private bool bypassHandled;

        public CasuRuntimeState State { get; private set; }
        public CasuExitReason ExitReason { get; private set; }

        public int RestartCount
        {
            get { return restartCount; }
        }

        public bool SafeFallbackRequired
        {
            get { return State == CasuRuntimeState.SafeFallback; }
        }

        public event EventHandler ExitRequested;
        public event EventHandler RestartRequested;
        public event EventHandler SafeFallbackRequested;

        public CasuRuntimeController()
        {
            State = CasuRuntimeState.Starting;
            ExitReason = CasuExitReason.None;
        }

        public void MarkRunning()
        {
            if (State == CasuRuntimeState.Starting)
                State = CasuRuntimeState.Running;
        }

        public void RequestEmergencyBypass()
        {
            if (bypassHandled)
                return;

            bypassHandled = true;

            State = CasuRuntimeState.BypassRequested;
            ExitReason = CasuExitReason.EmergencyBypass;

            Console.WriteLine("RUNTIME_BYPASS_REQUEST=RECEIVED");
            Console.WriteLine("CASU_EXIT_REQUESTED=TRUE");
            Console.WriteLine("EXIT_REASON=EMERGENCY_BYPASS");

            EventHandler handler = ExitRequested;

            if (handler != null)
                handler(this, EventArgs.Empty);

            State = CasuRuntimeState.Exited;
        }

        public void ReportCrash()
        {
            ExitReason = CasuExitReason.Crash;

            Console.WriteLine("WATCHDOG_CRASH_DETECTED=TRUE");

            if (restartCount < MAX_RESTARTS)
            {
                restartCount++;

                State = CasuRuntimeState.RestartRequested;

                Console.WriteLine("WATCHDOG_RESTART_REQUESTED=TRUE");
                Console.WriteLine("WATCHDOG_RESTART_COUNT=" + restartCount);

                EventHandler handler = RestartRequested;

                if (handler != null)
                    handler(this, EventArgs.Empty);

                return;
            }

            State = CasuRuntimeState.SafeFallback;
            ExitReason = CasuExitReason.WatchdogFallback;

            Console.WriteLine("WATCHDOG_RESTART_LIMIT_REACHED=TRUE");
            Console.WriteLine("SAFE_FALLBACK_REQUESTED=TRUE");

            EventHandler fallback = SafeFallbackRequested;

            if (fallback != null)
                fallback(this, EventArgs.Empty);
        }

        public void ConfirmRestarted()
        {
            if (State != CasuRuntimeState.RestartRequested)
                throw new InvalidOperationException(
                    "Restart confirmation received from invalid state.");

            State = CasuRuntimeState.Running;

            Console.WriteLine("WATCHDOG_RESTART_CONFIRMED=TRUE");
        }
    }
}
