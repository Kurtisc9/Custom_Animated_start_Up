namespace CASU.Themes
{
    public sealed class ThemeConfiguration
    {
        public string ThemeId { get; set; }

        public string DisplayName { get; set; }

        public string WelcomeMessage { get; set; }

        public string IdentityText { get; set; }

        public int PrimaryDisplayPreference { get; set; }

        public string StartupVariant { get; set; }

        public string SuccessSequence { get; set; }

        public string AnimationIntensity { get; set; }

        public string MultiDisplayMode { get; set; }

        public bool AudioEnabled { get; set; }

        public bool RandomizationEnabled { get; set; }
    }
}
