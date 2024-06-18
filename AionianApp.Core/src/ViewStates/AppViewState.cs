namespace AionianApp.ViewStates;
/// <summary>Represents a ViewState of the app at any given instance</summary>
public class AppViewState
{
	/// <summary>The various types of commands available</summary>
	public enum CommandType : byte
	{
		/// <summary>The command has not done any noticable update for the view</summary>
		NoUpdateDone,
		/// <summary>The command has updated the ReadState</summary>
		UpdateBibleReading,
		/// <summary>The command has added new assets to the ContentState</summary>
		AddedAsset,
		/// <summary>The command has removed existing assets to the ContentState</summary>
		RemovedAsset,
		/// <summary>The command has updated the SearchState</summary>
		PerformSearch,
		/// <summary>The command has sent some other changes to the view</summary>
		OtherUpdateType,
	}
	/// <summary>The last command type assigned on the app</summary>
	public CommandType LastCommand;
	/// <summary>Current ViewState for bible reading</summary>
	public ReadViewState ReadState;
	/// <summary>Current ViewState for content management</summary>
	public ContentViewState ContentState;
	/// <summary>Current ViewState for bible search</summary>
	public SearchViewState SearchState;
}