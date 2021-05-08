using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
namespace Aionian
{
	/// <summary>Represents a single link of an Aionian Bible</summary>
	[Serializable]
	public class BibleLink : IComparable<BibleLink>, IEquatable<BibleLink>
	{
		/// <summary>The Title of the bible</summary>
		[JsonInclude] public readonly string Title;
		/// <summary>The Language of the bible</summary>
		[JsonInclude] public readonly string Language;
		/// <summary>The URL of the bible to download</summary>
		[JsonInclude] public readonly string URL;
		/// <summary>Indicates whether the bible edition is Aionian or not</summary>
		[JsonInclude] public readonly bool AionianEdition;
		/// <summary>Constructor for the BibleLink type</summary>
		public BibleLink(string title, string language, string url, bool aionianEdition)
		{
			Title = title;
			Language = language;
			URL = url;
			AionianEdition = aionianEdition;
		}
		/// <summary>
		/// Compares the other link with this link in terms of precedence in a sorted array
		/// </summary>
		/// <param name="other">The link to compare with</param>
		/// <returns>A value denoting the precedence</returns>
		public virtual int CompareTo(BibleLink other) => Title.CompareTo(other.Title);
		/// <summary>
		/// Compares the other link with this link in terms of equality
		/// </summary>
		/// <param name="other">The link to compare with</param>
		/// <returns>returns true if equal, otherwise false</returns>
		public virtual bool Equals(BibleLink other) => URL == other.URL;
		/// <summary>
		/// Returns the equality of objects
		/// </summary>
		/// <param name="obj">The object to compare</param>
		/// <returns>returns true if equal, otherwise false</returns>
		public override bool Equals(object obj) => obj is BibleLink x && Equals(x);
		/// <summary>
		/// Returns Hash Code
		/// </summary>
		/// <returns>Returns Hash Code</returns>
		public override int GetHashCode() => URL.GetHashCode();
		/// <summary>
		/// Returns the string represntation of this object
		/// </summary>
		/// <returns>Returns the URL</returns>
		public override string ToString() => $"{Language}|{Title}";
		/// <summary>
		/// Returns a touple of all links available for download. Needless to say, this function requres internet
		/// </summary>
		/// <returns>Returs an array of every link avaialble to download in the Aionian</returns>
		public static BibleLink[] GetAllUrlsFromWebsite()
		{
			string ResourceSite = @"http://resources.aionianbible.org/";//Address of the page where we are searching the links
			string responsestring;
			using (StreamReader sr = new StreamReader(WebRequest.Create(ResourceSite).GetResponse().GetResponseStream())) responsestring = sr.ReadToEnd();//The page containing a lot of things, but we are focused on the links
			Regex regex = new Regex(@"Holy-Bible---([a-zA-Z-]*)---([a-zA-Z-]*)---(Aionian|Standard)-Edition\.noia");//Using REGEX to fetch all links to Aionian/Standard Editions of bible in the page 
			HashSet<BibleLink> links = new HashSet<BibleLink>();//A list to store all the links
			foreach (Match match in regex.Matches(responsestring))//For every regex match
			{
				GroupCollection caps = match.Groups;//Extract the captured groupes
				_ = links.Add
				(
					new BibleLink(caps[2].Value, caps[1].Value, (ResourceSite + caps[0].Value).Trim(), caps[3].Value == "Aionian")
				);//Add to the list
			}
			return links.ToArray();//Return the array of list
		}
		/// <summary>
		/// This is an unofficial way to download the stream which can be further used to convert into the Bible Class object.
		/// Needless to say, this is not the necessary way to download and should not be used if it does not meet 
		/// any requiements of the developer.
		/// </summary>
		/// <returns>The stream of the *.noia file is returned</returns>
		public virtual StreamReader DownloadStream() => new StreamReader(WebRequest.Create(URL).GetResponse().GetResponseStream());
		/// <summary>
		/// Preview method for downloading and keeping track of progress of download
		/// </summary>
		/// <param name="progressChanged">The event called when the download progress is changed</param>
		/// <param name="downloadDataCompleted">The event called when the download is completed</param>
		/// <returns>Returns the FileStream of the .noia database of Bible</returns>
		public virtual async Task<StreamReader> DownloadStreamAsync(DownloadProgressChangedEventHandler progressChanged = null, DownloadDataCompletedEventHandler downloadDataCompleted = null)
		{
			WebClient client = new WebClient();
			if (progressChanged != null) client.DownloadProgressChanged += progressChanged;
			if (downloadDataCompleted != null) client.DownloadDataCompleted += downloadDataCompleted;
			string result = await client.DownloadStringTaskAsync(new Uri(URL));
			return new StreamReader(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(result)));
		}
	}
}