using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using BundleToolUI.Models;
using BundleToolUI.ViewModels;
using BundleToolUI.Views;

namespace BundleToolUI.Win
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            BuildAvaloniaApp().Start(AppMain, args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        private static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();

        // Your application's entry point. Here you can initialize your MVVM framework, DI
        // container, etc.
        private static void AppMain(Application app, string[] args)
        {
            var keytool = new WinKeyTool();
            var commandExecutor = new WinCommandExecutor(new CommandBuilder());
            var templatesModule = new TemplatesModule();

            var window = new MainWindow();
            window.DataContext = new MainWindowViewModel(window, keytool, commandExecutor, templatesModule);

            app.Run(window);
        }
            
    }
    
}
