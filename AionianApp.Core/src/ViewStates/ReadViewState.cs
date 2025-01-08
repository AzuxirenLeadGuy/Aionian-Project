using Aionian;
using System.Collections.Generic;

namespace AionianApp.ViewStates;

/// <summary>Represents the ViewState of bible reading</summary>
public struct ReadViewState
{
	/// <summary>The currently loaded chapter content. Empty when uninitialized</summary>
	public Dictionary<byte, string> CurrentChapterContent { internal set; get; }
	/// <summary>The currently loaded chapter number. Zero when uninititialized</summary>
	public byte CurrentSelectedChapter { internal set; get; }
	/// <summary>The currently loaded bible. BibleDescriptor.Empty when uninitialized.</summary>
	public BibleDescriptor LoadedBible { internal set; get; }
	/// <summary>The bibles that are available to read right now. Sized 0 when uninitialized</summary>
	public List<BibleDescriptor> AvailableBibles { internal set; get; }
	/// <summary>The books of the bibles that are available to read right now. Sized 0 when uninitialized</summary>
	public Dictionary<BibleBook, string> AvailableBooksNames { internal set; get; }
	/// <summary>The number of chapters in the currently loaded book. Zero when uninitialized.</summary>
	public byte CurrentBookChapterCount { internal set; get; }
	/// <summary>The currently loaded book. BibleBook.NULL when uninitialized</summary>
	public BibleBook CurrentSelectedBook { internal set; get; }
}
