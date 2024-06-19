using Aionian;
using System.Collections.Generic;

namespace AionianApp.ViewStates;
/// <summary>Represents the ViewState of the Bible-Search view of the app</summary>
public struct SearchViewState
{
	/// <summary>The references found in the search</summary>
	public IEnumerable<(BibleReference rf, string verse)> FoundReferences { internal set; get; }
	/// <summary>The current progress of search in interval (0, 1)</summary>
	public float SearchProgress { internal set; get; }
	/// <summary>Returns true if the search is ongoing or not </summary>
	public readonly bool SearchOngoing => SearchProgress > 0 && SearchProgress < 1;
}