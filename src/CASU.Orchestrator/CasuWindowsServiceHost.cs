using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;

namespace CASU.Orchestrator
{
    public sealed class CasuWindowsServiceHost : ServiceBase
    {
        private const uint InvalidSessionId = 0xFFFFFFFF;
        private const uint MaximumAllowed = 0x02000000;
        private const uint CreateUnicodeEnvironment = 0x00000400;
        private const uint CreateNoWindow = 0x08000000;

        private const uint WaitObject0 = 0x00000000;
        private const uint WaitTimeout = 0x00000102;

        private const int SecurityImpersonation = 2;
        private const int TokenPrimary = 1;

        private readonly ManualResetEvent _stopSignal =
            new ManualResetEvent(false);

        private Thread _worker;

        public static string ServiceNameValue =
            "CASUOrchestrator";

        public static string RuntimeCommand =
            null;

        public static string EvidencePath =
            null;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct STARTUPINFO
        {
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }

        [DllImport("kernel32.dll")]
        private static extern uint WTSGetActiveConsoleSessionId();

        [DllImport("Wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSQueryUserToken(
            uint SessionId,
            out IntPtr phToken);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool DuplicateTokenEx(
            IntPtr hExistingToken,
            uint dwDesiredAccess,
            IntPtr lpTokenAttributes,
            int ImpersonationLevel,
            int TokenType,
            out IntPtr phNewToken);

        [DllImport("userenv.dll", SetLastError = true)]
        private static extern bool CreateEnvironmentBlock(
            out IntPtr lpEnvironment,
            IntPtr hToken,
            bool bInherit);

        [DllImport("userenv.dll", SetLastError = true)]
        private static extern bool DestroyEnvironmentBlock(
            IntPtr lpEnvironment);

        [DllImport(
            "advapi32.dll",
            SetLastError = true,
            CharSet = CharSet.Unicode)]
        private static extern bool CreateProcessAsUser(
            IntPtr hToken,
            string lpApplicationName,
            string lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            uint dwCreationFlags,
            IntPtr lpEnvironment,
            string lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint WaitForSingleObject(
            IntPtr hHandle,
            uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetExitCodeProcess(
            IntPtr hProcess,
            out uint lpExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(
            IntPtr hObject);

        public CasuWindowsServiceHost()
        {
            ServiceName = ServiceNameValue;
            CanStop = true;
            CanShutdown = true;
            AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            WriteEvidence("SERVICE_ONSTART=PASS");

            _worker =
                new Thread(WorkerMain);

            _worker.IsBackground = true;
            _worker.Start();
        }

        protected override void OnStop()
        {
            _stopSignal.Set();
            WriteEvidence("SERVICE_ONSTOP=PASS");
        }

        protected override void OnShutdown()
        {
            _stopSignal.Set();
            WriteEvidence("SERVICE_ONSHUTDOWN=PASS");
        }

        private static void WriteEvidence(string text)
        {
            if (string.IsNullOrWhiteSpace(EvidencePath))
                return;

            File.AppendAllText(
                EvidencePath,
                text + Environment.NewLine);
        }

        private static void WriteFailure(string api)
        {
            int error =
                Marshal.GetLastWin32Error();

            WriteEvidence("FAILED_API=" + api);
            WriteEvidence("WIN32_ERROR_CODE=" + error);
            WriteEvidence(
                "WIN32_ERROR_MESSAGE=" +
                new Win32Exception(error).Message);
        }

        private void WorkerMain()
        {
            try
            {
                WindowsIdentity identity =
                    WindowsIdentity.GetCurrent();

                WriteEvidence(
                    "SERVICE_IDENTITY=" +
                    identity.Name);

                WriteEvidence(
                    "SERVICE_SESSION_ID=" +
                    Process.GetCurrentProcess().SessionId);

                ExecuteInteractiveRuntimeProbe();

                WriteEvidence(
                    "ORCHESTRATOR_WORKER_STATE=RUNNING");

                _stopSignal.WaitOne();
            }
            catch (Exception ex)
            {
                WriteEvidence(
                    "ORCHESTRATOR_EXCEPTION=" +
                    ex.GetType().FullName);

                WriteEvidence(
                    "ORCHESTRATOR_EXCEPTION_MESSAGE=" +
                    ex.Message);
            }
        }

        private static void ExecuteInteractiveRuntimeProbe()
        {
            IntPtr userToken = IntPtr.Zero;
            IntPtr primaryToken = IntPtr.Zero;
            IntPtr environment = IntPtr.Zero;

            PROCESS_INFORMATION processInfo =
                new PROCESS_INFORMATION();

            try
            {
                uint session =
                    WTSGetActiveConsoleSessionId();

                WriteEvidence(
                    "TARGET_SESSION_ID=" +
                    session);

                if (
                    session == InvalidSessionId ||
                    session == 0)
                {
                    WriteEvidence(
                        "ACTIVE_SESSION_TARGET_RESOLUTION=FAIL");

                    return;
                }

                WriteEvidence(
                    "ACTIVE_SESSION_TARGET_RESOLUTION=PASS");

                if (!WTSQueryUserToken(
                    session,
                    out userToken))
                {
                    WriteFailure("WTSQueryUserToken");
                    return;
                }

                WriteEvidence(
                    "USER_TOKEN_ACQUISITION=PASS");

                if (!DuplicateTokenEx(
                    userToken,
                    MaximumAllowed,
                    IntPtr.Zero,
                    SecurityImpersonation,
                    TokenPrimary,
                    out primaryToken))
                {
                    WriteFailure("DuplicateTokenEx");
                    return;
                }

                WriteEvidence(
                    "TOKEN_DUPLICATION=PASS");

                if (!CreateEnvironmentBlock(
                    out environment,
                    primaryToken,
                    false))
                {
                    WriteFailure("CreateEnvironmentBlock");
                    return;
                }

                WriteEvidence(
                    "ENVIRONMENT_BLOCK_CREATION=PASS");

                string powershell =
                    Path.Combine(
                        Environment.GetFolderPath(
                            Environment.SpecialFolder.System),
                        "WindowsPowerShell",
                        "v1.0",
                        "powershell.exe");

                string command =
                    RuntimeCommand;

                if (string.IsNullOrWhiteSpace(command))
                {
                    command =
                        "\"" + powershell + "\"" +
                        " -NoProfile -NonInteractive" +
                        " -Command \"exit 0\"";
                }

                STARTUPINFO startup =
                    new STARTUPINFO();

                startup.cb =
                    Marshal.SizeOf(typeof(STARTUPINFO));

                startup.lpDesktop =
                    @"winsta0\default";

                if (!CreateProcessAsUser(
                    primaryToken,
                    powershell,
                    command,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    false,
                    CreateUnicodeEnvironment |
                    CreateNoWindow,
                    environment,
                    null,
                    ref startup,
                    out processInfo))
                {
                    WriteFailure("CreateProcessAsUser");
                    return;
                }

                WriteEvidence(
                    "CROSS_SESSION_PROCESS_CREATION=PASS");

                WriteEvidence(
                    "CHILD_PROCESS_ID=" +
                    processInfo.dwProcessId);

                int childSession = -1;

                try
                {
                    using (
                        Process child =
                            Process.GetProcessById(
                                unchecked(
                                    (int)processInfo.dwProcessId)))
                    {
                        childSession =
                            child.SessionId;
                    }
                }
                catch
                {
                    childSession = -1;
                }

                WriteEvidence(
                    "CHILD_SESSION_ID=" +
                    childSession);

                if (childSession == (int)session)
                {
                    WriteEvidence(
                        "TARGET_SESSION_MATCH=PASS");
                }
                else
                {
                    WriteEvidence(
                        "TARGET_SESSION_MATCH=FAIL");
                }

                uint wait =
                    WaitForSingleObject(
                        processInfo.hProcess,
                        15000);

                if (wait == WaitTimeout)
                {
                    WriteEvidence(
                        "CHILD_PROCESS_WAIT=TIMEOUT");

                    return;
                }

                if (wait != WaitObject0)
                {
                    WriteFailure("WaitForSingleObject");
                    return;
                }

                uint exitCode;

                if (!GetExitCodeProcess(
                    processInfo.hProcess,
                    out exitCode))
                {
                    WriteFailure(
                        "GetExitCodeProcess");

                    return;
                }

                WriteEvidence(
                    "CHILD_EXIT_CODE=" +
                    exitCode);

                if (
                    childSession == (int)session &&
                    exitCode == 0)
                {
                    WriteEvidence(
                        "INTERACTIVE_RUNTIME_LAUNCH=PASS");
                }
                else
                {
                    WriteEvidence(
                        "INTERACTIVE_RUNTIME_LAUNCH=FAIL");
                }
            }
            finally
            {
                if (processInfo.hThread != IntPtr.Zero)
                {
                    WriteEvidence(
                        "THREAD_HANDLE_CLEANUP=" +
                        CloseHandle(
                            processInfo.hThread));
                }

                if (processInfo.hProcess != IntPtr.Zero)
                {
                    WriteEvidence(
                        "PROCESS_HANDLE_CLEANUP=" +
                        CloseHandle(
                            processInfo.hProcess));
                }

                if (environment != IntPtr.Zero)
                {
                    WriteEvidence(
                        "ENVIRONMENT_BLOCK_CLEANUP=" +
                        DestroyEnvironmentBlock(
                            environment));
                }

                if (primaryToken != IntPtr.Zero)
                {
                    WriteEvidence(
                        "PRIMARY_TOKEN_HANDLE_CLEANUP=" +
                        CloseHandle(
                            primaryToken));
                }

                if (userToken != IntPtr.Zero)
                {
                    WriteEvidence(
                        "USER_TOKEN_HANDLE_CLEANUP=" +
                        CloseHandle(
                            userToken));
                }

                WriteEvidence(
                    "R3D17_RUNTIME_PROBE_COMPLETED=TRUE");
            }
        }

        public static void Main(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg.StartsWith("--service-name="))
                {
                    ServiceNameValue =
                        arg.Substring(
                            "--service-name=".Length);
                }

                if (arg.StartsWith("--evidence="))
                {
                    EvidencePath =
                        arg.Substring(
                            "--evidence=".Length);
                }
            }

            ServiceBase.Run(
                new CasuWindowsServiceHost());
        }
    }
}
