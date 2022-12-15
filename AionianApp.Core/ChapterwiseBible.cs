using Aionian;

using System;
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
		public Bible LoadedBible { get; }
		/// <summary>
		/// Loads the Bible from the asset
		/// </summary>
		/// <param name="loadedBible">The bible queried to load.</param>
		/// <returns>The bible object deserialized from the asset file</returns>
		public ChapterwiseBible(Bible loadedBible)
		{
			LoadedBible = loadedBible;
			Dictionary<BibleBook, Book> books = LoadedBible.Books;
			RegionalNames = new();
			foreach (var book in books)
				RegionalNames.Add(book.Key, book.Value.RegionalBookName);
			BookCounts = (byte)RegionalNames.Count;
		}
		/// <summary>Dictionary of all books available in this bible, mapped to their regional names.</summary>
		public Dictionary<BibleBook, string> RegionalNames { get; }
		/// <summary>The number of books in the loaded bible</summary>
		public readonly byte BookCounts;
		/// <summary>
		/// Returns the number of chapters in a given book of the loaded bible
		/// </summary>
		/// <param name="book">The book to check for the count of chapters</param>
		/// <returns>The number of chapters in the given book of the loaded bible</returns>
		public byte GetChapterCount(BibleBook book)
		{
			if (RegionalNames.ContainsKey(book) == false) throw new ArgumentException("The given key does not exist in the loaded bible!");
			return (byte)LoadedBible.Books[book].Chapter.Count;
		}
		/// <summary>Returns the next chapter for a given book and chapter in the loaded bible</summary>
		/// <param name="currentBook">The book currently at</param>
		/// <param name="currentChapter">The chapter currently at</param>
		/// <returns>Tuple of book and chapter that succeeds this current chapter</returns>
		public (BibleBook BookEnum, byte Chapter) NextChapter(BibleBook currentBook, byte currentChapter)
		{
			currentChapter++;
			if (GetChapterCount(currentBook) < currentChapter)
			{
				currentChapter = 1;
				BibleBook[] booklist = RegionalNames.Keys.ToArray();
				int idx = Array.IndexOf(booklist, currentBook);
				currentBook = booklist[(idx + 1)%booklist.Length];
			}
			return (currentBook, currentChapter);
		}
		/// <summary>Returns the next chapter for a given book and chapter in the loaded bible</summary>
		/// <param name="currentBook">The book currently at</param>
		/// <param name="currentChapter">The chapter currently at</param>
		/// <returns>Tuple of book and chapter that succeeds this current chapter</returns>
		public (BibleBook BookEnum, byte Chapter) PreviousChapter(BibleBook currentBook, byte currentChapter)
		{
			currentChapter--;
			if (currentChapter == 0)
			{
				BibleBook[] booklist = RegionalNames.Keys.ToArray();
				int idx = Array.IndexOf(booklist, currentBook);
				currentBook = booklist[(booklist.Length + idx - 1)%booklist.Length];
				currentChapter = GetChapterCount(currentBook);
			}
			return (currentBook, currentChapter);
		}
		/// <summary> Returns a collection of verse for the given chapter. </summary>
		/// <param name="book">BibleBook to load</param>
		/// <param name="chapter">Selected chapter</param>
		/// <returns>A dictionary of verses</returns>
		public Dictionary<byte, string> LoadChapter(BibleBook book, byte chapter) => LoadedBible.Books[book].Chapter[chapter];
	}
}