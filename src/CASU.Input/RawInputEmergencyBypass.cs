using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CASU.Input
{
    public sealed class EmergencyBypassForm : Form
    {
        private const int WM_INPUT = 0x00FF;
        private const int RID_INPUT = 0x10000003;
        private const int RIDI_DEVICENAME = 0x20000007;

        private const uint RIDEV_INPUTSINK = 0x00000100;

        private const ushort HID_USAGE_PAGE_GENERIC = 0x01;
        private const ushort HID_USAGE_GENERIC_KEYBOARD = 0x06;

        private const ushort LEFT_SHIFT_MAKECODE = 0x002A;

        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_SYSKEYDOWN = 0x0104;

        private const string TARGET_VID = "VID_1EA7";
        private const string TARGET_PID = "PID_0169";
        private const string TARGET_INTERFACE = "MI_00";

        private bool bypassLatched;

        public int RawKeyboardPackets { get; private set; }
        public int TargetPackets { get; private set; }
        public int LeftShiftKeyDownEvents { get; private set; }
        public int BypassSignalCount { get; private set; }

        public string LastDeviceName { get; private set; }
        public string LastResult { get; private set; }

        public event EventHandler EmergencyBypassRequested;

        public EmergencyBypassForm()
        {
            Text = "CASU Emergency Bypass Runtime Test";
            Width = 720;
            Height = 220;
            StartPosition = FormStartPosition.CenterScreen;

            Label instructions = new Label();
            instructions.Dock = DockStyle.Fill;
            instructions.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            instructions.Font = new System.Drawing.Font("Segoe UI", 13F);
            instructions.Text =
                "CASU 02B-R3D2 Runtime Test\r\n\r\n" +
                "Press LEFT SHIFT once on the verified physical keyboard.\r\n" +
                "The bypass must trigger exactly once.";

            Controls.Add(instructions);

            FormClosed += delegate
            {
                Application.ExitThread();
            };
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            RegisterKeyboardRawInput();
        }

        private void RegisterKeyboardRawInput()
        {
            RAWINPUTDEVICE[] devices = new RAWINPUTDEVICE[1];

            devices[0].usUsagePage = HID_USAGE_PAGE_GENERIC;
            devices[0].usUsage = HID_USAGE_GENERIC_KEYBOARD;
            devices[0].dwFlags = RIDEV_INPUTSINK;
            devices[0].hwndTarget = Handle;

            if (!RegisterRawInputDevices(
                    devices,
                    (uint)devices.Length,
                    (uint)Marshal.SizeOf(typeof(RAWINPUTDEVICE))))
            {
                throw new InvalidOperationException(
                    "RegisterRawInputDevices failed. Win32=" +
                    Marshal.GetLastWin32Error());
            }

            Console.WriteLine("RAW_INPUT_REGISTRATION=PASS");
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_INPUT)
            {
                ProcessRawInput(m.LParam);
            }

            base.WndProc(ref m);
        }

        private void ProcessRawInput(IntPtr rawInputHandle)
        {
            uint size = 0;
            uint headerSize = (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER));

            uint probe = GetRawInputData(
                rawInputHandle,
                RID_INPUT,
                IntPtr.Zero,
                ref size,
                headerSize);

            if (size == 0)
                return;

            IntPtr buffer = Marshal.AllocHGlobal((int)size);

            try
            {
                uint read = GetRawInputData(
                    rawInputHandle,
                    RID_INPUT,
                    buffer,
                    ref size,
                    headerSize);

                if (read != size)
                    return;

                RAWINPUT raw = Marshal.PtrToStructure<RAWINPUT>(buffer);

                // RIM_TYPEKEYBOARD = 1
                if (raw.header.dwType != 1)
                    return;

                RawKeyboardPackets++;

                string device = GetDeviceName(raw.header.hDevice);
                LastDeviceName = device;

                bool targetDevice =
                    !String.IsNullOrWhiteSpace(device) &&
                    device.IndexOf(TARGET_VID, StringComparison.OrdinalIgnoreCase) >= 0 &&
                    device.IndexOf(TARGET_PID, StringComparison.OrdinalIgnoreCase) >= 0 &&
                    device.IndexOf(TARGET_INTERFACE, StringComparison.OrdinalIgnoreCase) >= 0;

                if (targetDevice)
                    TargetPackets++;

                bool isLeftShift =
                    raw.keyboard.MakeCode == LEFT_SHIFT_MAKECODE;

                bool isKeyDown =
                    raw.keyboard.Message == WM_KEYDOWN ||
                    raw.keyboard.Message == WM_SYSKEYDOWN;

                Console.WriteLine(
                    "RAW_PACKET DEVICE={0} MAKECODE=0x{1:X2} FLAGS=0x{2:X2} VKEY=0x{3:X2} MESSAGE=0x{4:X} TARGET={5}",
                    device,
                    raw.keyboard.MakeCode,
                    raw.keyboard.Flags,
                    raw.keyboard.VKey,
                    raw.keyboard.Message,
                    targetDevice ? "MATCH" : "NO_MATCH");

                if (!targetDevice || !isLeftShift || !isKeyDown)
                    return;

                LeftShiftKeyDownEvents++;

                if (bypassLatched)
                {
                    Console.WriteLine("BYPASS_DUPLICATE_SUPPRESSED=TRUE");
                    return;
                }

                bypassLatched = true;
                BypassSignalCount++;

                LastResult = "PASS";

                Console.WriteLine("LEFT_SHIFT_DETECTED=TRUE");
                Console.WriteLine("DEVICE_MATCH=VID_1EA7_PID_0169_MI_00");
                Console.WriteLine("BYPASS_REQUESTED=TRUE");
                Console.WriteLine("BYPASS_SIGNAL_COUNT=" + BypassSignalCount);
                Console.WriteLine("BYPASS_LATCHED=TRUE");

                EventHandler handler = EmergencyBypassRequested;

                if (handler != null)
                    handler(this, EventArgs.Empty);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        private static string GetDeviceName(IntPtr device)
        {
            uint chars = 0;

            GetRawInputDeviceInfo(
                device,
                RIDI_DEVICENAME,
                IntPtr.Zero,
                ref chars);

            if (chars == 0)
                return String.Empty;

            StringBuilder builder = new StringBuilder((int)chars + 1);

            uint result = GetRawInputDeviceInfo(
                device,
                RIDI_DEVICENAME,
                builder,
                ref chars);

            if (result == UInt32.MaxValue)
                return String.Empty;

            return builder.ToString();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RAWINPUTDEVICE
        {
            public ushort usUsagePage;
            public ushort usUsage;
            public uint dwFlags;
            public IntPtr hwndTarget;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RAWINPUTHEADER
        {
            public uint dwType;
            public uint dwSize;
            public IntPtr hDevice;
            public IntPtr wParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RAWKEYBOARD
        {
            public ushort MakeCode;
            public ushort Flags;
            public ushort Reserved;
            public ushort VKey;
            public uint Message;
            public uint ExtraInformation;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct RAWINPUT
        {
            [FieldOffset(0)]
            public RAWINPUTHEADER header;

            [FieldOffset(24)]
            public RAWKEYBOARD keyboard;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterRawInputDevices(
            RAWINPUTDEVICE[] pRawInputDevices,
            uint uiNumDevices,
            uint cbSize);

        [DllImport("user32.dll")]
        private static extern uint GetRawInputData(
            IntPtr hRawInput,
            uint uiCommand,
            IntPtr pData,
            ref uint pcbSize,
            uint cbSizeHeader);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern uint GetRawInputDeviceInfo(
            IntPtr hDevice,
            uint uiCommand,
            StringBuilder pData,
            ref uint pcbSize);

        [DllImport("user32.dll")]
        private static extern uint GetRawInputDeviceInfo(
            IntPtr hDevice,
            uint uiCommand,
            IntPtr pData,
            ref uint pcbSize);
    }
}
