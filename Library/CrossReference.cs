namespace Aionian
{
	/// <summary>
	/// Denotes a single cross-reference link
	/// </summary>
	public struct CrossReference
	{
		/// <summary>The reference to view cross-references of</summary>
		public readonly (BibleBook Book, byte Chapter, byte Verse) Source;
		/// <summary>The cross-references of the queried reference</summary>
		public readonly (BibleBook Book, byte Chapter, byte Verse)[] Destination;
		/// <summary>The constructor of the Cross reference</summary>
		public CrossReference((BibleBook Book, byte Chapter, byte Verse) source, (BibleBook Book, byte Chapter, byte Verse)[] dest)
		{
			Source = source;
			Destination = dest;
		}
	}
	/// <summary>
	/// The database of all the cross-references in a bible
	/// </summary>
	public class CrossReferenceDatabase
	{
	}
}