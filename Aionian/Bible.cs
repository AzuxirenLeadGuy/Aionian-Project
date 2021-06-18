using System;
using System.Collections.Generic;
using System.IO;
namespace Aionian
{
	/// <summary> This denotes a Bible of a specific Language and Version/Title</summary>
	[Serializable]
	public struct Bible
	{
		/// <summary> The Language of the bible </summary>
		public readonly string Language;
		/// <summary> The Specific bible </summary>
		public readonly string Title;
		/// <summary> Indicates whether this Bible is Aionian or Standard edition </summary>
		public readonly bool AionianEdition;
		/// <summary> The Collection of books </summary>
		public readonly Dictionary<BibleBook, Book> Books;
		/// <summary>
		/// Default constructor for the Bible type
		/// </summary>
		/// <param name="title">Title</param>
		/// <param name="language">Language</param>
		/// <param name="aionianEdition">Aionian-Edition bool</param>
		/// <param name="books">Dictionary of Bible content</param>
		public Bible(string title, string language, bool aionianEdition, Dictionary<BibleBook, Book> books)
		{
			Language = language;
			Title = title;
			AionianEdition = aionianEdition;
			Books = books;
		}
		/// <summary>
		/// Indexer to return the verse(as a string) given any book,chapter number and verse index
		/// </summary>
		public string this[BibleBook b, byte c, byte v] => Books[b].Chapter[c][v];
		/// <summary>
		/// Indexer to return the verse(as a string) given any book,chapter number and verse index
		/// </summary>
		public string this[BibleReference v] => this[v.Book, v.Chapter, v.Verse];
		/// <summary>
		/// Since the *.noia database identifies every book (irrespective of the bible language) with these short names, this array is used for that purpose while Extracting it.
		/// Contains a list of Short Names of Books. The first index is NULL so as to match the enum BibleBook index and make its value interconvertible to this array's index
		///
		/// This way, we can get short name of a book as
		/// <code>
		///
		/// string shortname = ShortBookNames[(byte)BibleBook.Leviticus]
		///
		/// </code>
		/// </summary>
		/// <value></value>
		public static readonly string[] ShortBookNames =
		{
			"",
			"GEN","EXO","LEV","NUM","DEU","JOS","JDG","RUT","1SA","2SA","1KI",
			"2KI","1CH","2CH","EZR","NEH","EST","JOB","PSA","PRO","ECC","SOL",
			"ISA","JER","LAM","EZE","DAN","HOS","JOE","AMO","OBA","JON","MIC",
			"NAH","HAB","ZEP","HAG","ZEC","MAL","MAT","MAR","LUK","JOH","ACT",
			"ROM","1CO","2CO","GAL","EPH","PHI","COL","1TH","2TH","1TI","2TI",
			"TIT","PHM","HEB","JAM","1PE","2PE","1JO","2JO","3JO","JUD","REV"
		};
		/// <summary>
		/// Creates a bible from the inputted stream of a Aionian bible noia Database
		/// </summary>
		/// <param name="stream">The Stream to the *.noia Database</param>
		/// <returns>The initiated Bible from the stream is returned</returns>
		public static Bible ExtractBible(StreamReader stream)
		{
			string line;
			byte CurrentChapter = 255;
			BibleBook CurrentBook = BibleBook.NULL;
			Dictionary<byte, Dictionary<byte, string>> CurrentBookData = null;
			Dictionary<byte, string> CurrentChapterData = null;
			line = stream.ReadLine();//Read the first line containing file name
			bool aionianEdition = line.EndsWith("Aionian-Edition.noia");
			string[] tl = line.Replace("---", "|").Split('|');
			string language = tl[1];
			string title = tl[2];
			Dictionary<BibleBook, string> RegionalName = new Dictionary<BibleBook, string>();
			while ((line = stream.ReadLine())[0] == '#')
			{
				if (line.Contains("# Books:"))
				{
					string[] rn = line.Split('\t');
					for (byte it = 1; it <= 66; it++) RegionalName.Add((BibleBook)it, rn[it]);
				}
			}
			Dictionary<BibleBook, Book> books = new Dictionary<BibleBook, Book>();
			while ((line = stream.ReadLine()) != null)
			{
				if (line[0] == '0')//The valid lines of the database begin with 0, not with # or INDEX (header row)
				{
					string[] rows = line.Split('\t');//Returns the line after splitting into multiple rows
					BibleBook book = (BibleBook)(byte)Array.IndexOf(ShortBookNames, rows[1]);//Get the BibleBook from BookName
					byte chapter = byte.Parse(rows[2]);//Get the Chapter number
					byte verseno = byte.Parse(rows[3]);//Get the Verse number
					string verse = rows[4];//Get the verse content
					if (book != CurrentBook)
					{
						if (CurrentBookData != null)
						{
							CurrentBookData[CurrentChapter] = CurrentChapterData;
							books[CurrentBook] = new Book((byte)CurrentBook, CurrentBookData, RegionalName[CurrentBook]);
						}
						CurrentBookData = new Dictionary<byte, Dictionary<byte, string>>();
						CurrentBook = book;
						CurrentChapterData = new Dictionary<byte, string>();
						CurrentChapter = chapter;
					}
					else if (chapter != CurrentChapter)
					{
						CurrentBookData[CurrentChapter] = CurrentChapterData;
						CurrentChapterData = new Dictionary<byte, string>();
						CurrentChapter = chapter;
					}
					CurrentChapterData[verseno] = verse;
				}
			}
			CurrentBookData[CurrentChapter] = CurrentChapterData;
			books[CurrentBook] = new Book((byte)CurrentBook, CurrentBookData, RegionalName[CurrentBook]);
			return new Bible(title, language, aionianEdition, books);
		}
	}
}