using System;

namespace Aionian;

/// <summary>
/// Denotes a single reference
/// </summary>
public struct BibleReference : IEquatable<BibleReference>, IComparable<BibleReference>, IEquatable<BibleRegion>
{
	/// <summary>The Book of this reference</summary>
	public BibleBook Book;
	/// <summary>The Chapter number of this reference</summary>
	public byte Chapter;
	/// <summary>The Verse number of this reference</summary>
	public byte Verse;
	/// <summary>
	/// Denotes which of the two verses should be ahead/behind of each other
	/// </summary>
	/// <param name="other">The other reference to compare</param>
	/// <returns>int value indicating the order</returns>
	public readonly int CompareTo(BibleReference other)
	{
		if (Book != other.Book) return ((byte)Book).CompareTo((byte)other.Book);
		else if (Chapter != other.Chapter) return Chapter.CompareTo(other.Chapter);
		else return Verse.CompareTo(other.Verse);
	}
	/// <summary>
	/// Denotes if the queried other reference is equivalent to this one
	/// </summary>
	/// <param name="other">The other reference to compare</param>
	/// <returns>Boolean result indicating if both are equal</returns>
	public readonly bool Equals(BibleReference other) => Book == other.Book && Chapter == other.Chapter && Verse == other.Verse;
	/// <summary>
	/// Denotes if the queried other reference is equivalent to this one
	/// </summary>
	/// <param name="obj">The other reference to compare</param>
	/// <returns>Boolean result indicating if both are equal</returns>
	public override readonly bool Equals(object? obj) => obj is BibleReference reference && Equals(reference);
	/// <summary>Compares a BibleRegion to this BibleReference. It is true only if the region consists of only a single verse, which is the other BibleReference</summary>
	/// <param name="other">The BibleRegion to compare</param>
	/// <returns>boolean value indicating that both are equivalent</returns>
	public readonly bool Equals(BibleRegion other) => other.Equals(this);
	/// <summary>Returns HashCode of the object</summary>
	/// <returns>The byte value of Book</returns>
	public override readonly int GetHashCode() => (byte)Book;
	/// <summary>
	/// Returns the string representation of the reference
	/// </summary>
	/// <returns></returns>
	public override readonly string ToString() => $"{Enum.GetName(typeof(BibleBook), Book)} {Chapter} : {Verse}";
	/// <summary>
	/// Denotes if the two refereces are equal to each other or not
	/// </summary>
	/// <param name="a">BibleReference operator</param>
	/// <param name="b">BibleReference operator</param>
	/// <returns>Boolean result indicating if both are equal</returns>
	public static bool operator ==(BibleReference a, BibleReference b) => a.Equals(b);
	/// <summary>
	/// Denotes if the two refereces are not equal to each other or not
	/// </summary>
	/// <param name="a">BibleReference operator</param>
	/// <param name="b">BibleReference operator</param>
	/// <returns>Boolean result indicating if both are not equal</returns>
	public static bool operator !=(BibleReference a, BibleReference b) => !a.Equals(b);
}
