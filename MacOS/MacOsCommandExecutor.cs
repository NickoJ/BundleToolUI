using System;
using System.Diagnostics;
using BundleToolUI.Models;

namespace BundleToolUI.MacOS
{
    public class MacOsCommandExecutor : CommandExecutor
    {
        
        public MacOsCommandExecutor(CommandBuilder builder) : base(builder) {}

        protected override ExecuteResult Execute(string command)
        {
            command = command.Replace("\"", "\\\"");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{command}\"",
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