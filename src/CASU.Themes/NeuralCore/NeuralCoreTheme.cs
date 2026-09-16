using System;

namespace CASU.Themes.NeuralCore
{
    public sealed class NeuralCoreTheme : ICasuTheme
    {
        private bool initialized;

        public string ThemeId
        {
            get { return "neural-core"; }
        }

        public string DisplayName
        {
            get { return "KurtisC Neural Core"; }
        }

        public ThemeStage CurrentStage { get; private set; }

        public AuthenticationVisualState AuthenticationState
        {
            get;
            private set;
        }

        public void Initialize()
        {
            initialized = true;
            CurrentStage = ThemeStage.Wake;
            AuthenticationState =
                AuthenticationVisualState.Waiting;
        }

        public void EnterStage(ThemeStage stage)
        {
            if (!initialized)
                throw new InvalidOperationException(
                    "Neural Core theme is not initialized.");

            CurrentStage = stage;
        }

        public void SetAuthenticationVisualState(
            AuthenticationVisualState state)
        {
            if (!initialized)
                throw new InvalidOperationException(
                    "Neural Core theme is not initialized.");

            AuthenticationState = state;
        }

        public void Shutdown()
        {
            initialized = false;
        }
    }
}
