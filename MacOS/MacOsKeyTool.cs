using System.Diagnostics;
using BundleToolUI.Models;

namespace BundleToolUI.MacOS
{
    public class MacOsKeyTool : KeyTool
    {
        protected override Process CreateProcess(string keyToolPath, string keystorePath, string keystorePassword)
        {
            var keyTool = string.IsNullOrWhiteSpace(keyToolPath) ? "keytool" : $"\"{keyToolPath}\"";
            
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{keyTool} -v -list -keystore {keystorePath} -storepass {keystorePassword}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
        }
    }
}