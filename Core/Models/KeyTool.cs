using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BundleToolUI.Models
{
    
    public abstract class KeyTool
    {

        public bool GetAliases(string keystorePath, string keystorePassword, out List<string> aliases)
        {
            var process = CreateProcess(keystorePath, keystorePassword);

            int exitCode;
            string message;
            
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
                message = e.Message;
            }

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

        protected abstract Process CreateProcess(string keystorePath, string keystorePassword);

    }
    
}