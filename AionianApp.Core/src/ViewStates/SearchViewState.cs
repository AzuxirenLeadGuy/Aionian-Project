using Aionian;
using System.Collections.Generic;

namespace AionianApp.ViewStates;
/// <summary>Represents the ViewState of the Bible-Search view of the app</summary>
public struct SearchViewState
{
	/// <summary>The references found in the search</summary>
	public IEnumerable<SearchedVerse> FoundReferences { internal set; get; }
}