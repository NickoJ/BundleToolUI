using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Avalonia.Controls;
using BundleToolUI.Models;
using DynamicData;
using ReactiveUI;

namespace BundleToolUI.ViewModels
{
    
    public class MainWindowViewModel : ViewModelBase
    {

        private const string ExtAab = "aab";
        private const string ExtApks = "apks";
        private const string ExtKeystore = "keystore";
        private const string ExtJks = "jks";
        private const string ExtJar = "jar";
        private const string ExtBundleTemplate = "bndltmplt";

        private const string DefaultOutputFilename = "output." + ExtApks; 
        
        private readonly Window _window;

        private readonly KeyTool _keyTool;
        private readonly CommandExecutor _executor;
        private readonly TemplatesModule _templatesModule;
        private readonly ObservableAsPropertyHelper<bool> _isOnBuildMode;

        private readonly StringBuilder _logs = new StringBuilder();
        
        private bool _processing;

        private readonly ExecuteMode[] _executeModes;
        
        public MainWindowViewModel(Window window, KeyTool keyTool, CommandExecutor executor,
            TemplatesModule templatesModule)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
            _keyTool = keyTool ?? throw new ArgumentNullException(nameof(keyTool));
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _templatesModule = templatesModule ?? throw new ArgumentNullException(nameof(templatesModule));

            _executor.LogMessage += PrintMessage;

            this.WhenAnyValue(x => x.BundlePath, x => x.ApksPath)
                .Where(t => string.IsNullOrWhiteSpace(t.Item2))
                .Where(t => !string.IsNullOrWhiteSpace(t.Item1))
                .Where(t => string.Equals(Path.GetExtension(t.Item1)!.Substring(1), ExtAab))
                .Select(t => Path.ChangeExtension(t.Item1, ExtApks))
                .Subscribe(path => ApksPath = path);

            this.WhenAnyValue(x => x.KeystorePath, x => x.KeystorePassword)
                .Select(x => new { Path = x.Item1, Pass = x.Item2 })
                .Where(x => !string.IsNullOrWhiteSpace(x.Path))
                .Where(x => !string.IsNullOrWhiteSpace(x.Pass))
                .Where(x => (File.GetAttributes(x.Path) & FileAttributes.Directory) != FileAttributes.Directory)
                .Where(x => File.Exists(x.Path))
                .Subscribe(x => UpdateAliases());

            _executeModes = new[]
            {
                ExecuteMode.BuildApks,
                ExecuteMode.InstallApks,
                ExecuteMode.ShowApkSize
            };
            
            ExecuteModesNames = new ObservableCollection<string>(_executeModes.Select(m => m.GetModeName()));
            AvailableAliases = new ObservableCollection<string>();
            
            _isOnBuildMode = this.WhenAnyValue(x => x.SelectedExecuteModeIndex)
                .Select(index => _executeModes[index])
                .Select(mode => mode == ExecuteMode.BuildApks)
                .ToProperty(this, x => x.IsOnBuildMode);
        }

        private string BundleToolPath
        {
            get => ExecuteParams.BundleToolPath;
            set
            {
                if (string.Equals(ExecuteParams.BundleToolPath, value)) return;
                ExecuteParams.BundleToolPath = value;
                this.RaisePropertyChanged(nameof(BundleToolPath));
            }
        }
        
        private string BundlePath
        {
            get => ExecuteParams.BundlePath;
            set
            {
                if (string.Equals(ExecuteParams.BundlePath, value)) return;
                ExecuteParams.BundlePath = value;
                this.RaisePropertyChanged(nameof(BundlePath));
            }
        }

        private string ApksPath
        {
            get => ExecuteParams.ApksPath;
            set
            {
                if (string.Equals(ExecuteParams.ApksPath, value)) return;
                ExecuteParams.ApksPath = value;
                this.RaisePropertyChanged(nameof(ApksPath));
            }
        }

        private bool OverwriteOutput
        {
            get => ExecuteParams.OverwriteOutput;
            set => ExecuteParams.OverwriteOutput = value;
        }

        private string KeystorePath
        {
            get => ExecuteParams.KeystorePath;
            set
            {
                if (string.Equals(ExecuteParams.KeystorePath, value)) return;
                ExecuteParams.KeystorePath = value;
                this.RaisePropertyChanged(nameof(KeystorePath));
            }
        }

        private string KeystorePassword
        {
            get => ExecuteParams.KeystorePassword;
            set
            {
                if (ExecuteParams.KeystorePassword == value) return;
                
                ExecuteParams.KeystorePassword = value; 
                this.RaisePropertyChanged(nameof(KeystorePassword));
            }
        }

        private ObservableCollection<string> AvailableAliases { get; }

        private int SelectedAliasIndex
        {
            get
            {
                var index = AvailableAliases.IndexOf(ExecuteParams.KeystoreAlias);
                return Math.Max(index, 0);
            }
            set
            {
                var newAlias = AvailableAliases[value];
                if (string.Equals(ExecuteParams.KeystoreAlias, newAlias)) return;

                ExecuteParams.KeystoreAlias = newAlias;
                this.RaisePropertyChanged(nameof(SelectedAliasIndex));
            }
        }

        private string AliasPassword
        {
            get => ExecuteParams.AliasPassword;
            set => ExecuteParams.AliasPassword = value;
        }

        private bool ApkWithDeviceTarget
        {
            get => ExecuteParams.ApkWithDeviceTarget;
            set => ExecuteParams.ApkWithDeviceTarget = value;
        }

        private string DeviceId
        {
            get => ExecuteParams.DeviceId;
            set => ExecuteParams.DeviceId = value;
        }
        
        private ObservableCollection<string> ExecuteModesNames { get; }

        private int SelectedExecuteModeIndex
        {
            get => ExecuteModesNames.IndexOf(ExecuteParams.ExecuteMode.GetModeName());
            set
            {
                if (SelectedExecuteModeIndex == value) return;

                ExecuteParams.ExecuteMode = _executeModes[value];
                this.RaisePropertyChanged(nameof(SelectedExecuteModeIndex));
            }
        }

        private bool IsOnBuildMode => _isOnBuildMode.Value;

        private ExecuteParams ExecuteParams => _executor.ExecuteParams;

        private bool Processing
        {
            get => _processing;
            set => this.RaiseAndSetIfChanged(ref _processing, value);
        }

        private string Logs => _logs.ToString();

        private void UpdateAliases()
        {
            if (!_keyTool.GetAliases(KeystorePath, KeystorePassword, out var newAliases)) return;

            AvailableAliases.Clear();
            AvailableAliases.AddRange(newAliases);

            SelectedAliasIndex = SelectedAliasIndex;
        }

        private async void OpenTemplate()
        {
            var filters = new List<FileDialogFilter>();
            var extensions = new List<string> {ExtBundleTemplate};
            filters.Add(new FileDialogFilter { Name = "Template", Extensions = extensions});

            var dialog = new OpenFileDialog
            {
                AllowMultiple = false,
                Filters = filters,
                Title = "Select template"
            };

            var result = await dialog.ShowAsync(_window);
            if (result is null || result.Length == 0) return;
            
            if (!_templatesModule.TryLoadTemplate(result[0], out var template)) return;

            ApplyTemplate(template);
        }

        private void SaveTemplate()
        {
            if (string.IsNullOrWhiteSpace(_templatesModule.CurrentTemplatePath))
            {
                SaveTemplateAs();
            }
            else
            {
                SaveTemplate(_templatesModule.CurrentTemplatePath);
            }
        }

        private async void SaveTemplateAs()
        {
            var filters = new List<FileDialogFilter>();
            var extensions = new List<string> {ExtBundleTemplate};
            filters.Add(new FileDialogFilter { Name = "Template", Extensions = extensions});

            var dialog = new SaveFileDialog
            {
                Filters = filters,
                Title = "Select template"
            };
            
            var result = await dialog.ShowAsync(_window);
            if (result is null || result.Length == 0) return;
            
            SaveTemplate(result);
        }

        private void SaveTemplate(string path)
        {
            var template = GenerateTemplate();
            _templatesModule.SaveTemplate(path, template);
        }

        private async void OnBundleToolPathSelectClick()
        {
            var filters = new List<FileDialogFilter>();
            var extensions = new List<string> {ExtJar};
            filters.Add(new FileDialogFilter { Name = "BundleTool", Extensions = extensions });
            
            var dialog = new OpenFileDialog
            {
                AllowMultiple = false,
                Filters = filters,
                Title = "Select BundleTool",
            };
            
            var result = await dialog.ShowAsync(_window);
            if (result is null || result.Length == 0) return;
            
            BundleToolPath = result[0];
        }

        private async void OnBundlePathSelectClick()
        {
            var filters = new List<FileDialogFilter>();
            var extensions = new List<string> {ExtAab};
            filters.Add(new FileDialogFilter { Name = "Android App Bundle", Extensions = extensions });
            
            var dialog = new OpenFileDialog
            {
                AllowMultiple = false,
                Filters = filters,
                Title = "Select app bundle"
            };
            
            string[] result = await dialog.ShowAsync(_window);
            if (result?.Length == 0) return;
            
            // ReSharper disable once PossibleNullReferenceException
            BundlePath = result[0];
        }

        private async void OnApksPathSelectClick()
        {
            var fileName = ApksPath != null ? Path.GetFileName(ApksPath) : null;
            if (fileName == null)
            {
                fileName = BundlePath != null 
                    ? Path.ChangeExtension(BundlePath, ExtApks)
                    : DefaultOutputFilename;
            }

            var filters = new List<FileDialogFilter>();
            var extensions = new List<string> {ExtApks};
            filters.Add(new FileDialogFilter { Name = "apks", Extensions = extensions});

            var dialog = new SaveFileDialog
            {
                DefaultExtension = ExtApks,
                InitialFileName = fileName,
                Filters = filters,
                Title = "Save..."
            };

            string result = await dialog.ShowAsync(_window);
            if (result == null) return;

            result = Path.ChangeExtension(result, ExtApks);
            ApksPath = result;
        }

        private async void OnKeystorePathSelectClick()
        {
            var filters = new List<FileDialogFilter>();

            var extensions = new List<string> { ExtKeystore, ExtJks };

            filters.Add(new FileDialogFilter { Name = "keystore", Extensions = extensions });
            
            var dialog = new OpenFileDialog
            {
                AllowMultiple = false,
                Filters = filters,
                Title = "Select keystore"
            };
            
            var result = await dialog.ShowAsync(_window);
            if (result?.Length == 0) return;
            
            // ReSharper disable once PossibleNullReferenceException
            KeystorePath = result[0];
        }

        private async void OnExecuteClick()
        {
            Processing = true;

            _logs.Clear();
            PrintMessage($"Execution time: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");

            try
            {
                var result = await _executor.ExecuteAsync();
                PrintMessage($"Exit code: {result.ExitCode.ToString()}");
            }
            catch (Exception e)
            {
                PrintMessage($"{e.Message}\n");
                PrintMessage(e.Source);
                PrintMessage($"{e.StackTrace}\n");
            }
            finally
            {

                PrintMessage($"Finish time: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}\n");
                
                Processing = false;
            }
        }
        
        private Template GenerateTemplate()
        {
            return new Template(_executor.ExecuteParams);
        }

        private void ApplyTemplate(Template template)
        {
            BundleToolPath = template.BundleToolPath;
            BundlePath = template.BundlePath;
            ApksPath = template.ApksPath;

            OverwriteOutput = template.OverwriteOutput;

            KeystorePath = template.KeystorePath;
        }

        private void PrintMessage(string message)
        {
            _logs.AppendLine(message);
            this.RaisePropertyChanged(nameof(Logs));
        }
    }
    
}
