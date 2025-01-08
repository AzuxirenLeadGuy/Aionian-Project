using System;
using System.Reflection;
using Avalonia.ReactiveUI;
using AionianApp.ViewModels;

namespace AionianApp.Views;

public class MainWindow : ReactiveWindow<ViewModelBase>
{
    public MainWindow()
    {
        Title = "AionianApp";
        Content = new MainView();
        Icon = new(
            Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("iconfile")
            ?? throw new ArgumentException("Icon file could not be loaded"));
    }
}
