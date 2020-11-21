using System.Collections.Generic;

namespace Aionian
{
    /// <summary> This denotes a Bible of a specific Language and Version/Title</summary>
    public struct Bible 
	{
		/// <summary> The Language of the bible </summary>
		public string Language;
		/// <summary> The Specific bible </summary>
		public string Title;
		/// <summary> Indicates whether this Bible is Aionian or Standard edition </summary>
		public bool AionianEdition;
		/// <summary> The Collection of books </summary>
		public IDictionary<BibleBook,Book>  Books;
		/// <summary>
		/// Indexer to return the verse(as a string) given any book,chapter number and verse index
		/// </summary>
		public string this[BibleBook b,byte c,byte v]=>Books[b].Chapter[c][v];
		/// <summary>
		/// Indexer to return the verse(as a string) given any book,chapter number and verse index
		/// </summary>
		public string this[(BibleBook,byte,byte) v]=>this[v.Item1,v.Item2,v.Item3];
	}
	/// <summary> The Language of the bible </summary>
	public struct Book
	{
		/// <summary> The Index of the book (Starts from 1) </summary>
		public byte BookIndex;
		/// <summary> The Full name of the book </summary>
		public string BookName;
		/// <summary> The Short name of the book </summary>
		public string ShortBookName;
		/// <summary> The collection of chapters in this book </summary>
		public Dictionary<byte,Dictionary<byte,string>> Chapter;
		/// <summary> Returns the current BibleBook enum of this chapter </summary>
		public BibleBook CurrentBibleBook=>(BibleBook)BookIndex;
	}
}
