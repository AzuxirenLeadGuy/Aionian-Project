using System;
using Aionian;
using System.Collections.Generic;
using System.Linq;

namespace AionianApp
{
	/// <summary>A bible wrapped with helper functions that load a single chapter of a bible at a time</summary>
	public struct ChapterwiseBible
	{
		/// <summary>
		/// The bible to load verses from.
		/// </summary>
		public Bible? LoadedBible { get; private set; }
		/// <summary>
		/// Loads the Bible from the asset
		/// </summary>
		/// <param name="loadedBible">The bible queried to load.</param>
		/// <returns>The bible object deserialized from the asset file</returns>
		public ChapterwiseBible(Bible loadedBible)
		{
			LoadedBible = loadedBible;
			CurrentAllBooks = LoadedBible.Value.Books.Keys.ToArray();
			_currentBookIndex = 0;
			CurrentChapter = 1;
			LoadedChapter = null;
			CurrentBookRegionalName = null;
			LoadChapter(CurrentAllBooks[0], 1);
		}
		/// <summary>The currently loaded chapter. </summary>
		public Dictionary<byte, string>? LoadedChapter { get; private set; }
		/// <summary>An array of all books available in the Bible loaded by the <c>LoadBible</c> method</summary>
		public BibleBook[] CurrentAllBooks { get; private set; }
		/// <summary>The book of the loaded chapter. </summary>
		public BibleBook CurrentBook => CurrentAllBooks[_currentBookIndex];
		private byte _currentBookIndex;
		/// <summary>The chapter number of the loaded chapter. </summary>
		public byte CurrentChapter { get; private set; }
		/// <summary>The regional name of the loaded book. </summary>
		public string? CurrentBookRegionalName { get; private set; }
		/// <summary>
		/// Sets the `LoadedChapter` object with the given values
		/// </summary>
		/// <param name="book">BibleBook to load</param>
		/// <param name="chapter">Selected chapter</param>
		public void LoadChapter(BibleBook book, byte chapter)
		{
			if (LoadedBible == null) throw new ArgumentException("Bible is not loaded!");
			if (!LoadedBible.Value.Books.ContainsKey(book)) throw new ArgumentException("This book does not exist in this Bible", nameof(book));
			_currentBookIndex = (byte)Array.IndexOf(CurrentAllBooks, book);
			CurrentChapter = chapter;
			Book bookmem = LoadedBible.Value.Books[CurrentBook];
			LoadedChapter = bookmem.Chapter[CurrentChapter];
			CurrentBookRegionalName = bookmem.RegionalBookName;
		}
		/// <summary>Moves to the next chapter</summary>
		public void NextChapter()
		{
			if (LoadedBible == null) throw new Exception("The method 'LoadChapter()' must be called before. Currently the `LoadedBible` variable is null");
			if (LoadedBible.Value.Books[CurrentBook].Chapter.Count > CurrentChapter) CurrentChapter++;
			else
			{
				CurrentChapter = 1;
				if (_currentBookIndex != CurrentAllBooks.Length - 1) _currentBookIndex++;
				else _currentBookIndex = 0;
			}
		}
		/// <summary>Moves to the next chapter</summary>
		public void PreviousChapter()
		{
			if (LoadedBible == null) throw new Exception("The method 'LoadChapter()' must be called before. Currently the `LoadedBible` variable is null");
			if (1 < CurrentChapter) CurrentChapter--;
			else
			{
				if (_currentBookIndex != 0) _currentBookIndex--;
				else _currentBookIndex = (byte)(CurrentAllBooks.Length - 1); ;
				CurrentChapter = (byte)LoadedBible.Value.Books[CurrentBook].Chapter.Count;
			}
		}
	}
}