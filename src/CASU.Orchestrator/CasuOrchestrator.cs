using System;

namespace CASU.Orchestrator
{
    public enum OrchestratorState
    {
        Stopped,
        Starting,
        RuntimeRequested,
        RuntimeActive,
        RestartRequested,
        SafeFallback,
        Exited
    }

    public sealed class CasuOrchestrator
    {
        public const int MAX_RESTARTS = 1;

        private int restartCount;

        public OrchestratorState State { get; private set; }

        public int RestartCount
        {
            get { return restartCount; }
        }

        public event EventHandler RuntimeLaunchRequested;
        public event EventHandler SafeFallbackRequested;
        public event EventHandler ExitRequested;

        public CasuOrchestrator()
        {
            State = OrchestratorState.Stopped;
            restartCount = 0;
        }

        public void Start()
        {
            if (State != OrchestratorState.Stopped)
                return;

            State = OrchestratorState.Starting;
            RequestRuntimeLaunch();
        }

        public void ConfirmRuntimeActive()
        {
            State = OrchestratorState.RuntimeActive;
        }

        public void ReportRuntimeFailure()
        {
            if (restartCount < MAX_RESTARTS)
            {
                restartCount++;
                State = OrchestratorState.RestartRequested;
                RequestRuntimeLaunch();
                return;
            }

            State = OrchestratorState.SafeFallback;

            if (SafeFallbackRequested != null)
                SafeFallbackRequested(this, EventArgs.Empty);
        }

        public void RequestEmergencyBypass()
        {
            State = OrchestratorState.Exited;

            if (ExitRequested != null)
                ExitRequested(this, EventArgs.Empty);
        }

        private void RequestRuntimeLaunch()
        {
            State = OrchestratorState.RuntimeRequested;

            if (RuntimeLaunchRequested != null)
                RuntimeLaunchRequested(this, EventArgs.Empty);
        }
    }
}
