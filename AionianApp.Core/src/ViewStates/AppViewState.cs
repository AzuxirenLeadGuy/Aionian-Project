using Aionian;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AionianApp.ViewStates;
/// <summary>Represents a ViewState of the app at any given instance</summary>
public class AppViewState
{
	/// <summary>Current ViewState for bible reading</summary>
	public ReadViewState ReadState;
	/// <summary>Current ViewState for content management</summary>
	public ContentViewState ContentState;
	/// <summary>Current ViewState for bible search</summary>
	public SearchViewState SearchState;
	/// <summary>The list of bible names available</summary>
	public List<BibleDescriptor> AvailableBibles { internal set; get; }
	/// <summary>The directory where all data is stored</summary>
	public string RootDir;
	/// <summary>The currently loaded bible name</summary>
	public BibleDescriptor CurrentLoadedBible { internal set; get; }
	/// <summary>The names of the currently loaded bible books</summary>
	public IEnumerable<(BibleBook, string)> CurrentlyLoadedBookNames =>
		CurrentLoadedBible.RegionalName.Select(x => (x.Key, x.Value));
	/// <summary>If true, bible content is loaded. Otherwise false</summary>
	public bool IsLoaded => CurrentLoadedBible != BibleDescriptor.Empty;
	/// <summary>Constructs a new ViewState instance</summary>
	public AppViewState()
	{
		AvailableBibles = new();
		ContentState = new()
		{
			AvailableLinks = Array.Empty<Listing>(),
			Progress = 0,
		};
		SearchState = new()
		{
			SearchProgress = 0,
			FoundReferences = Array.Empty<(BibleReference, string)>(),
		};
		ReadState = new()
		{
			CurrentChapterContent = new(),
			CurrentSelectedBook = 0,
			CurrentSelectedChapter = 1,
			CurrentBookChapterCount = 0,
		};
		CurrentLoadedBible = default;
		RootDir = "";
	}
}