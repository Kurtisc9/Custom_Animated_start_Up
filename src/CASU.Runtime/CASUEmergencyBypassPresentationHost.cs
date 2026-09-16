using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CASU.Runtime
{
    public sealed class CASUBypassWindow : Form
    {
        private readonly Timer _timer;
        private readonly DateTime _started;
        private readonly int _displayNumber;
        private readonly bool _casuPrimary;

        public CASUBypassWindow(
            Screen screen,
            int displayNumber,
            bool casuPrimary)
        {
            _started = DateTime.UtcNow;
            _displayNumber = displayNumber;
            _casuPrimary = casuPrimary;

            Text = "CASU Bypass Test Display " +
                   displayNumber;

            FormBorderStyle =
                FormBorderStyle.None;

            StartPosition =
                FormStartPosition.Manual;

            Bounds = screen.Bounds;
            BackColor = Color.Black;
            TopMost = true;
            ShowInTaskbar = false;

            _timer = new Timer();
            _timer.Interval = 16;
            _timer.Tick += delegate
            {
                Invalidate();
            };

            _timer.Start();
        }

        protected override void OnPaint(
            PaintEventArgs e)
        {
            base.OnPaint(e);

            double seconds =
                (DateTime.UtcNow -
                 _started).TotalSeconds;

            int pulse =
                70 +
                (int)(
                    Math.Abs(
                        Math.Sin(
                            seconds * 1.4 +
                            _displayNumber * 0.3
                        )
                    ) * 170
                );

            int width =
                ClientRectangle.Width;

            int height =
                ClientRectangle.Height;

            using (Pen ring =
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
                    ring,
                    rect);
            }

            using (Font titleFont =
                new Font(
                    "Segoe UI",
                    34,
                    FontStyle.Bold))
            using (Font infoFont =
                new Font(
                    "Segoe UI",
                    14,
                    FontStyle.Regular))
            using (Brush titleBrush =
                new SolidBrush(Color.White))
            using (Brush infoBrush =
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

                string bypass =
                    "LEFT SHIFT = Emergency Bypass";

                SizeF titleSize =
                    e.Graphics.MeasureString(
                        title,
                        titleFont);

                SizeF subSize =
                    e.Graphics.MeasureString(
                        subtitle,
                        infoFont);

                SizeF bypassSize =
                    e.Graphics.MeasureString(
                        bypass,
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
                    centerY - 60);

                e.Graphics.DrawString(
                    subtitle,
                    infoFont,
                    infoBrush,
                    centerX -
                    subSize.Width / 2f,
                    centerY + 15);

                e.Graphics.DrawString(
                    bypass,
                    infoFont,
                    infoBrush,
                    centerX -
                    bypassSize.Width / 2f,
                    centerY + 50);
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

    public sealed class
        CASUEmergencyBypassContext :
        ApplicationContext
    {
        private const int WM_INPUT = 0x00FF;
        private const int RIM_TYPEKEYBOARD = 1;
        private const uint RID_INPUT = 0x10000003;
        private const uint RIDI_DEVICENAME = 0x20000007;
        private const uint RIDEV_INPUTSINK = 0x00000100;

        private const ushort
            LEFT_SHIFT_MAKECODE = 0x002A;

        private readonly
            List<CASUBypassWindow>
            _windows =
                new List<CASUBypassWindow>();

        private readonly RawInputSink _sink;

        private bool _bypassLatched;
        private int _bypassSignalCount;

        public CASUEmergencyBypassContext()
        {
            Screen[] screens =
                Screen.AllScreens;

            if (
                screens == null ||
                screens.Length == 0)
            {
                throw new
                    InvalidOperationException(
                        "No displays detected.");
            }

            int casuPrimaryIndex =
                screens.Length >= 2
                    ? 1
                    : 0;

            for (
                int i = 0;
                i < screens.Length;
                i++)
            {
                CASUBypassWindow window =
                    new CASUBypassWindow(
                        screens[i],
                        i + 1,
                        i == casuPrimaryIndex);

                _windows.Add(window);
            }

            _sink =
                new RawInputSink(
                    OnRawInput);

            foreach (
                CASUBypassWindow window
                in _windows)
            {
                window.Show();
            }
        }

        private void OnRawInput(
            IntPtr lParam)
        {
            uint size = 0;

            GetRawInputData(
                lParam,
                RID_INPUT,
                IntPtr.Zero,
                ref size,
                (uint)Marshal.SizeOf(
                    typeof(RAWINPUTHEADER)));

            if (size == 0)
                return;

            IntPtr buffer =
                Marshal.AllocHGlobal(
                    (int)size);

            try
            {
                uint read =
                    GetRawInputData(
                        lParam,
                        RID_INPUT,
                        buffer,
                        ref size,
                        (uint)Marshal.SizeOf(
                            typeof(RAWINPUTHEADER)));

                if (read != size)
                    return;

                RAWINPUT input =
                    (RAWINPUT)
                    Marshal.PtrToStructure(
                        buffer,
                        typeof(RAWINPUT));

                if (
                    input.header.dwType !=
                    RIM_TYPEKEYBOARD)
                {
                    return;
                }

                // Ignore break / key-up packets.
                if (
                    (input.keyboard.Flags &
                     0x0001) != 0)
                {
                    return;
                }

                if (
                    input.keyboard.MakeCode !=
                    LEFT_SHIFT_MAKECODE)
                {
                    return;
                }

                string device =
                    GetDeviceName(
                        input.header.hDevice);

                if (
                    String.IsNullOrEmpty(
                        device))
                {
                    return;
                }

                string upper =
                    device.ToUpperInvariant();

                if (
                    !upper.Contains(
                        "VID_1EA7") ||
                    !upper.Contains(
                        "PID_0169") ||
                    !upper.Contains(
                        "MI_00"))
                {
                    return;
                }

                TriggerBypass();
            }
            finally
            {
                Marshal.FreeHGlobal(
                    buffer);
            }
        }

        private void TriggerBypass()
        {
            if (_bypassLatched)
                return;

            _bypassLatched = true;
            _bypassSignalCount++;

            Console.WriteLine(
                "LEFT_SHIFT_DETECTED=TRUE");

            Console.WriteLine(
                "DEVICE_MATCH=" +
                "VID_1EA7_PID_0169_MI_00");

            Console.WriteLine(
                "BYPASS_SIGNAL_COUNT=" +
                _bypassSignalCount);

            Console.WriteLine(
                "ONE_SHOT_LATCH=PASS");

            ExitAll();
        }

        private void ExitAll()
        {
            foreach (
                CASUBypassWindow window
                in _windows.ToArray())
            {
                if (!window.IsDisposed)
                {
                    window.Close();
                }
            }

            ExitThread();
        }

        private static string
            GetDeviceName(
                IntPtr device)
        {
            uint size = 0;

            GetRawInputDeviceInfo(
                device,
                RIDI_DEVICENAME,
                IntPtr.Zero,
                ref size);

            if (size == 0)
                return String.Empty;

            StringBuilder builder =
                new StringBuilder(
                    (int)size + 1);

            uint result =
                GetRawInputDeviceInfo(
                    device,
                    RIDI_DEVICENAME,
                    builder,
                    ref size);

            if (result == UInt32.MaxValue)
                return String.Empty;

            return builder.ToString();
        }

        private sealed class
            RawInputSink : NativeWindow
        {
            private readonly
                Action<IntPtr> _callback;

            public RawInputSink(
                Action<IntPtr> callback)
            {
                _callback = callback;

                CreateHandle(
                    new CreateParams());

                RAWINPUTDEVICE[] devices =
                {
                    new RAWINPUTDEVICE
                    {
                        usUsagePage = 0x01,
                        usUsage = 0x06,
                        dwFlags =
                            RIDEV_INPUTSINK,
                        hwndTarget = Handle
                    }
                };

                if (
                    !RegisterRawInputDevices(
                        devices,
                        1,
                        (uint)Marshal.SizeOf(
                            typeof(
                                RAWINPUTDEVICE))))
                {
                    throw new
                        InvalidOperationException(
                            "Raw Input registration failed.");
                }
            }

            protected override void WndProc(
                ref Message m)
            {
                if (m.Msg == WM_INPUT)
                {
                    _callback(m.LParam);
                }

                base.WndProc(ref m);
            }
        }

        [StructLayout(
            LayoutKind.Sequential)]
        private struct RAWINPUTDEVICE
        {
            public ushort usUsagePage;
            public ushort usUsage;
            public uint dwFlags;
            public IntPtr hwndTarget;
        }

        [StructLayout(
            LayoutKind.Sequential)]
        private struct RAWINPUTHEADER
        {
            public int dwType;
            public int dwSize;
            public IntPtr hDevice;
            public IntPtr wParam;
        }

        [StructLayout(
            LayoutKind.Sequential)]
        private struct RAWKEYBOARD
        {
            public ushort MakeCode;
            public ushort Flags;
            public ushort Reserved;
            public ushort VKey;
            public uint Message;
            public uint ExtraInformation;
        }

        [StructLayout(
            LayoutKind.Explicit)]
        private struct RAWINPUT
        {
            [FieldOffset(0)]
            public RAWINPUTHEADER header;

            [FieldOffset(24)]
            public RAWKEYBOARD keyboard;
        }

        [DllImport(
            "User32.dll",
            SetLastError = true)]
        private static extern bool
            RegisterRawInputDevices(
                RAWINPUTDEVICE[] devices,
                uint numDevices,
                uint size);

        [DllImport(
            "User32.dll")]
        private static extern uint
            GetRawInputData(
                IntPtr rawInput,
                uint command,
                IntPtr data,
                ref uint size,
                uint headerSize);

        [DllImport(
            "User32.dll",
            CharSet = CharSet.Unicode)]
        private static extern uint
            GetRawInputDeviceInfo(
                IntPtr device,
                uint command,
                IntPtr data,
                ref uint size);

        [DllImport(
            "User32.dll",
            CharSet = CharSet.Unicode)]
        private static extern uint
            GetRawInputDeviceInfo(
                IntPtr device,
                uint command,
                StringBuilder data,
                ref uint size);
    }
}
