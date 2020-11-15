using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Text;
using Avalonia;

namespace BundleToolUI.Models
{
    public class CommandBuilder
    {

        private const string CommandBuildApks = "build-apks";
        private const string CommandInstallApks = "install-apks";

        private const string BundlePath = "--bundle";
        private const string OutputPath = "--output";
        private const string ApksPath = "--apks";
        private const string OverwriteOutput = "--overwrite";
        private const string KeystorePath = "--ks";
        private const string KeystorePassword = "--ks-pass";
        private const string KeystoreAlias = "--ks-key-alias";
        private const string AliasPassword = "--key-pass";
        private const string ApkWithDeviceTarget = "--connected-device";
        private const string DeviceId = "--device-id";

        private const string QualifierPassword = "pass";
        
        public string Build(string bundleToolPath, ExecuteParams executeParams)
        {
            if (string.IsNullOrEmpty(bundleToolPath) || executeParams == null) return string.Empty;

            var sb = new StringBuilder();

            sb.Append("java -jar").Append(' ').Append(executeParams.BundleToolPath).Append(' ');
            
            switch (executeParams.ExecuteMode)
            {
                case ExecuteMode.BuildApks: BuildForApksBuild(sb, executeParams); break;
                case ExecuteMode.InstallApks: BuildForApksInstall(sb, executeParams); break;
            }

            return sb.ToString();
        }

        private static void BuildForApksBuild(StringBuilder sb, ExecuteParams executeParams)
        {
            SetCommand(sb, CommandBuildApks);
            
            AddFlag(sb, BundlePath, executeParams.BundlePath);
            AddFlag(sb, OutputPath, executeParams.ApksPath);
            if (executeParams.OverwriteOutput) AddFlag(sb, OverwriteOutput);

            AddKeystoreFlags(sb, executeParams);
            if (executeParams.ApkWithDeviceTarget) AddFlag(sb, ApkWithDeviceTarget);
            if (!string.IsNullOrWhiteSpace(executeParams.DeviceId)) AddFlag(sb, DeviceId, executeParams.DeviceId);
        }

        private static void BuildForApksInstall(StringBuilder sb, ExecuteParams executeParams)
        {
            SetCommand(sb, CommandInstallApks);
            
            AddFlag(sb, ApksPath, executeParams.ApksPath);
            if (!string.IsNullOrWhiteSpace(executeParams.DeviceId)) AddFlag(sb, DeviceId, executeParams.DeviceId);
        }

        private static void SetCommand(StringBuilder sb, string command)
        {
            sb.Append(command).Append(' ');
        }
        
        private static void AddKeystoreFlags(StringBuilder sb, ExecuteParams executeParams)
        {
            if (string.IsNullOrWhiteSpace(executeParams.KeystorePath)) return;
            
            AddFlag(sb, KeystorePath, executeParams.KeystorePath);
            AddFlag(sb, KeystorePassword, QualifierPassword, executeParams.KeystorePassword);
            AddFlag(sb, KeystoreAlias, executeParams.KeystoreAlias);
            AddFlag(sb, AliasPassword, QualifierPassword, executeParams.AliasPassword);
        }
        
        private static void AddFlag(StringBuilder sb, string flag)
        {
            sb.Append(flag).Append(' ');
        }
        
        private static void AddFlag(StringBuilder sb, string flag, string value)
        {
            sb.Append(flag).Append('=').Append(value).Append(' ');
        }

        private static void AddFlag(StringBuilder sb, string flag, string qualifier, string value)
        {
            sb.Append(flag).Append('=').Append('\"').Append(qualifier).Append(':').Append(value).Append('\"').Append(' ');
        }
        
    }
}