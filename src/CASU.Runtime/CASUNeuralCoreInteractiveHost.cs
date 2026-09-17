using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using CASU.Themes.NeuralCore;

namespace CASU.Runtime
{
    public sealed class NeuralCoreForm : Form
    {
        private readonly NeuralCoreRenderer renderer;
        private readonly Stopwatch clock;
        private readonly Timer timer;
        private readonly bool focal;
        private string stage;

        public NeuralCoreForm(
            Rectangle bounds,
            bool focalDisplay,
            int seed)
        {
            focal = focalDisplay;
            stage = "WAKE";

            Bounds = bounds;
            StartPosition = FormStartPosition.Manual;
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            BackColor = Color.Black;
            DoubleBuffered = true;

            renderer =
                new NeuralCoreRenderer(seed);

            clock = Stopwatch.StartNew();

            timer = new Timer();
            timer.Interval = 16;
            timer.Tick += delegate
            {
                UpdateStage();
                Invalidate();
            };

            timer.Start();
        }

        public string CurrentStage
        {
            get { return stage; }
        }

        private void UpdateStage()
        {
            double t = clock.Elapsed.TotalSeconds;

            if (t < 1.5)
                stage = "WAKE";
            else if (t < 3.5)
                stage = "CORE FORMATION";
            else if (t < 5.5)
                stage = "KURTISC ASSEMBLY";
            else if (t < 7.5)
                stage = "SYSTEM INITIALIZATION";
            else if (t < 9.5)
                stage = "CORE ENTRY";
            else if (t < 11.5)
                stage = "AUTH VISUAL";
            else
                stage = "SUCCESS EXIT";
        }

        protected override void OnPaint(
            PaintEventArgs e)
        {
            base.OnPaint(e);

            renderer.Render(
                e.Graphics,
                ClientRectangle,
                clock.Elapsed.TotalSeconds,
                focal,
                "KurtisC",
                stage);
        }

        protected override void Dispose(
            bool disposing)
        {
            if (disposing)
            {
                timer.Stop();
                timer.Dispose();
                clock.Stop();
            }

            base.Dispose(disposing);
        }
    }

    public static class CASUNeuralCoreInteractiveHost
    {
        [STAThread]
        public static void Run(int durationSeconds)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Screen[] screens = Screen.AllScreens;

            if (screens == null || screens.Length == 0)
                throw new InvalidOperationException(
                    "No displays detected.");

            int focalIndex =
                screens.Length >= 2 ? 1 : 0;

            var forms =
                new List<NeuralCoreForm>();

            int seed = Environment.TickCount;

            for (int i = 0; i < screens.Length; i++)
            {
                var form =
                    new NeuralCoreForm(
                        screens[i].Bounds,
                        i == focalIndex,
                        seed + (i * 997));

                forms.Add(form);
            }

            var shutdownTimer = new Timer();

            shutdownTimer.Interval =
                Math.Max(3, durationSeconds) * 1000;

            shutdownTimer.Tick += delegate
            {
                shutdownTimer.Stop();

                foreach (var form in forms)
                {
                    if (!form.IsDisposed)
                        form.Close();
                }

                Application.ExitThread();
            };

            foreach (var form in forms)
                form.Show();

            shutdownTimer.Start();

            Application.Run();

            shutdownTimer.Dispose();

            foreach (var form in forms)
                form.Dispose();
        }
    }
}

