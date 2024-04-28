using Aionian;
using AionianApp;
using System;
using System.Collections.Generic;
using System.Linq;
namespace AionianApp;
/// <summary>A viewmodel for the bible app</summary>
public class BibleReadingViewModel
{
	/// <summary>The reference to the parent running app</summary>
	protected readonly CoreApp RunningApp;
	/// <summary>The current bible loaded (if any)</summary>
	protected ChapterwiseBible? _currentBible;
	/// <summary>The current book portion </summary>
	protected Book _loadedPortion;
	/// <summary>The current bible chapter loaded (if any)</summary>
	public byte CurrentChapter { get; protected set; }
	/// <summary>The current chapterverses displayed(if any)</summary>
	public Dictionary<byte, string> CurrentReading { get; protected set; }
	/// <summary>The cached details of the books in this bible</summary>
	protected KeyValuePair<BibleBook, string>[] _booksData;
	/// <summary>Creates a viewmodel instance</summary>
	/// <param name="app">The parent CoreApp that loads the configuration</param>
	public BibleReadingViewModel(CoreApp app)
	{
		RunningApp = app;
		_booksData = Array.Empty<KeyValuePair<BibleBook, string>>();
		CurrentReading = new();
		if (
			RunningApp.GetBibles().Any() &&
			!SelectBible(RunningApp.GetBibles().FirstOrDefault()))
		{
			throw new Exception("Could not load bible portion");
		}
	}
	/// <summary>Operation to select a bible</summary>
	/// <param name="b">The bible to load</param>
	/// <returns>true if the load is successful, otherwise false</returns>
	public bool SelectBible(BibleDescriptor b)
	{
		if (!AvailableBibles.Contains(b))
			return false;
		_currentBible = RunningApp.LoadChapterwiseBible(b);
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
		CurrentReading = _loadedPortion.Chapter[chapter];
		CurrentChapter = chapter;
		return true;
	}
	/// <summary>Updates the reading portion with the next chapter of this bible</summary>
	/// <returns>Returns true if the operation was successful, otherwise false</returns>
	public bool NextChapter()
	{
		if (_currentBible == null)
			return false;
		else if (CurrentChapter < AvailableChapters)
			return LoadReading(CurrentBook, ++CurrentChapter);
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
		else if (CurrentChapter > 0)
			return LoadReading(CurrentBook, --CurrentChapter);
		int idx = Array.FindIndex(
			_booksData,
			x => x.Key == CurrentBook);
		return idx >= 0 && LoadReading(
			_booksData[(_booksData.Length + idx - 1) % _booksData.Length].Key,
			0);
	}
	/// <summary>The names of the books in the currently loaded bible</summary>
	public string[] AvailableBooks => _booksData.Select(
		x => x.Value).ToArray();
	/// <summary> Details of currently selected bible (if any)</summary>
	public BibleDescriptor SelectedBible => _currentBible?.Descriptor ?? default;
	/// <summary>The current bible book loaded (if any)</summary>
	public BibleBook CurrentBook => _loadedPortion.CurrentBibleBook;
	/// <summary>The chapters in the currently loaded </summary>
	public int AvailableChapters => _loadedPortion.Chapter.Count;
	/// <summary>The list of bibles available to read</summary>
	public IEnumerable<BibleDescriptor> AvailableBibles => RunningApp.GetBibles();
}