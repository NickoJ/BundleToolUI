using System.Diagnostics;
using BundleToolUI.Models;

namespace BundleToolUI.MacOS
{
    public class MacOsKeyTool : KeyTool
    {
        protected override Process CreateProcess(string keystorePath, string keystorePassword) =>
            new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"keytool -v -list -keystore {keystorePath} -storepass {keystorePassword}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

    }
}