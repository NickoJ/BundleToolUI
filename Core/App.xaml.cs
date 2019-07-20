using Avalonia;
using Avalonia.Markup.Xaml;

namespace BundleToolUI
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
   }
}