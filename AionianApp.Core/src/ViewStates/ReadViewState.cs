using Aionian;
using System.Collections.Generic;

namespace AionianApp.ViewStates;

/// <summary>Represents the ViewState of bible reading</summary>
public struct ReadViewState
{
	/// <summary>The currently loaded chapter content</summary>
	public Dictionary<byte, string> CurrentChapterContent { internal set; get; }
	/// <summary>The currently loaded chapter number</summary>
	public byte CurrentSelectedChapter { internal set; get; }
	/// <summary>The currently loaded book</summary>
	public BibleBook CurrentSelectedBook { internal set; get; }
	/// <summary>The number of chapters in the currently loaded book</summary>
	public byte CurrentBookChapterCount { internal set; get; }
}
