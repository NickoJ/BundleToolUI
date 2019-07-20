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

        private const string FlagBundlePath = "--bundle";
        private const string FlagOutputPath = "--output";
        private const string FlagApksPath = "--apks";
        private const string FlagOverwriteOutput = "--overwrite";
        private const string FlagKeystorePath = "--ks";
        private const string FlagKeystorePassword = "--ks-pass";
        private const string FlagKeystoreAlias = "--ks-key-alias";
        private const string FlagAliasPassword = "--key-pass";
        private const string FlagApkWithDeviceTarget = "--connected-device";
        private const string FlagDeviceId = "--device-id";

        private const string QualifierPassword = "pass";
        
        public string Build(string bundleToolPath, ExecuteParams executeParams)
        {
            if (string.IsNullOrEmpty(bundleToolPath) || executeParams == null) return string.Empty;

            var sb = new StringBuilder();

            var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, bundleToolPath);
            sb.Append("java -jar").Append(' ').Append(fullPath).Append(' ');
            
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
            
            AddFlag(sb, FlagBundlePath, executeParams.BundlePath);
            AddFlag(sb, FlagOutputPath, executeParams.ApksPath);
            if (executeParams.OverwriteOutput) AddFlag(sb, FlagOverwriteOutput);

            AddKeystoreFlags(sb, executeParams);
            if (executeParams.ApkWithDeviceTarget) AddFlag(sb, FlagApkWithDeviceTarget);
            if (!string.IsNullOrWhiteSpace(executeParams.DeviceId)) AddFlag(sb, FlagDeviceId, executeParams.DeviceId);
        }

        private static void BuildForApksInstall(StringBuilder sb, ExecuteParams executeParams)
        {
            SetCommand(sb, CommandInstallApks);
            
            AddFlag(sb, FlagApksPath, executeParams.ApksPath);
            if (!string.IsNullOrWhiteSpace(executeParams.DeviceId)) AddFlag(sb, FlagDeviceId, executeParams.DeviceId);
        }

        private static void SetCommand(StringBuilder sb, string command)
        {
            sb.Append(command).Append(' ');
        }
        
        private static void AddKeystoreFlags(StringBuilder sb, ExecuteParams executeParams)
        {
            if (string.IsNullOrWhiteSpace(executeParams.KeystorePath)) return;
            
            AddFlag(sb, FlagKeystorePath, executeParams.KeystorePath);
            AddFlag(sb, FlagKeystorePassword, QualifierPassword, executeParams.KeystorePassword);
            AddFlag(sb, FlagKeystoreAlias, executeParams.KeystoreAlias);
            AddFlag(sb, FlagAliasPassword, QualifierPassword, executeParams.AliasPassword);
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
            sb.Append(flag).Append('=').Append(qualifier).Append(':').Append(value).Append(' ');
        }
        
    }
}