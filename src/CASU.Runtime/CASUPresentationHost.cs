using System;
using System.Drawing;
using System.Windows.Forms;

namespace CASU.Runtime
{
    public sealed class CASUPresentationHost : Form
    {
        private readonly Timer _timer;
        private readonly DateTime _started;
        private bool _closing;

        public CASUPresentationHost()
        {
            _started = DateTime.UtcNow;

            Text = "CASU Presentation Runtime";
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.Manual;
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
                70 + (int)(Math.Abs(Math.Sin(seconds * 1.4)) * 150);

            using (var glow = new Pen(
                Color.FromArgb(255, 0, pulse, 255), 4))
            {
                var bounds =
                    new Rectangle(
                        ClientRectangle.Width / 4,
                        ClientRectangle.Height / 4,
                        ClientRectangle.Width / 2,
                        ClientRectangle.Height / 2);

                e.Graphics.DrawEllipse(glow, bounds);
            }

            using (var titleFont =
                new Font("Segoe UI", 34, FontStyle.Bold))
            using (var subFont =
                new Font("Segoe UI", 15, FontStyle.Regular))
            using (var titleBrush =
                new SolidBrush(Color.White))
            using (var subBrush =
                new SolidBrush(Color.FromArgb(180, 210, 230)))
            {
                const string title = "KurtisC";
                const string subtitle =
                    "CASU Interactive Presentation Runtime";

                SizeF titleSize =
                    e.Graphics.MeasureString(title, titleFont);

                SizeF subSize =
                    e.Graphics.MeasureString(subtitle, subFont);

                float centerX =
                    ClientRectangle.Width / 2f;

                float centerY =
                    ClientRectangle.Height / 2f;

                e.Graphics.DrawString(
                    title,
                    titleFont,
                    titleBrush,
                    centerX - titleSize.Width / 2f,
                    centerY - titleSize.Height / 2f);

                e.Graphics.DrawString(
                    subtitle,
                    subFont,
                    subBrush,
                    centerX - subSize.Width / 2f,
                    centerY + titleSize.Height);
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                ControlledExit();
            }
        }

        private void ControlledExit()
        {
            if (_closing)
                return;

            _closing = true;
            _timer.Stop();
            Close();
        }

        protected override void OnFormClosed(
            FormClosedEventArgs e)
        {
            _timer.Stop();
            _timer.Dispose();
            base.OnFormClosed(e);
        }
    }
}
