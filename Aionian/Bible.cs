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
	private enum ReadProgress : byte { StartKeyValComments, BookStart, VerseRead, EndStream };
	private enum InputType : byte { Comments, BookStartComment, VerseContent, EndOfFile };
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
		ReadProgress mode = ReadProgress.StartKeyValComments;
		InputType inputType;
		void process(string[] rows)
		{
			byte chap;
			switch (mode)
			{
				case ReadProgress.StartKeyValComments:
					if (inputType == InputType.Comments) break;
					else if (inputType == InputType.BookStartComment)
					{
						mode = ReadProgress.BookStart;
						RegionalName.Add(CurrentBook = (BibleBook)byte.Parse(rows[1]), rows[4]);
						break;
					}
					else throw new InvalidDataException("Expected StartBookComment token. Invalid data format!");
				case ReadProgress.BookStart:
					if (inputType == InputType.VerseContent)
					{
						mode = ReadProgress.VerseRead;
						goto case ReadProgress.VerseRead;
					}
					else if (inputType == InputType.Comments) break;
					else throw new InvalidDataException("Expected Verse token. Invalid data format!");
				case ReadProgress.VerseRead:
					if (inputType == InputType.BookStartComment)
					{
						CurrentBookData[CurrentChapter] = CurrentChapterData;
						books[CurrentBook] = new((byte)CurrentBook, CurrentBookData, RegionalName[CurrentBook]);
						RegionalName.Add(CurrentBook = (BibleBook)byte.Parse(rows[1]), rows[4]);
						(CurrentBookData, CurrentChapterData, CurrentChapter) = (new(), new(), 1);
						mode = ReadProgress.BookStart;
						break;
					}
					else if (inputType == InputType.EndOfFile)
					{
						mode = ReadProgress.EndStream;
						goto case ReadProgress.EndStream;
					}
					else if (inputType == InputType.Comments) break;
					else if ((chap = byte.Parse(rows[2])) != CurrentChapter)
					{
						CurrentBookData[CurrentChapter] = CurrentChapterData;
						(CurrentChapterData, CurrentChapter) = (new(), chap);
					}
					CurrentChapterData[byte.Parse(rows[3])] = rows[4];
					break;
				case ReadProgress.EndStream:
					if (inputType != InputType.EndOfFile) throw new InvalidDataException("Recived some data after EOF!");
					break;
			}
		}
		do
		{
			if ((line = stream.ReadLine()?.Trim()) != null)
			{
				string[] rows = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
				inputType = line.StartsWith('0') ? InputType.VerseContent
					: line.StartsWith("# BOOK") ? InputType.BookStartComment
					: InputType.Comments;
				process(rows);
			}
			else
			{
				inputType = InputType.EndOfFile;
				process(Array.Empty<string>());
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