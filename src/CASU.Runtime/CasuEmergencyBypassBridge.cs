using System;
using CASU.Orchestrator;

namespace CASU.Runtime
{
    public sealed class CasuEmergencyBypassBridge
    {
        private readonly CasuOrchestrator orchestrator;
        private readonly CasuInteractiveRuntimeHost runtime;

        private bool bypassHandled;

        public bool BypassHandled
        {
            get { return bypassHandled; }
        }

        public CasuEmergencyBypassBridge(
            CasuOrchestrator orchestrator,
            CasuInteractiveRuntimeHost runtime)
        {
            if (orchestrator == null)
                throw new ArgumentNullException("orchestrator");

            if (runtime == null)
                throw new ArgumentNullException("runtime");

            this.orchestrator = orchestrator;
            this.runtime = runtime;

            this.runtime.EmergencyBypassRequested +=
                OnEmergencyBypassRequested;
        }

        private void OnEmergencyBypassRequested(
            object sender,
            EventArgs e)
        {
            if (bypassHandled)
                return;

            bypassHandled = true;

            orchestrator.RequestEmergencyBypass();
        }
    }
}
