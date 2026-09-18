using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CASU.Renderer.Direct3D11;

internal sealed class NeuralCoreGpuWindow : Form
{
    public Direct3DRenderer Renderer { get; }

    public NeuralCoreGpuWindow(Rectangle bounds)
    {
        Bounds = bounds;
        StartPosition = FormStartPosition.Manual;
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        TopMost = true;
        BackColor = Color.Black;
        KeyPreview = true;

        KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Escape)
            {
                Console.WriteLine("GPU_MANUAL_EXIT_REQUESTED=TRUE");
                Application.Exit();
            }
        };

        Renderer = new Direct3DRenderer(this);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            Renderer.Dispose();

        base.Dispose(disposing);
    }
}

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        Console.WriteLine("CASU_GPU_PRESENTATION_RUNTIME=START");

        ApplicationConfiguration.Initialize();

        Screen[] screens = Screen.AllScreens;

        if (screens == null || screens.Length == 0)
        {
            Console.WriteLine("DISPLAY_DISCOVERY=FAIL");
            return 2;
        }

        int focalIndex = screens.Length >= 2 ? 1 : 0;

        Console.WriteLine($"DISPLAY_COUNT={screens.Length}");
        Console.WriteLine($"CASU_PRIMARY_DISPLAY=DISPLAY_{focalIndex + 1}");

        var windows = new List<NeuralCoreGpuWindow>();

        try
        {
            for (int i = 0; i < screens.Length; i++)
            {
                var window =
                    new NeuralCoreGpuWindow(screens[i].Bounds);

                windows.Add(window);

                window.Show();
                window.Renderer.Initialize();

                Console.WriteLine(
                    $"DISPLAY_{i + 1}_GPU_INITIALIZE=PASS");
            }

            var clock = Stopwatch.StartNew();

            while (windows.Any(w => w.Created))
            {
                Application.DoEvents();

                float seconds =
                    (float)clock.Elapsed.TotalSeconds;

                foreach (var window in windows)
                {
                    if (window.Created)
                        window.Renderer.Render(seconds);
                }

                System.Threading.Thread.Sleep(1);
            }

            Console.WriteLine("GPU_FRAME_LOOP=PASS");
            Console.WriteLine("CASU_GPU_PRESENTATION_RUNTIME=PASS");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("CASU_GPU_PRESENTATION_RUNTIME=FAIL");
            Console.WriteLine($"ERROR_TYPE={ex.GetType().FullName}");
            Console.WriteLine($"ERROR_MESSAGE={ex.Message}");
            return 1;
        }
        finally
        {
            foreach (var window in windows)
            {
                if (!window.IsDisposed)
                    window.Dispose();
            }
        }
    }
}

