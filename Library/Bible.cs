using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Aionian
{
	/// <summary> This denotes a Bible of a specific Language and Version/Title</summary>
	[Serializable]
	public struct Bible
	{
		/// <summary> The Language of the bible </summary>
		[JsonInclude] public string Language;
		/// <summary> The Specific bible </summary>
		[JsonInclude] public string Title;
		/// <summary> Indicates whether this Bible is Aionian or Standard edition </summary>
		[JsonInclude] public bool AionianEdition;
		/// <summary> The Collection of books </summary>
		[JsonInclude] public IDictionary<BibleBook, Book> Books;
		/// <summary>
		/// Indexer to return the verse(as a string) given any book,chapter number and verse index
		/// </summary>
		public string this[BibleBook b, byte c, byte v] => Books[b].Chapter[c][v];
		/// <summary>
		/// Indexer to return the verse(as a string) given any book,chapter number and verse index
		/// </summary>
		public string this[(BibleBook, byte, byte) v] => this[v.Item1, v.Item2, v.Item3];
		/// <summary>
		/// Contains a list of Short Names of Books. The first index is NULL so as to match the enum BibleBook index and make its value interconvertible to this array's index
		/// 
		/// This way, we can get short name of a book as 
		/// 
		/// ---------------------------------------------
		/// 
		/// 
		/// string shortname = ShortBookNames[(byte)BibleBook.Leviticus]
		/// 
		/// 
		/// -------------------------------------------- 
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
			Bible bible = new Bible()
			{
				Books = new Dictionary<BibleBook, Book>()
			};
			byte CurrentChapter = 255;
			BibleBook CurrentBook = BibleBook.NULL;
			Dictionary<byte, Dictionary<byte, string>> CurrentBookData = null;
			Dictionary<byte, string> CurrentChapterData = null;
			line = stream.ReadLine();//Read the first line containing file name
			GroupCollection g = Regex.Match(line, @"Holy-Bible---([a-zA-Z-]*)---([a-zA-Z-]*)---(Aionian|Standard)-Edition\.noia").Groups;
			bible.Language = g[1].Value;
			bible.Title = g[2].Value;
			bible.AionianEdition = g[3].Value == "Aionian";
			while ((line = stream.ReadLine()) != null)
			{
				if (line[0] == '0')//The valid lines of the database do not begin with # or INDEX (header row)
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
							bible.Books[CurrentBook] = new Book()
							{
								Chapter = CurrentBookData,
								ShortBookName = ShortBookNames[(byte)CurrentBook],
								BookIndex = (byte)CurrentBook,
								BookName = Enum.GetName(typeof(BibleBook), book - 1)
							};
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
			bible.Books[CurrentBook] = new Book()
			{
				Chapter = CurrentBookData,
				ShortBookName = ShortBookNames[(byte)CurrentBook],
				BookIndex = (byte)CurrentBook,
				BookName = Enum.GetName(typeof(BibleBook), BibleBook.Reveleation)
			};
			return bible;
		}
	}
	/// <summary> The Language of the bible </summary>
	[Serializable]
	public struct Book
	{
		/// <summary> The Index of the book (Starts from 1) </summary>
		[JsonInclude] public byte BookIndex;
		/// <summary> The Full name of the book </summary>
		[JsonInclude] public string BookName;
		/// <summary> The Short name of the book </summary>
		[JsonInclude] public string ShortBookName;
		/// <summary> The collection of chapters in this book </summary>
		[JsonInclude] public Dictionary<byte, Dictionary<byte, string>> Chapter;
		/// <summary> Returns the current BibleBook enum of this chapter </summary>
		public BibleBook CurrentBibleBook => (BibleBook)BookIndex;
	}
}