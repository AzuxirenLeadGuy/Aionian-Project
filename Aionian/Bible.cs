using System;
using System.Collections.Generic;
using System.IO;
namespace Aionian;
/// <summary> This denotes a Bible of a specific Language and Version/Title</summary>
public abstract class Bible
{
	/// <summary> Describes the bible </summary>
	public readonly BibleDescriptor Descriptor;
	/// <summary> Initializes the bible </summary>
	protected Bible(BibleDescriptor desc) => Descriptor = desc;
	/// <summary> Gets the books of this bible </summary>
	public abstract Book FetchBook(BibleBook book);
	/// <summary>
	/// Indexer to return the verse(as a string) given any book,chapter number and verse index
	/// </summary>
	public string this[BibleBook b, byte c, byte v] => FetchBook(b).Chapter[c][v];
	/// <summary>
	/// Indexer to return the verse(as a string) given any book,chapter number and verse index
	/// </summary>
	public string this[BibleReference v] => this[v.Book, v.Chapter, v.Verse];
	/// <summary>
	/// <para>
	/// Since the *.noia database identifies every book (irrespective of the bible language) with these short names, this array is used for that purpose while Extracting it.
	/// Contains a list of Short Names of Books. The first index is NULL so as to match the enum BibleBook index and make its value interconvertible to this array's index
	/// </para>
	/// <para>This way, we can get short name of a book as</para>
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
	private enum ReadProgress : byte { StartBook, ReadVerse, StopChapter, StopBook, Comments };
	/// <summary>
	/// Creates a bible from the inputted stream of a Aionian bible noia Database
	/// </summary>
	/// <param name="stream">The Stream to the *.noia Database</param>
	/// <returns>The initiated Bible from the stream is returned</returns>
	public static (BibleDescriptor descriptor, Dictionary<BibleBook, Book> books) ExtractBible(StreamReader stream)
	{
		string? line;
		byte CurrentChapter = 1;
		BibleBook CurrentBook = BibleBook.NULL;
		Dictionary<byte, Dictionary<byte, string>> CurrentBookData = new();
		Dictionary<byte, string> CurrentChapterData = new();
		line = stream.ReadLine();//Read the first line containing file name
		if (line == null) throw new ArgumentException("Unable to parse data from stream!");
		bool aionianEdition = line.EndsWith("Aionian-Edition.noia");
		string[] tl = line.Replace("---", "|").Split('|');
		(string language, string title) = (tl[1], tl[2]);
		Dictionary<BibleBook, string> RegionalName = new();
		Dictionary<BibleBook, Book> books = new();
		ReadProgress mode;
		void process(string[] rows)
		{
			switch (mode)
			{
				case ReadProgress.StartBook:
					CurrentBook = (BibleBook)byte.Parse(rows[1]);
					if (CurrentBook!=BibleBook.NULL)
						RegionalName.Add(CurrentBook = (BibleBook)byte.Parse(rows[1]), rows[4]);
					break;
				case ReadProgress.ReadVerse:
					byte verseno = byte.Parse(rows[3]);//Get the Verse number
					string verse = rows[4];//Get the verse content
					CurrentChapterData[verseno] = verse;
					break;
				case ReadProgress.StopChapter:
					var chapter = byte.Parse(rows[2]);//Get the Chapter number
					CurrentBookData[CurrentChapter] = CurrentChapterData;
					CurrentChapterData = new();
					CurrentChapter = chapter;
					goto case ReadProgress.ReadVerse;
				case ReadProgress.StopBook:
					CurrentBookData[CurrentChapter] = CurrentChapterData;
					CurrentChapterData = new();
					books[CurrentBook] = new Book((byte)CurrentBook, CurrentBookData, RegionalName[CurrentBook]);
					CurrentBookData = new();
					CurrentChapter = 1;
					goto case ReadProgress.StartBook;
				case ReadProgress.Comments:
					break;
				default:
					break;
			}
		}
		string[] dummy = new string[] { "0", "0", "0", "0", "0", "0" };
		do
		{
			if ((line = stream.ReadLine()?.Trim()) != null)
			{
				string[] rows = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
				if (line.StartsWith('0'))
				{
					var chap = byte.Parse(rows[2]);//Get the Chapter number
					mode = chap == CurrentChapter ? ReadProgress.ReadVerse : ReadProgress.StopChapter;
				}
				else if (line.StartsWith("# BOOK"))
					mode = CurrentBook == BibleBook.NULL ? ReadProgress.StartBook : ReadProgress.StopBook;
				else mode = ReadProgress.Comments;
				process(rows);
			}
			else
			{
				mode = ReadProgress.StopBook;
				process(dummy);
				break;
			}
		}
		while (true);
		return
		(
			new BibleDescriptor()
			{
				Title = title,
				Language = language,
				AionianEdition = aionianEdition,
				RegionalName = RegionalName,
			},
			books
		);
	}
}