using System;
using System.Collections.Generic;
namespace Aionian;
/// <summary> Describes a Bible object </summary>
[Serializable]
public readonly record struct BibleDescriptor
{
	/// <summary> Title of the Bible </summary>
	public readonly string Title { init; get; }
	/// <summary> Language of the Bible </summary>
	public readonly string Language { init; get; }
	/// <summary> Denotes whether or not this Bible is Aionian edition or the standard edition </summary>
	public readonly bool AionianEdition { init; get; }
	/// <summary> Regional names of the books of this Bible </summary>
	public readonly Dictionary<BibleBook, string> RegionalName { init; get; }
}