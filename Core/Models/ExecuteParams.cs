namespace BundleToolUI.Models
{

    public class ExecuteParams
    {

        public string KeyToolPath { get; set; }
        public string BundleToolPath { get; set; }
        public string BundlePath { get; set; }
        public string ApksPath { get; set; }

        public bool OverwriteOutput { get; set; } = true;

        public string KeystorePath { get; set; }
        public string KeystorePassword { get; set; }
        public string KeystoreAlias { get; set; }
        public string AliasPassword { get; set; }

        public bool ApkWithDeviceTarget { get; set; }
        public string DeviceId { get; set; }

        public ExecuteMode ExecuteMode { get; set; }

    }
    
}