using Aionian;
using AionianApp.ViewStates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
namespace AionianApp;
/// <summary>A viewmodel for the bible app</summary>
public class AppViewModel
{
	/// <summary>The reference to the parent running app</summary>
	protected readonly CoreApp RunningApp;
	/// <summary>The current bible loaded (if any)</summary>
	protected ChapterwiseBible? _currentBible;
	/// <summary>The current book portion </summary>
	protected Book _loadedPortion;
	/// <summary>The cached details of the books in this bible</summary>
	protected KeyValuePair<BibleBook, string>[] _booksData;
	/// <summary>The cached list of the bibles available to download</summary>
	protected List<Listing> _downloadLinks;
	/// <summary>The current state of the app</summary>
	protected AppViewState _state;
	/// <summary>The references that match the search parameters</summary>
	protected ObservableCollection<BibleReference> _searchList;
	/// <summary>Creates a viewmodel instance</summary>
	/// <param name="app">The parent CoreApp that loads the configuration</param>
	/// <param name="defaultLink">
	/// The link to load this viewmodel with (by default, the first link in CoreApp will be selected)
	/// </param>
	public AppViewModel(CoreApp app, BibleDescriptor? defaultLink = null)
	{
		_downloadLinks = new();
		RunningApp = app;
		_booksData = Array.Empty<KeyValuePair<BibleBook, string>>();
		_searchList = new();
		_state = new()
		{
			ContentState = new()
			{
				AvailableLinks = _downloadLinks,
				DownloadedBibles = AvailableBibles,
				Progress = 0,
				SelectedBible = null,
				SelectedLink = null,
			},
			SearchState = new()
			{
				AvailableBibleNames = AvailableBibles.Select(x => x.Title),
				SearchProgress = 0,
				SearchText = "",
				SelectedBible = "",
				SelectedMode = SearchMode.MatchAnyWord,
				FoundReferences = _searchList,
			},
			ReadState = new()
			{
				CurrentChapterContent = new(),
				CurrentLoadedBible = "",
				CurrentSelectedBook = 0,
				CurrentSelectedChapter = 1,
				LoadedBookNames = _booksData.Select(x => x.Value),
				LoadedBibles = AvailableBibles.Select(x => x.Title),
			},
		};
		if (defaultLink == null)
			ResetReadingList();
		else
			SelectBible(defaultLink.Value);
	}
	/// <summary>Resets the reading data. Should be used when the available bibles are modified</summary>
	/// <returns>Returns true if a bible is selected/loaded, otherwise false</returns>
	public bool ResetReadingList()
	{
		if (RunningApp.GetBibles().Any())
			return SelectBible(RunningApp.GetBibles().FirstOrDefault());
		_state.ReadState.CurrentChapterContent.Clear();
		_booksData = Array.Empty<KeyValuePair<BibleBook, string>>();
		_loadedPortion = new()
		{
			RegionalBookName = "",
			Chapter = new(),
			BookIndex = 0,
		};
		_state.ReadState.CurrentSelectedChapter = 0;
		return false;
	}
	/// <summary>Operation to select a bible</summary>
	/// <param name="b">The bible to load</param>
	/// <returns>true if the load is successful, otherwise false</returns>
	public bool SelectBible(BibleDescriptor b)
	{
		if (!AvailableBibles.Contains(b))
			return false;
		_currentBible = ChapterwiseBible.LoadChapterwiseBible(
			RunningApp,
			b);
		_booksData = _currentBible.Descriptor.RegionalName.ToArray();
		return LoadReading();
	}
	/// <summary>Loads the current reading portionn</summary>
	/// <param name="book">
	/// The book to load. By default, loads as NULL which is updated
	/// with the first book in the bible)
	/// </param>
	/// <param name="chapter">
	/// The chapter of the bible to load. By default, loads the first chapter.
	/// if 0 is provided for this value, it selects the last chapter of the book.
	/// </param>
	/// <returns>Returns true if the operation was successful, otherwise false</returns>
	public bool LoadReading(BibleBook book = BibleBook.NULL, byte chapter = 1)
	{
		Book portion;
		if (_currentBible == null)
			return false;
		else if (book == BibleBook.NULL)
			book = _currentBible.Descriptor.RegionalName.Keys.First();
		portion = CurrentBook != book ?
			_currentBible.FetchBook(book) :
			_loadedPortion;
		if (chapter > portion.Chapter.Count)
			return false;
		_loadedPortion = portion;
		chapter = chapter > 0 ? chapter : (byte)_loadedPortion.Chapter.Count;
		_state.ReadState.CurrentChapterContent = _loadedPortion.Chapter[chapter];
		_state.ReadState.CurrentSelectedChapter = chapter;
		return true;
	}
	/// <summary>Updates the reading portion with the next chapter of this bible</summary>
	/// <returns>Returns true if the operation was successful, otherwise false</returns>
	public bool NextChapter()
	{
		if (_currentBible == null)
			return false;
		else if (_state.ReadState.CurrentSelectedChapter < AvailableChapters)
			return LoadReading(CurrentBook, ++_state.ReadState.CurrentSelectedChapter);
		int idx = Array.FindIndex(
			_booksData,
			x => x.Key == CurrentBook);
		return idx >= 0 && LoadReading(
			_booksData[(idx + 1) % _booksData.Length].Key,
			1);
	}
	/// <summary>Updates the reading portion with the next chapter of this bible</summary>
	/// <returns>Returns true if the operation was successful, otherwise false</returns>
	public bool PrevChapter()
	{
		if (_currentBible == null)
			return false;
		else if (_state.ReadState.CurrentSelectedChapter > 1)
			return LoadReading(CurrentBook, --_state.ReadState.CurrentSelectedChapter);
		int idx = Array.FindIndex(
			_booksData,
			x => x.Key == CurrentBook);
		return idx >= 0 && LoadReading(
			_booksData[(_booksData.Length + idx - 1) % _booksData.Length].Key,
			0);
	}
	/// <summary>Updates the list of links that are available to download</summary>
	/// <returns>returns true if update is successful, otherwise false</returns>
	public bool UpdateDownloadableLinks()
	{
		try
		{
			Listing[] links = BibleLink.GetAllUrlsFromWebsite();
			_downloadLinks.Clear();
			_downloadLinks.AddRange(links);
		}
		catch { return false; }
		return true;
	}
	/// <summary>The names of the books in the currently loaded bible</summary>
	public string[] AvailableBookNames => _booksData.Select(
		x => x.Value).ToArray();
	/// <summary>The names of the books in the currently loaded bible</summary>
	public BibleBook[] AvailableBooks => _booksData.Select(
		x => x.Key).ToArray();
	/// <summary> Details of currently selected bible (if any)</summary>
	public BibleDescriptor SelectedBible => _currentBible?.Descriptor ?? BibleDescriptor.Empty;
	/// <summary>The current bible book loaded (if any)</summary>
	public BibleBook CurrentBook => _loadedPortion.CurrentBibleBook;
	/// <summary>The chapters in the currently loaded </summary>
	public int AvailableChapters => _loadedPortion.Chapter.Count;
	/// <summary>The list of bibles available to read</summary>
	public IEnumerable<BibleDescriptor> AvailableBibles => RunningApp.GetBibles();
	/// <summary>The number of books in this bible</summary>
	public int BookCount => _booksData.Length;
	/// <summary>The cached list of the bibles available to download</summary>
	public IEnumerable<Listing> DownloadableLinks => _downloadLinks;
	/// <summary>Gets the current state of the app</summary>
	public AppViewState CurrentState => _state;
}