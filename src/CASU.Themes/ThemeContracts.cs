using System;

namespace CASU.Themes
{
    public enum ThemeStage
    {
        Wake,
        CoreFormation,
        KurtisCAssembly,
        SystemInitialization,
        CoreEntry,
        AuthTransition,
        SuccessExit
    }

    public enum AuthenticationVisualState
    {
        Waiting,
        Failure,
        Success
    }

    public enum SuccessSequence
    {
        CoreExplosionDissolve,
        SpaceshipTakeoff
    }

    public interface ICasuTheme
    {
        string ThemeId { get; }

        string DisplayName { get; }

        ThemeStage CurrentStage { get; }

        void Initialize();

        void EnterStage(ThemeStage stage);

        void SetAuthenticationVisualState(
            AuthenticationVisualState state);

        void Shutdown();
    }
}
