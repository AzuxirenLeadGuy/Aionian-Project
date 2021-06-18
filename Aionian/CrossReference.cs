using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System;

namespace Aionian
{
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
		public int CompareTo(BibleReference other)
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
		public bool Equals(BibleReference other) => Book == other.Book && Chapter == other.Chapter && Verse == other.Verse;
		/// <summary>
		/// Denotes if the queried other reference is equivalent to this one
		/// </summary>
		/// <param name="obj">The other reference to compare</param>
		/// <returns>Boolean result indicating if both are equal</returns>
		public override bool Equals(object obj) => obj is BibleReference reference && Equals(reference);
		/// <summary>Compares a BibleRegion to this BibleReference. It is true only if the region consists of only a single verse, which is the other BibleReference</summary>
		/// <param name="other">The BibleRegion to compare</param>
		/// <returns>boolean value indicating that both are equivalent</returns>
		public bool Equals(BibleRegion other) => other.Equals(this);
		/// <summary>Returns HashCode of the object</summary>
		/// <returns>The byte value of Book</returns>
		public override int GetHashCode() => (byte)Book;
		/// <summary>
		/// Returns the string representation of the reference
		/// </summary>
		/// <returns></returns>
		public override string ToString() => $"{Enum.GetName(typeof(BibleBook), Book)} {Chapter} : {Verse}";
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
	/// <summary>Denotes a continues region/range of verses</summary>
	public struct BibleRegion : IEquatable<BibleRegion>, IEquatable<BibleReference>
	{
		/// <summary>The reference of region start</summary>
		public readonly BibleReference RegionStart;
		/// <summary>The reference of region end</summary>
		public readonly BibleReference RegionEnd;
		/// <summary>
		/// Constructor for regions
		/// </summary>
		/// <param name="start">The start of the region</param>
		/// <param name="end">The end of the region</param>
		public BibleRegion(BibleReference start, BibleReference end)
		{
			RegionStart = start;
			RegionEnd = end;
		}
		/// <summary>Compares a BibleReference to this region. It is true only if the region consists of only a single verse, which is the other BibleReference as parameters</summary>
		/// <param name="other">The BibleReference to compare</param>
		/// <returns>boolean value indicating that both are equivalent</returns>
		public bool Equals(BibleReference other) => RegionStart.Equals(other) && RegionEnd.Equals(other);
		/// <summary>Compares two bible references</summary>
		/// <param name="other">The other bible reference being checked for equality</param>
		/// <returns>boolean value indicating both are equivalent</returns>
		public bool Equals(BibleRegion other) => RegionStart.Equals(other.RegionStart) && RegionEnd.Equals(other.RegionEnd);
		/// <summary>Compares object for equality</summary>
		/// <param name="obj">object to compare</param>
		/// <returns>boolean value indicating equality</returns>
		public override bool Equals(object obj)
		{
			if (obj is BibleReference reference) return Equals(reference);
			else if (obj is BibleRegion bregion) return Equals(bregion);
			else return false;
		}
		/// <summary>Returns the hash code of this object</summary>
		/// <returns>Hash code for the object</returns>
		public override int GetHashCode() => RegionStart.GetHashCode();
		/// <summary>Denotes object as a string</summary>
		/// <returns>String representation of the object</returns>
		public override string ToString() => RegionStart == RegionEnd ? RegionStart.ToString() : $"[{RegionStart} to {RegionEnd} ]";
		/// <summary>Compares two bible references </summary>
		/// <param name="a">left operator</param>
		/// <param name="b">right operator</param>
		/// <returns>boolean value indicating equality</returns>
		public static bool operator ==(BibleRegion a, BibleRegion b) => a.Equals(b);
		/// <summary>Compares two bible references </summary>
		/// <param name="a">left operator</param>
		/// <param name="b">right operator</param>
		/// <returns>boolean value indicating inequality</returns>
		public static bool operator !=(BibleRegion a, BibleRegion b) => !a.Equals(b);
		/// <summary>Compares a BibleReference to this region. It is true only if the region consists of only a single verse, which is the other BibleReference as parameters</summary>
		/// <param name="a">left operator</param>
		/// <param name="b">right operator</param>
		/// <returns>boolean value indicating that both are equivalent</returns>
		public static bool operator ==(BibleRegion a, BibleReference b) => a.Equals(b);
		/// <summary>Compares a BibleReference to this region. It is true only if the region consists of only a single verse, which is the other BibleReference as parameters</summary>
		/// <param name="a">left operator</param>
		/// <param name="b">right operator</param>
		/// <returns>boolean value indicating that both are not equivalent</returns>
		public static bool operator !=(BibleRegion a, BibleReference b) => !a.Equals(b);
		/// <summary>Compares a BibleReference to this region. It is true only if the region consists of only a single verse, which is the other BibleReference as parameters</summary>
		/// <param name="a">left operator</param>
		/// <param name="b">right operator</param>
		/// <returns>boolean value indicating that both are equivalent</returns>
		public static bool operator ==(BibleReference a, BibleRegion b) => b.Equals(a);
		/// <summary>Compares a BibleReference to this region. It is true only if the region consists of only a single verse, which is the other BibleReference as parameters</summary>
		/// <param name="a">left operator</param>
		/// <param name="b">right operator</param>
		/// <returns>boolean value indicating that both are not equivalent</returns>
		public static bool operator !=(BibleReference a, BibleRegion b) => !b.Equals(a);
	}
	/// <summary>
	/// Denotes a single cross-reference link
	/// </summary>
	public struct CrossReference : IComparable<CrossReference>, IEquatable<CrossReference>
	{
		/// <summary>The reference to view cross-references of</summary>
		public readonly BibleReference Source;
		/// <summary>The cross-references of the queried reference</summary>
		public readonly BibleRegion[] Destination;
		/// <summary>
		/// The constructor for CrossReference type
		/// </summary>
		/// <param name="source">The reference queried for cross reference</param>
		/// <param name="destination">All regions of cross references linked to the source</param>
		public CrossReference(BibleReference source, BibleRegion[] destination)
		{
			Source = source;
			Destination = destination;
		}
		/// <summary>Compares two Cross-references</summary>
		/// <param name="other">The other cross-reference to compare</param>
		/// <returns>int value indicating order</returns>
		public int CompareTo(CrossReference other) => Source.CompareTo(other.Source);
		/// <summary>Compares two Cross-references</summary>
		/// <param name="other">The other cross-reference to compare</param>
		/// <returns>boolean value indicating equality</returns>
		public bool Equals(CrossReference other) => Source.Equals(other.Source) && Destination.Equals(other.Destination);
	}
	/// <summary>
	/// The database of all the cross-references in a bible, taken from OpenBible.info
	/// </summary>
	[Obsolete("This class is still in development. Right now it only exists as a proof of concept.")]
	public class CrossReferenceDatabase
	{
		/// <summary>
		/// Contains all crossreference as a Dictionary
		/// </summary>
		public SortedSet<CrossReference> AllCrossReferences;
		internal readonly string[] BookNames =
		{
			"", "Gen", "Exod", "Lev", "Num", "Deut", "Josh", "Judg", "Ruth", "1Sam", "2Sam", "1Kgs", "2Kgs", "1Chr", "2Chr", "Ezra", "Neh", "Esth", "Job", "Ps", "Prov", "Eccl", "Song", "Isa", "Jer", "Lam", "Ezek", "Dan", "Hos", "Joel", "Amos", "Obad", "Jonah", "Mic", "Nah", "Hab", "Zeph", "Hag", "Zech", "Mal", "Matt", "Mark", "Luke", "John", "Acts", "Rom", "1Cor", "2Cor", "Gal", "Eph", "Phil", "Col", "1Thess", "2Thess", "1Tim", "2Tim", "Titus", "Phlm", "Heb", "Jas", "1Pet", "2Pet", "1John", "2John", "3John", "Jude", "Rev"
		};
		/// <summary>
		/// Reads text file containing all CRs
		/// </summary>
		/// <param name="treshold">The minimum number of votes to be considered added in list</param>
		public void ReadFromFile(int treshold)
		{
			using (StreamReader stream = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Aionian.cross_references.cross_references.txt")))
			{
				AllCrossReferences = new SortedSet<CrossReference>();
				_ = stream.ReadLine();//Ignore the first header line
				List<BibleRegion> AllVerses = new List<BibleRegion>();
				BibleReference prevSource = new BibleReference() { Book = BibleBook.NULL, Chapter = 0, Verse = 0 };
				while (!stream.EndOfStream)
				{
					string[] row = stream.ReadLine().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
					if (int.Parse(row[2]) >= treshold)
					{
						BibleReference currentSource = ParseReference(row[0]);
						if (currentSource != prevSource)
						{
							if (AllVerses.Count > 0)
							{
								_ = AllCrossReferences.Add(new CrossReference(prevSource, AllVerses.ToArray()));
								AllVerses.Clear();
							}
							prevSource = currentSource;
						}
						if (row[1].Contains("-"))
						{
							string[] region = row[1].Split('-');
							AllVerses.Add(new BibleRegion(ParseReference(region[0]), ParseReference(region[1])));
						}
						else
						{
							BibleReference referece = ParseReference(row[1]);
							AllVerses.Add(new BibleRegion(referece, referece));
						}
					}
				}
				_ = AllCrossReferences.Add(new CrossReference(prevSource, AllVerses.ToArray()));
			}
			BibleReference ParseReference(string s)
			{
				string[] parts = s.Split('.');
				BibleBook bk = (BibleBook)(byte)Array.FindIndex(BookNames, x => x == parts[0]);
				return new BibleReference() { Book = bk, Chapter = byte.Parse(parts[1]), Verse = byte.Parse(parts[2]) };
			}
		}
	}
}