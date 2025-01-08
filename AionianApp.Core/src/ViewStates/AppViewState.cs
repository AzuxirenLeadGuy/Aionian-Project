using Aionian;
using System;
using System.Collections.Generic;

namespace AionianApp.ViewStates;
/// <summary>Represents a ViewState of the app at any given instance</summary>
public class AppViewState
{
	/// <summary>Current ViewState for bible reading</summary>
	protected internal ReadViewState _readState;
	/// <summary>Current ViewState for content management</summary>
	protected internal ContentViewState _contentState;
	/// <summary>Current ViewState for bible search</summary>
	protected internal SearchViewState _searchState;
	/// <summary>The directory where all data is stored</summary>
	public readonly string RootDir;
	/// <summary>Constructs a new ViewState instance</summary>
	public AppViewState(
		List<BibleDescriptor> offline_bibles,
		List<SearchedVerse> search_list,
		List<Listing> online_bibles,
		string root_dir
	)
	{
		_contentState = new()
		{
			AvailableLinks = online_bibles,
			LastRefreshed = DateTime.MinValue,
			OfflineBibles = offline_bibles,
		};
		_searchState = new()
		{
			FoundReferences = search_list
		};
		_readState = new()
		{
			CurrentChapterContent = new(),
			CurrentSelectedChapter = 0,
			LoadedBible = BibleDescriptor.Empty,
			AvailableBibles = offline_bibles,
			CurrentBookChapterCount = 0,
			CurrentSelectedBook = BibleBook.NULL,
		};
		RootDir = root_dir;
	}
	/// <summary>The state of the bible reading view</summary>
	public ReadViewState ReadState => _readState;
	/// <summary>The state of the bible search view</summary>
	public SearchViewState SearchState => _searchState;
	/// <summary>The state of the content manager view</summary>
	public ContentViewState ContentState => _contentState;
}