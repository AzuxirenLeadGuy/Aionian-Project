using System;
using System.Collections.Generic;
namespace Aionian;
/// <summary> The Language of the bible </summary>
[Serializable]
public readonly record struct Book
{
	/// <summary> The Index of the book (Starts from 1) </summary>
	public readonly byte BookIndex { get; init; }
	/// <summary> The collection of chapters in this book </summary>
	public readonly Dictionary<byte, Dictionary<byte, string>> Chapter { get; init; }
	/// <summary>The name of the book in the language of the region</summary>
	public readonly string RegionalBookName { get; init; }
	/// <summary> The Constructor for this type</summary>
	public Book(byte bookindex, Dictionary<byte, Dictionary<byte, string>> chapter, string regionalBookName)
	{
		BookIndex = bookindex;
		Chapter = chapter;
		RegionalBookName = regionalBookName;
	}
	/// <summary> Returns the current BibleBook enum of this chapter </summary>
	public BibleBook CurrentBibleBook => (BibleBook)BookIndex;
	/// <summary> The name of the book in English</summary>
	public string BookName => Enum.GetName(typeof(BibleBook), (BibleBook)BookIndex) ?? throw new ArgumentException("Enum cannot be parsed!");
	/// <summary> The Short name of the book </summary>
	public string ShortBookName => Bible.ShortBookNames[BookIndex];
}