using Newtonsoft.Json;

namespace BundleToolUI.Models
{
    public sealed class Template
    {

        [JsonConstructor] public Template() {}
        
        public Template(ExecuteParams parameters)
        {
            KeyToolPath = parameters.KeyToolPath;
            BundleToolPath = parameters.BundleToolPath;
            BundlePath = parameters.BundlePath;
            ApksPath = parameters.ApksPath;
            OverwriteOutput = parameters.OverwriteOutput;
            KeystorePath = parameters.KeystorePath;
        }
        
        [JsonProperty] public string KeyToolPath { get; private set; }
        [JsonProperty] public string BundleToolPath { get; private set; }
        [JsonProperty] public string BundlePath { get; private set; }
        [JsonProperty] public string ApksPath { get; private set; }

        [JsonProperty] public bool OverwriteOutput { get; private set; }

        [JsonProperty] public string KeystorePath { get; private set; }
        
    }
}