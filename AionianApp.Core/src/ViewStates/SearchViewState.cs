using Aionian;
using DynamicData.Binding;
using System.Collections.Generic;

namespace AionianApp.ViewStates;
/// <summary>Represents the ViewState of the Bible-Search view of the app</summary>
public struct SearchViewState
{
	/// <summary>The references found in the search</summary>
	public IEnumerable<BibleReference> FoundReferences { internal set; get; }
	/// <summary>The current percent (out of 100) of the search process</summary>
	public float SearchProgress { internal set; get; }
	/// <summary>Returns true if the search is ongoing or not </summary>
	public readonly bool SearchOngoing => SearchProgress == 0;
	/// <summary>The text input entered for search</summary>
	public string SearchText { internal set; get; }
	/// <summary>The searchmode currently selected</summary>
	public SearchMode SelectedMode { internal set; get; }
	/// <summary>The list of bible names available</summary>
	public IEnumerable<string> AvailableBibleNames { internal set; get; }
	/// <summary>The current element of Bible that is selected right now</summary>
	public string SelectedBible { internal set; get; }
	/// <summary>Search Query object from current data</summary>
	public readonly SearchQuery CurrentSearchQuery =>
		new(SearchText, SelectedMode);
}