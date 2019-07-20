using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BundleToolUI.Models;

namespace BundleToolUI.MacOS
{
    public class MacOsKeyTool : IKeyTool
    {
        
        public bool GetAliases(string keystorePath, string keystorePassword, out List<string> aliases)
        {
            var process = new Process
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

            int exitCode = 1;
            string message = null;

            try
            {
                process.Start();
                message = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                exitCode = process.ExitCode;
            }
            catch (IOException e)
            {
                exitCode = 1;
            }

            if (exitCode != 0 || message == null)
            {
                aliases = null;
                return false;
            }

            MatchCollection matches = Regex.Matches(message, @"Alias name: (.*)\n");
            aliases = matches
                .Select(m => m.Groups[1].Value)
                .ToList();
            
            return true;
        }
        
    }
}