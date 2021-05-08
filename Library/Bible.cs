using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
namespace Aionian
{
	/// <summary> This denotes a Bible of a specific Language and Version/Title</summary>
	[Serializable]
	public class Bible
	{
		/// <summary> The Language of the bible </summary>
		[JsonInclude] public readonly string Language;
		/// <summary> The Specific bible </summary>
		[JsonInclude] public readonly string Title;
		/// <summary> Indicates whether this Bible is Aionian or Standard edition </summary>
		[JsonInclude] public readonly bool AionianEdition;
		/// <summary> The Collection of books </summary>
		[JsonInclude] public readonly Dictionary<BibleBook, Book> Books;
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
		public string this[(BibleBook book, byte chapter, byte verse) v] => this[v.book, v.chapter, v.verse];
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
			byte CurrentChapter = 255;
			BibleBook CurrentBook = BibleBook.NULL;
			Dictionary<byte, Dictionary<byte, string>> CurrentBookData = null;
			Dictionary<byte, string> CurrentChapterData = null;
			line = stream.ReadLine();//Read the first line containing file name
			GroupCollection g = Regex.Match(line, @"Holy-Bible---([a-zA-Z-]*)---([a-zA-Z-]*)---(Aionian|Standard)-Edition\.noia").Groups;
			string language = g[1].Value;
			string title = g[2].Value;
			bool aionianEdition = g[3].Value == "Aionian";
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
							books[CurrentBook] = new Book((byte)CurrentBook, CurrentBookData);
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
			books[CurrentBook] = new Book((byte)CurrentBook, CurrentBookData);
			return new Bible(title, language, aionianEdition, books);
		}
	}
	/// <summary> The Language of the bible </summary>
	[Serializable]
	public class Book
	{
		/// <summary> The Index of the book (Starts from 1) </summary>
		[JsonInclude] public readonly byte BookIndex;
		/// <summary> The collection of chapters in this book </summary>
		[JsonInclude] public readonly Dictionary<byte, Dictionary<byte, string>> Chapter;
		/// <summary> The Constructor for this type</summary>
		public Book(byte bookindex, Dictionary<byte, Dictionary<byte, string>> chapter)
		{
			BookIndex = bookindex;
			Chapter = chapter;
		}
		/// <summary> Returns the current BibleBook enum of this chapter </summary>
		public BibleBook CurrentBibleBook => (BibleBook)BookIndex;
		/// <summary> The Full name of the book </summary>
		public string BookName => Enum.GetName(typeof(BibleBook), (BibleBook)BookIndex);
		/// <summary> The Short name of the book </summary>
		public string ShortBookName => Bible.ShortBookNames[BookIndex];
	}
}