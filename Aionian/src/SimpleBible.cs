using System.Collections.Generic;

namespace Aionian;
/// <summary> A simple implementation of the abstract class Bible </summary>
public class SimpleBible : Bible
{
	/// <summary> The books in this bible</summary>
	public readonly Dictionary<BibleBook, Book> Books;
	/// <summary> Initializes a SimpleBible object </summary>
	/// <param name="desc">The description of the bible</param>
	/// <param name="books">The books of the bible</param>
	public SimpleBible(BibleDescriptor desc, Dictionary<BibleBook, Book> books)
	{
		Descriptor = desc;
		Books = books;
	}

	/// <inheritdoc/>
	public override Book FetchBook(BibleBook book) => Books[book];
	/// <inheritdoc/>
	public override IEnumerable<BibleBook> GetBooks() => Books.Keys;
}