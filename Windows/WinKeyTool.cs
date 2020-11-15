using System.Diagnostics;
using BundleToolUI.Models;

namespace BundleToolUI.Win
{
    public sealed class WinKeyTool : KeyTool
    {
        
        protected override Process CreateProcess(string keystorePath, string keystorePassword) =>
            new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "CMD.exe",
                    Arguments = $"/C \"keytool -v -list -keystore {keystorePath} -storepass {keystorePassword}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

    }
}