using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace CASU.Renderer.Direct3D11;

internal static class Program
{
    [STAThread]
    private static int Main()
    {
        Console.WriteLine("CASU_DIRECT3D11_RENDERER_CORE_TEST=START");

        ApplicationConfiguration.Initialize();

        using var window = new Form
        {
            Text = "CASU Neural Core — Direct3D Foundation",
            ClientSize = new Size(1280, 720),
            StartPosition = FormStartPosition.CenterScreen,
            BackColor = Color.Black
        };

        using var renderer =
            new Direct3DRenderer(window);

        try
        {
            window.Show();

            renderer.Initialize();

            Console.WriteLine("DIRECT3D11_RENDERER_INITIALIZE=PASS");

            var stopwatch = Stopwatch.StartNew();

            bool resizeExecuted = false;

            while (window.Created &&
                   stopwatch.Elapsed.TotalSeconds < 6.0)
            {
                Application.DoEvents();

                float seconds =
                    (float)stopwatch.Elapsed.TotalSeconds;

                renderer.Render(seconds);

                if (!resizeExecuted &&
                    stopwatch.Elapsed.TotalSeconds >= 2.0)
                {
                    window.ClientSize =
                        new Size(1024, 640);

                    renderer.Resize();

                    resizeExecuted = true;
                }

                System.Threading.Thread.Sleep(1);
            }

            Console.WriteLine("DIRECT3D11_FRAME_LOOP=PASS");

            if (!resizeExecuted)
            {
                Console.WriteLine("DIRECT3D11_RESIZE_TEST=FAIL");
                return 2;
            }

            Console.WriteLine("DIRECT3D11_RESIZE_TEST=PASS");

            window.Close();

            Application.DoEvents();

            Console.WriteLine("DIRECT3D11_WINDOW_EXIT=PASS");
            Console.WriteLine("CASU_DIRECT3D11_RENDERER_CORE_TEST=PASS");

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("CASU_DIRECT3D11_RENDERER_CORE_TEST=FAIL");
            Console.WriteLine($"ERROR_TYPE={ex.GetType().FullName}");
            Console.WriteLine($"ERROR_MESSAGE={ex.Message}");

            return 1;
        }
    }
}
