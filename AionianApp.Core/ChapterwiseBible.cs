using Aionian;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace AionianApp
{
	/// <summary>A bible wrapped with helper functions that load a single chapter of a bible at a time</summary>
	public class ChapterwiseBible : Bible
	{
		/// <summary>The path of the books of Bible, given the root</summary>
		public static string GetBookPath(string root, BibleBook book) => $"{root}/{(byte)book}.dat";
		///<summary>The root folder where files for this bible is stored</summary>
		public readonly string RootPath;
		/// <summary>
		/// Loads the Bible from the asset
		/// </summary>
		/// <param name="loadedBible">The bible queried to load.</param>
		/// <param name="path">The path of bible books to load.</param>
		/// <returns>The bible object deserialized from the asset file</returns>
		public ChapterwiseBible(BibleDescriptor loadedBible, string path) : base(loadedBible) => RootPath = path;

		/// <summary>Fetches a book from this bible</summary>
		public override Book FetchBook(BibleBook book) =>
			JsonSerializer.Deserialize<Book>(
				File.ReadAllText(
					GetBookPath(RootPath, book)));
		/// <summary>The number of books in the loaded bible</summary>
		public int BookCounts => Descriptor.RegionalName.Keys.Count;
		/// <summary>
		/// Returns the number of chapters in a given book of the loaded bible
		/// </summary>
		/// <param name="book">The book to check for the count of chapters</param>
		/// <returns>The number of chapters in the given book of the loaded bible</returns>
		public byte GetChapterCount(BibleBook book) =>
			Descriptor.RegionalName.ContainsKey(book) ?
				(byte)FetchBook(book).Chapter.Count :
				throw new ArgumentException("Given key does not exist in this bible");
		/// <summary>Returns the next chapter for a given book and chapter in the loaded bible</summary>
		/// <param name="currentBook">The book currently at</param>
		/// <param name="currentChapter">The chapter currently at</param>
		/// <returns>Tuple of book and chapter that succeeds this current chapter</returns>
		public (BibleBook BookEnum, byte Chapter) NextChapter(
			BibleBook currentBook, 
			byte currentChapter)
		{
			currentChapter++;
			if (GetChapterCount(currentBook) < currentChapter)
			{
				currentChapter = 1;
				BibleBook[] booklist = Descriptor.RegionalName.Keys.ToArray();
				int idx = Array.IndexOf(booklist, currentBook);
				currentBook = booklist[(idx + 1) % booklist.Length];
			}
			return (currentBook, currentChapter);
		}
		/// <summary>Returns the next chapter for a given book and chapter in the loaded bible</summary>
		/// <param name="currentBook">The book currently at</param>
		/// <param name="currentChapter">The chapter currently at</param>
		/// <returns>Tuple of book and chapter that succeeds this current chapter</returns>
		public (BibleBook BookEnum, byte Chapter) PreviousChapter(
			BibleBook currentBook, 
			byte currentChapter)
		{
			currentChapter--;
			if (currentChapter == 0)
			{
				BibleBook[] booklist = Descriptor.RegionalName.Keys.ToArray();
				int idx = Array.IndexOf(booklist, currentBook);
				currentBook = booklist[(booklist.Length + idx - 1) % booklist.Length];
				currentChapter = GetChapterCount(currentBook);
			}
			return (currentBook, currentChapter);
		}
		/// <summary> Returns a collection of verse for the given chapter. </summary>
		/// <param name="book">BibleBook to load</param>
		/// <param name="chapter">Selected chapter</param>
		/// <returns>A dictionary of verses</returns>
		public Dictionary<byte, string> LoadChapter(BibleBook book, byte chapter) => 
			FetchBook(book).Chapter[chapter];
	}
}