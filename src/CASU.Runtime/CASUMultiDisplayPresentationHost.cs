using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CASU.Runtime
{
    public sealed class CASUDisplayWindow : Form
    {
        private readonly Timer _timer;
        private readonly DateTime _started;
        private readonly int _displayNumber;
        private readonly bool _casuPrimary;
        private readonly Action _exitAll;

        public CASUDisplayWindow(
            Screen screen,
            int displayNumber,
            bool casuPrimary,
            Action exitAll)
        {
            _started = DateTime.UtcNow;
            _displayNumber = displayNumber;
            _casuPrimary = casuPrimary;
            _exitAll = exitAll;

            Text = "CASU Display " + displayNumber;

            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            Bounds = screen.Bounds;

            BackColor = Color.Black;
            TopMost = true;
            ShowInTaskbar = false;
            KeyPreview = true;

            _timer = new Timer();
            _timer.Interval = 16;
            _timer.Tick += OnTick;
            _timer.Start();

            KeyDown += OnKeyDown;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            double seconds =
                (DateTime.UtcNow - _started).TotalSeconds;

            int pulse =
                70 +
                (int)(
                    Math.Abs(
                        Math.Sin(
                            seconds * 1.4 +
                            (_displayNumber * 0.30)
                        )
                    ) * 170
                );

            int width = ClientRectangle.Width;
            int height = ClientRectangle.Height;

            using (var outer =
                new Pen(
                    Color.FromArgb(
                        255,
                        0,
                        pulse,
                        255),
                    5))
            {
                Rectangle rect =
                    new Rectangle(
                        width / 4,
                        height / 4,
                        width / 2,
                        height / 2);

                e.Graphics.DrawEllipse(
                    outer,
                    rect);
            }

            using (var inner =
                new Pen(
                    Color.FromArgb(
                        200,
                        pulse,
                        40,
                        255),
                    2))
            {
                Rectangle rect =
                    new Rectangle(
                        width / 3,
                        height / 3,
                        width / 3,
                        height / 3);

                e.Graphics.DrawEllipse(
                    inner,
                    rect);
            }

            using (var titleFont =
                new Font(
                    "Segoe UI",
                    34,
                    FontStyle.Bold))
            using (var infoFont =
                new Font(
                    "Segoe UI",
                    14,
                    FontStyle.Regular))
            using (var titleBrush =
                new SolidBrush(Color.White))
            using (var infoBrush =
                new SolidBrush(
                    Color.FromArgb(
                        190,
                        210,
                        230)))
            {
                string title = "KurtisC";

                string subtitle =
                    _casuPrimary
                        ? "CASU — Monitor 2 Primary"
                        : "CASU — Unified Display " +
                          _displayNumber;

                SizeF titleSize =
                    e.Graphics.MeasureString(
                        title,
                        titleFont);

                SizeF subtitleSize =
                    e.Graphics.MeasureString(
                        subtitle,
                        infoFont);

                float centerX =
                    width / 2f;

                float centerY =
                    height / 2f;

                e.Graphics.DrawString(
                    title,
                    titleFont,
                    titleBrush,
                    centerX -
                    titleSize.Width / 2f,
                    centerY -
                    titleSize.Height);

                e.Graphics.DrawString(
                    subtitle,
                    infoFont,
                    infoBrush,
                    centerX -
                    subtitleSize.Width / 2f,
                    centerY + 20);
            }
        }

        private void OnTick(
            object sender,
            EventArgs e)
        {
            Invalidate();
        }

        private void OnKeyDown(
            object sender,
            KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _exitAll();
            }
        }

        protected override void OnFormClosed(
            FormClosedEventArgs e)
        {
            _timer.Stop();
            _timer.Dispose();

            base.OnFormClosed(e);
        }
    }

    public sealed class CASUMultiDisplayContext :
        ApplicationContext
    {
        private readonly List<CASUDisplayWindow>
            _windows =
                new List<CASUDisplayWindow>();

        private bool _closing;

        public CASUMultiDisplayContext()
        {
            Screen[] screens =
                Screen.AllScreens;

            if (screens == null ||
                screens.Length == 0)
            {
                throw new InvalidOperationException(
                    "No displays detected.");
            }

            // Locked CASU rule:
            // Monitor 2 becomes the designated
            // CASU primary when connected.
            int casuPrimaryIndex =
                screens.Length >= 2
                    ? 1
                    : 0;

            for (
                int i = 0;
                i < screens.Length;
                i++)
            {
                CASUDisplayWindow window =
                    new CASUDisplayWindow(
                        screens[i],
                        i + 1,
                        i == casuPrimaryIndex,
                        ExitAll);

                _windows.Add(window);
            }

            foreach (
                CASUDisplayWindow window
                in _windows)
            {
                window.Show();
            }
        }

        private void ExitAll()
        {
            if (_closing)
                return;

            _closing = true;

            foreach (
                CASUDisplayWindow window
                in _windows.ToArray())
            {
                if (!window.IsDisposed)
                {
                    window.Close();
                }
            }

            ExitThread();
        }
    }
}
