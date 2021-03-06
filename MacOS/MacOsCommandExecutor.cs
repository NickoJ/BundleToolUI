using System.Diagnostics;
using BundleToolUI.Models;

namespace BundleToolUI.MacOS
{
    public class MacOsCommandExecutor : CommandExecutor
    {
        
        public MacOsCommandExecutor(CommandBuilder builder) : base(builder) {}

        protected override Process CreateProcess(string command)
        {
            command = command.Replace("\"", "\\\"");

            return new Process
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
        }
        
    }
}