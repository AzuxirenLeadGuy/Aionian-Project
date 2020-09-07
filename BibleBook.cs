namespace Aionian
{
	
    /// <summary>
    /// This is a value denoting all the books in the Bible. 
	/// 
	/// Start with NULL=0, so the value of each book is also it's index in Bible. 
	/// For example Genesis=1 and it is the 1st book, Exodus=2,...and so on.
	/// 
	/// 
	/// C# has the Enum class which can return the string of each value, 
	/// as well returning the complete string array of all enum identifiers.
	/// Thus this enum can fulfill many requirements 
    /// </summary>
    public enum BibleBook:byte
	{
		/// <summary> Invalid Result</summary>
		NULL,
		/// <summary>Book of Bible</summary>
		Genesis,
		/// <summary>Book of Bible</summary>
		Exodus,
		/// <summary>Book of Bible</summary>
		Leviticus,
		/// <summary>Book of Bible</summary>
		Numbers,
		/// <summary>Book of Bible</summary>
		Deuteronomy,
		/// <summary>Book of Bible</summary>
		Joshua,
		/// <summary>Book of Bible</summary>
		Judges,
		/// <summary>Book of Bible</summary>
		Ruth,
		/// <summary>Book of Bible</summary>
		I_Samuel,
		/// <summary>Book of Bible</summary>
		II_Samuel,
		/// <summary>Book of Bible</summary>
		I_Kings,
		/// <summary>Book of Bible</summary>
		II_Kings,
		/// <summary>Book of Bible</summary>
		I_Chronicles,
		/// <summary>Book of Bible</summary>
		II_Chronicles,
		/// <summary>Book of Bible</summary>
		Ezra,
		/// <summary>Book of Bible</summary>
		Nehemiah,
		/// <summary>Book of Bible</summary>
		Esther,
		/// <summary>Book of Bible</summary>
		Job,
		/// <summary>Book of Bible</summary>
		Psalms,
		/// <summary>Book of Bible</summary>
		Proverbs,
		/// <summary>Book of Bible</summary>
		Ecclesiastes,
		/// <summary>Book of Bible</summary>
		Song_of_Solomon,
		/// <summary>Book of Bible</summary>
		Isaiah,
		/// <summary>Book of Bible</summary>
		Jeremiah,
		/// <summary>Book of Bible</summary>
		Lamentations,
		/// <summary>Book of Bible</summary>
		Ezekiel,
		/// <summary>Book of Bible</summary>
		Daniel,
		/// <summary>Book of Bible</summary>
		Hosea,
		/// <summary>Book of Bible</summary>
		Joel,
		/// <summary>Book of Bible</summary>
		Amos,
		/// <summary>Book of Bible</summary>
		Obadiah,
		/// <summary>Book of Bible</summary>
		Jonah,
		/// <summary>Book of Bible</summary>
		Micah,
		/// <summary>Book of Bible</summary>
		Nahum,
		/// <summary>Book of Bible</summary>
		Habakkuk,
		/// <summary>Book of Bible</summary>
		Zephaniah,
		/// <summary>Book of Bible</summary>
		Haggai,
		/// <summary>Book of Bible</summary>
		Zecharaiah,
		/// <summary>Book of Bible</summary>
		Malachi,
		/// <summary>Book of Bible</summary>
		Matthew,
		/// <summary>Book of Bible</summary>
		Mark,
		/// <summary>Book of Bible</summary>
		Luke,
		/// <summary>Book of Bible</summary>
		John,
		/// <summary>Book of Bible</summary>
		Acts,
		/// <summary>Book of Bible</summary>
		Romans,
		/// <summary>Book of Bible</summary>
		I_Corinthians,
		/// <summary>Book of Bible</summary>
		II_Corinthians,
		/// <summary>Book of Bible</summary>
		Galatians,
		/// <summary>Book of Bible</summary>
		Ephesians,
		/// <summary>Book of Bible</summary>
		Philippians,
		/// <summary>Book of Bible</summary>
		Colossians,
		/// <summary>Book of Bible</summary>
		I_Thessalonians,
		/// <summary>Book of Bible</summary>
		II_Thessalonians,
		/// <summary>Book of Bible</summary>
		I_Timothy,
		/// <summary>Book of Bible</summary>
		II_Timothy,
		/// <summary>Book of Bible</summary>
		Titus,
		/// <summary>Book of Bible</summary>
		Philemon,
		/// <summary>Book of Bible</summary>
		Hebrews,
		/// <summary>Book of Bible</summary>
		James,
		/// <summary>Book of Bible</summary>
		I_Peter,
		/// <summary>Book of Bible</summary>
		II_Peter,
		/// <summary>Book of Bible</summary>
		I_John,
		/// <summary>Book of Bible</summary>
		II_John,
		/// <summary>Book of Bible</summary>
		III_John,
		/// <summary>Book of Bible</summary>
		Jude,
		/// <summary>Book of Bible</summary>
		Reveleation
	}
}
