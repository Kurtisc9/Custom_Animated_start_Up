using System;
using System.Diagnostics;
using System.IO;

namespace CASU.Runtime
{
    public sealed class CasuInteractiveSessionLaunchResult
    {
        public int ProcessId { get; private set; }
        public int SessionId { get; private set; }
        public int ExitCode { get; private set; }

        public CasuInteractiveSessionLaunchResult(
            int processId,
            int sessionId,
            int exitCode)
        {
            ProcessId = processId;
            SessionId = sessionId;
            ExitCode = exitCode;
        }
    }

    public sealed class CasuInteractiveSessionLauncher
    {
        public int CurrentSessionId
        {
            get
            {
                return Process.GetCurrentProcess().SessionId;
            }
        }

        public bool IsInteractiveSession
        {
            get
            {
                return CurrentSessionId != 0;
            }
        }

        public CasuInteractiveSessionLaunchResult LaunchAndWait(
            string executable,
            string arguments,
            int timeoutMilliseconds)
        {
            if (String.IsNullOrWhiteSpace(executable))
                throw new ArgumentException("Executable is required.");

            if (!File.Exists(executable))
                throw new FileNotFoundException(
                    "Executable not found.",
                    executable);

            if (!IsInteractiveSession)
                throw new InvalidOperationException(
                    "Interactive launch from Session 0 is prohibited.");

            ProcessStartInfo info = new ProcessStartInfo();

            info.FileName = executable;
            info.Arguments = arguments ?? String.Empty;

            info.UseShellExecute = false;
            info.CreateNoWindow = true;

            Process process = Process.Start(info);

            if (process == null)
                throw new InvalidOperationException(
                    "Process launch returned null.");

            int processId = process.Id;
            int sessionId = process.SessionId;

            if (!process.WaitForExit(timeoutMilliseconds))
            {
                try
                {
                    process.Kill();
                }
                catch
                {
                }

                throw new TimeoutException(
                    "Interactive runtime prototype timed out.");
            }

            int exitCode = process.ExitCode;

            process.Dispose();

            return new CasuInteractiveSessionLaunchResult(
                processId,
                sessionId,
                exitCode);
        }
    }
}
