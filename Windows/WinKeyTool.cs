using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BundleToolUI.Models;

namespace BundleToolUI.Win
{
    public class WinKeyTool : IKeyTool
    {
        
        public bool GetAliases(string keystorePath, string keystorePassword, out List<string> aliases)
        {
            var process = new Process
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

            int exitCode = 1;
            string message = null;
            string errorMessage = null;
            
            try
            {
                process.Start();
                message = process.StandardOutput.ReadToEnd();
                errorMessage = process.StandardError.ReadToEnd();
                
                process.WaitForExit();
                exitCode = process.ExitCode;
            }
            catch (IOException e)
            {
                exitCode = 1;
            }

            if (!string.IsNullOrEmpty(errorMessage)) Console.WriteLine($"Error: {errorMessage}");
            
            if (exitCode != 0 || string.IsNullOrEmpty(message))
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