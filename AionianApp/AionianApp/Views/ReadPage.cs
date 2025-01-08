using Avalonia.Controls;

namespace AionianApp.Views;

public class ReadPage : UserControl
{
    protected AppViewModel _viewModel;
    public ReadPage(AppViewModel vm)
    {
        _viewModel = vm;
        Content = "Welcome to ReadPage";
    }
}
