using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using AionianApp.ViewModels;
using AionianApp.Views;

namespace AionianApp;

public partial class App : Application
{
    public override void Initialize()
    {
        DataTemplates.Add(new ViewLocator());
        FluentThemePack.DefaultTheme.ApplyTheme(this);
    }
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
