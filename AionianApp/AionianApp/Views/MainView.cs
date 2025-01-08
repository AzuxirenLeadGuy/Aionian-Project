using Avalonia.Controls;
namespace AionianApp.Views;

public class MainView : UserControl
{
	protected readonly TabControl _tabs;
    protected readonly AppViewModel _viewModel;
	public MainView()
	{
        _viewModel = new();
		HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
		VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
		Content = _tabs = new TabControl()
		{
			Items =
			{
                new TabItem()
                {
                    Content = new ReadPage(_viewModel),
                    Header = "Bible Reading",
                },
                new TabItem()
                {
                    Content = new AssetPage(_viewModel),
                    Header = "Asset Management",
                },
                new TabItem()
                {
                    Content = new SearchPage(_viewModel),
                    Header = "Verse Search",
                },
                new TabItem()
                {
                    Content = new SettingPage(_viewModel),
                    Header = "Settings",
                },
			}
		};
	}
}
