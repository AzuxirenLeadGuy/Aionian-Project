using System;
using System.Collections.Generic;
namespace Aionian;
/// <summary> Describes a Bible object </summary>
[Serializable]
public readonly record struct BibleDescriptor : IEquatable<BibleLink>
{
	/// <summary> Title of the Bible </summary>
	public readonly string Title { init; get; }
	/// <summary> Language of the Bible </summary>
	public readonly string Language { init; get; }
	/// <summary> Denotes whether or not this Bible is Aionian edition or the standard edition </summary>
	public readonly bool AionianEdition { init; get; }
	/// <summary> Regional names of the books of this Bible </summary>
	public readonly Dictionary<BibleBook, string> RegionalName { init; get; }
	/// <summary> String representation for this Bible </summary>
	public override string ToString() => $"{Language} Bible : {Title}";
	/// <summary>Compares if an offline Description is equal to a download link</summary>
	/// <param name="other">The download link to compare</param>
	/// <returns>Returns true if both are equal; otherwise false</returns>
	public bool Equals(BibleLink other) => Title.Equals(other.Title) && Language.Equals(other.Language) && AionianEdition.Equals(other.AionianEdition);
	/// <summary> Null/0 Representation </summary>
	public static BibleDescriptor Empty => new()
	{
		Title = "",
		Language = "",
		AionianEdition = false
	};
}