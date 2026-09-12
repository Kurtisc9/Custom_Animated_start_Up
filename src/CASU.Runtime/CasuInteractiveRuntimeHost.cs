using System;

namespace CASU.Runtime
{
    public enum InteractiveRuntimeState
    {
        Created,
        Starting,
        Running,
        BypassRequested,
        Faulted,
        Exited
    }

    public sealed class CasuInteractiveRuntimeHost
    {
        public InteractiveRuntimeState State { get; private set; }

        public event EventHandler Started;
        public event EventHandler EmergencyBypassRequested;
        public event EventHandler RuntimeFaulted;
        public event EventHandler Exited;

        public CasuInteractiveRuntimeHost()
        {
            State = InteractiveRuntimeState.Created;
        }

        public void Start()
        {
            State = InteractiveRuntimeState.Starting;
            State = InteractiveRuntimeState.Running;

            if (Started != null)
                Started(this, EventArgs.Empty);
        }

        public void RequestEmergencyBypass()
        {
            if (State == InteractiveRuntimeState.Exited)
                return;

            State = InteractiveRuntimeState.BypassRequested;

            if (EmergencyBypassRequested != null)
                EmergencyBypassRequested(this, EventArgs.Empty);

            Exit();
        }

        public void ReportFault()
        {
            State = InteractiveRuntimeState.Faulted;

            if (RuntimeFaulted != null)
                RuntimeFaulted(this, EventArgs.Empty);
        }

        public void Exit()
        {
            State = InteractiveRuntimeState.Exited;

            if (Exited != null)
                Exited(this, EventArgs.Empty);
        }
    }
}
