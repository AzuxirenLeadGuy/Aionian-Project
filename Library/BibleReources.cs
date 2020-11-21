using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System;
namespace Aionian
{
    /// <summary>
    /// Aionian Bible Descriptor. 
    /// Provides the description/url for the bible resources provided by Aionian
    /// </summary>
    public static class ABD
    {
		/// <summary>
		/// Contains a list of Short Names of Books. The first index is NULL so as to match the enum BibleBook index and make its value interconvertible to this array's index
		/// 
		/// This way, we can get short name of a book as 
		/// 
		/// ---------------------------------------------
		/// 
		/// 
		/// string shortname = ShortBookNames[(byte)BibleBook.Leviticus]
		/// 
		/// 
		/// -------------------------------------------- 
		/// </summary>
		/// <value></value>
		public static readonly string[] ShortBookNames=
		{
			"",
			"GEN","EXO","LEV","NUM","DEU","JOS","JDG","RUT","1SA","2SA","1KI",
			"2KI","1CH","2CH","EZR","NEH","EST","JOB","PSA","PRO","ECC","SOL",
			"ISA","JER","LAM","EZE","DAN","HOS","JOE","AMO","OBA","JON","MIC",
			"NAH","HAB","ZEP","HAG","ZEC","MAL","MAT","MAR","LUK","JOH","ACT",
			"ROM","1CO","2CO","GAL","EPH","PHI","COL","1TH","2TH","1TI","2TI",
			"TIT","PHM","HEB","JAM","1PE","2PE","1JO","2JO","3JO","JUD","REV"
		};
        /// <summary>
        /// Returns the URL for downloads of Aionian bible for the given parameters. It is advised not to use this unless fully understood of the functionality. 
		/// 
		/// It returns the URL such as http://resources.aionianbible.org/Holy-Bible---[lang]---[title]---Aionian-Edition.noia,
		/// 
		/// but does not ensure that placing the lang and title in the URL makes a valid existing URL or not
        /// </summary>
        /// <param name="lang">The language of the bible</param>
        /// <param name="title">The title of the bible</param>
        /// <param name="AionianEdition">If true, the link is of Aionian edition. Otherwise, it is the standard edition</param>
        /// <returns>Returns the URL string of the Aionian bible database file (May or may not exist)</returns>
        public static string GetBibleURL(string lang, string title, bool AionianEdition = true) => AionianEdition ? $"http://resources.aionianbible.org/Holy-Bible---[{lang}]---[{title}]---Aionian-Edition.noia" : $"http://resources.aionianbible.org/Holy-Bible---[{lang}]---[{title}]---Standard-Edition.noia";
        /// <summary>
        /// Returns a touple of all links available for download. Needless to say, this function requres internet
        /// </summary>
        /// <returns>Returs an array of every link avaialble to download in the Aionian</returns>
        public static BibleLink[] GetAllUrls()
        {
            string ResourceSite = @"http://resources.aionianbible.org/";//Address of the page where we are searching the links
            var responsestring = new StreamReader(WebRequest.Create(ResourceSite).GetResponse().GetResponseStream()).ReadToEnd();//The page containing a lot of things, but we are focused on the links
            Regex regex = new Regex(@"Holy-Bible---([a-zA-Z-]*)---([a-zA-Z-]*)---(Aionian|Standard)-Edition\.noia");//Using REGEX to fetch all links to Aionian/Standard Editions of bible in the page 
			var links=new List<BibleLink>();//A list to store all the links
            foreach (Match match in regex.Matches(responsestring))//For every regex match
            {
				var caps=match.Groups;//Extract the captured groupes
				links.Add(new BibleLink(){Language=caps[1].Value,Title=caps[2].Value,AionianEdition=caps[3].Value=="Aionian",URL=ResourceSite+caps[0].Value});//Add to the list
            }
            return links.ToArray();//Return the array of list
        }
		/// <summary>
		/// Creates a bible from the inputted stream of a Aionian bible noia Database
		/// </summary>
		/// <param name="stream">The Stream to the *.noia Database</param>
		/// <returns>The initiated Bible from the stream is returned</returns>
		public static Bible ExtractBible(StreamReader stream)
		{
			string line;
			var bible=new Bible()
			{
				Books=new Dictionary<BibleBook,Book>()
			};
			byte CurrentChapter=255;
			BibleBook CurrentBook=BibleBook.NULL;
			Dictionary<byte,Dictionary<byte,string>> CurrentBookData=null;
			Dictionary<byte,string> CurrentChapterData=null;
			line=stream.ReadLine();//Read the first line containing file name
			var g=Regex.Match(line,@"Holy-Bible---([a-zA-Z-]*)---([a-zA-Z-]*)---(Aionian|Standard)-Edition\.noia").Groups;
			bible.Language=g[1].Value;
			bible.Title=g[2].Value;
			bible.AionianEdition=g[3].Value=="Aionian";
			while((line=stream.ReadLine())!=null)
			{
				if(line[0]=='0')//The valid lines of the database do not begin with # or INDEX (header row)
				{
					var rows=line.Split('\t');//Returns the line after splitting into multiple rows
					BibleBook book=(BibleBook)(byte)Array.IndexOf(ShortBookNames,rows[1]);//Get the BibleBook from BookName
					var chapter=byte.Parse(rows[2]);//Get the Chapter number
					var verseno=byte.Parse(rows[3]);//Get the Verse number
					var verse=rows[4];//Get the verse content
					if(book!=CurrentBook)
					{
						if(CurrentBookData!=null)bible.Books[CurrentBook]=new Book()
						{
							Chapter=CurrentBookData, 
							ShortBookName=ShortBookNames[(byte)CurrentBook],
							BookIndex=(byte)CurrentBook,
							BookName=Enum.GetName(typeof(BibleBook),book)
						};
						CurrentBookData=new Dictionary<byte, Dictionary<byte, string>>();
						CurrentBook=book;
						CurrentChapterData=new Dictionary<byte, string>();
						CurrentChapter=chapter;
					}
					else if(chapter!=CurrentChapter)
					{
						CurrentBookData[CurrentChapter]=CurrentChapterData;
						CurrentChapterData=new Dictionary<byte, string>();
						CurrentChapter=chapter;
					}
					CurrentChapterData[verseno]=verse;
				}
			}
			return bible;
		}
		/// <summary>
		/// This is an unofficial way to download the stream which can be further used to convert into the Bible Class object.
		/// Needless to say, this is not the necessary way to download and should not be used if it does not meet 
		/// any requiements of the developer.
		/// </summary>
		/// <param name="link">The bible link containing the URL</param>
		/// <returns>The stream of the *.noia file is returned</returns>
		public static StreamReader DownloadStream(this BibleLink link) => new StreamReader(HttpWebRequest.Create(link.URL).GetResponse().GetResponseStream());
    }
	/// <summary>Represents a single link of an Aionian Bible</summary>
	public struct BibleLink
	{
		/// <summary>The Title of the bible</summary>
		public string Title;
		/// <summary>The Language of the bible</summary>
		public string Language;
		/// <summary>The URL of the bible to download</summary>
		public string URL;
		/// <summary>Indicates whether the bible edition is Aionian or not</summary>
		public bool AionianEdition;
	}
}