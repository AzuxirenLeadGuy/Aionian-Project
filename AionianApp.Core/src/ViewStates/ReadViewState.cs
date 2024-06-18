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
	/// <summary>The list of books from the currently loaded bible</summary>
	public IEnumerable<string> LoadedBookNames { internal set; get; }
	/// <summary>The currently loaded book</summary>
	public BibleBook CurrentSelectedBook { internal set; get; }
	/// <summary>The list of bibles available in the app</summary>
	public IEnumerable<string> LoadedBibles { internal set; get; }
	/// <summary>The currently loaded bible name</summary>
	public BibleDescriptor CurrentLoadedBible { internal set; get; }
	/// <summary>If true, bible content is loaded. Otherwise false</summary>
	public readonly bool IsLoaded => CurrentLoadedBible != BibleDescriptor.Empty;
}
