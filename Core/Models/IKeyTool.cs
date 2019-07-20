using System.Collections.Generic;

namespace BundleToolUI.Models
{
    
    public interface IKeyTool
    {
        
        bool GetAliases(string keystorePath, string keystorePassword, out List<string> aliases);
        
    }
    
}