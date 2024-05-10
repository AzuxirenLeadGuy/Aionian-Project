namespace AionianApp.ViewStates;
/// <summary>Represents a ViewState of the app at any given instance</summary>
public struct AppViewState
{
	/// <summary>Current ViewState for bible reading</summary>
	public ReadViewState ReadState;
	/// <summary>Current ViewState for content management</summary>
	public ContentViewState ContentState;
	/// <summary>Current ViewState for bible search</summary>
	public SearchViewState SearchState;
}