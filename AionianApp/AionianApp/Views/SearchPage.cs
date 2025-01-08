using Avalonia.Controls;

namespace AionianApp.Views;

public class SearchPage : UserControl
{
    protected AppViewModel _viewModel;
    public SearchPage(AppViewModel vm)
    {
        _viewModel = vm;
        Content = "Welcome to SearchPage";
    }
}