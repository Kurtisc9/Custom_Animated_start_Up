using System;
using System.Collections.Generic;

namespace CASU.Themes
{
    public sealed class ThemeEngine
    {
        private readonly Dictionary<string, ICasuTheme> themes =
            new Dictionary<string, ICasuTheme>(
                StringComparer.OrdinalIgnoreCase);

        public ICasuTheme ActiveTheme { get; private set; }

        public void Register(ICasuTheme theme)
        {
            if (theme == null)
                throw new ArgumentNullException("theme");

            themes[theme.ThemeId] = theme;
        }

        public void Activate(string themeId)
        {
            ICasuTheme theme;

            if (!themes.TryGetValue(themeId, out theme))
                throw new InvalidOperationException(
                    "Theme is not registered: " + themeId);

            if (ActiveTheme != null)
                ActiveTheme.Shutdown();

            ActiveTheme = theme;
            ActiveTheme.Initialize();
        }

        public void EnterStage(ThemeStage stage)
        {
            if (ActiveTheme == null)
                throw new InvalidOperationException(
                    "No active CASU theme.");

            ActiveTheme.EnterStage(stage);
        }

        public void SetAuthenticationVisualState(
            AuthenticationVisualState state)
        {
            if (ActiveTheme == null)
                throw new InvalidOperationException(
                    "No active CASU theme.");

            ActiveTheme.SetAuthenticationVisualState(state);
        }

        public void Shutdown()
        {
            if (ActiveTheme != null)
            {
                ActiveTheme.Shutdown();
                ActiveTheme = null;
            }
        }
    }
}
