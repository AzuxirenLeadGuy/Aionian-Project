using Avalonia.Controls;

namespace AionianApp.Views;

public class AssetPage : UserControl
{
    protected AppViewModel _viewModel;
    public AssetPage(AppViewModel vm)
    {
        _viewModel = vm;
        Content = "Welcome to AssetPage";
    }
}