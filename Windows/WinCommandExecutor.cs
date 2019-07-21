using System.Diagnostics;
using BundleToolUI.Models;

namespace BundleToolUI.Win
{
    public class WinCommandExecutor : CommandExecutor
    {
        
        public WinCommandExecutor(CommandBuilder builder) : base(builder) {}

        protected override ExecuteResult Execute(string command)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "CMD.exe",
                    Arguments = $"/C {command}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            
            process.Start();
            string message = process.StandardOutput.ReadToEnd();
            string errorMessage = process.StandardError.ReadToEnd();
            process.WaitForExit();
            int exitCode = process.ExitCode;

            return new ExecuteResult(exitCode, message, errorMessage);
        }
        
    }
}