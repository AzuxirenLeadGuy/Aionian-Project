using Avalonia.Controls;

namespace AionianApp.Views;

public class SettingPage : UserControl
{
    protected AppViewModel _viewModel;
    public SettingPage(AppViewModel vm)
    {
        _viewModel = vm;
        Content = "Welcome to SettingPage";
    }
}