using System.Diagnostics;
using BundleToolUI.Models;

namespace BundleToolUI.Win
{
    public class WinCommandExecutor : CommandExecutor
    {
        
        public WinCommandExecutor(CommandBuilder builder) : base(builder) {}

        protected override Process CreateProcess(string command) => new Process
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
        
    }
}