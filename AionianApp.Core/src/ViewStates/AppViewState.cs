using Aionian;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AionianApp.ViewStates;
/// <summary>Represents a ViewState of the app at any given instance</summary>
public class AppViewState
{
	/// <summary>The various states an Aionian app can be in</summary>
	public enum State : byte
	{
		/// <summary>This application is not connected to its viewmodel</summary>
		Uninitialized,
		/// <summary>This application is just been connected to its viewmodel</summary>
		Initialized,
		/// <summary>This application has just loaded a chapter, updating its ReadState contnet</summary>
		ChapterLoaded,
		/// <summary>The available bible list has been updated (added or deleted)</summary>
		BiblesUpdated,
		/// <summary>The available download list has been updated</summary>
		DownloadLinksUpdated,
		/// <summary>The search results list has been updated</summary>
		SearchUpdated,
	}
	/// <summary>The state of the ViewModel since the last command/request</summary>
	public State CurrentAppState { internal set; get; }
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
			LastRefreshed = DateTime.MinValue,
		};
		SearchState = new()
		{
			FoundReferences = Array.Empty<SearchedVerse>(),
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
		CurrentAppState = State.Uninitialized;
	}
}