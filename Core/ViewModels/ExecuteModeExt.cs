using System;
using BundleToolUI.Models;

namespace BundleToolUI.ViewModels
{
    
    public static class ExecuteModeExt
    {

        public static string GetModeName(this ExecuteMode mode)
        {
            switch (mode)
            {
                case ExecuteMode.BuildApks: return "Build APKs";
                case ExecuteMode.InstallApks: return "Install APKs";
                case ExecuteMode.ShowApkSize: return "Show APK size";
                default: throw new NotImplementedException(mode.ToString());
            }
        }
        
    }
    
}