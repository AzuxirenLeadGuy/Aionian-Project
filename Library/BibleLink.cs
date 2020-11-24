using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Aionian
{
	/// <summary>Represents a single link of an Aionian Bible</summary>
	[Serializable]
	public struct BibleLink : IComparable<BibleLink>, IEquatable<BibleLink>
	{
		/// <summary>The Title of the bible</summary>
		[JsonInclude] public string Title;
		/// <summary>The Language of the bible</summary>
		[JsonInclude] public string Language;
		/// <summary>The URL of the bible to download</summary>
		[JsonInclude] public string URL;
		/// <summary>Indicates whether the bible edition is Aionian or not</summary>
		[JsonInclude] public bool AionianEdition;
		/// <summary>
		/// Compares the other link with this link in terms of precedence in a sorted array
		/// </summary>
		/// <param name="other">The link to compare with</param>
		/// <returns>A value denoting the precedence</returns>
		public int CompareTo(BibleLink other) => Title.CompareTo(other.Title);
		/// <summary>
		/// Compares the other link with this link in terms of equality
		/// </summary>
		/// <param name="other">The link to compare with</param>
		/// <returns>returns true if equal, otherwise false</returns>
		public bool Equals(BibleLink other) => URL == other.URL;
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
		public override string ToString() => URL;
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
				_ = links.Add(new BibleLink() { Language = caps[1].Value, Title = caps[2].Value, AionianEdition = caps[3].Value == "Aionian", URL = (ResourceSite + caps[0].Value).Trim() });//Add to the list
			}
			return links.ToArray();//Return the array of list
		}
		/// <summary>
		/// This is an unofficial way to download the stream which can be further used to convert into the Bible Class object.
		/// Needless to say, this is not the necessary way to download and should not be used if it does not meet 
		/// any requiements of the developer.
		/// </summary>
		/// <returns>The stream of the *.noia file is returned</returns>
		public StreamReader DownloadStream() => new StreamReader(WebRequest.Create(URL).GetResponse().GetResponseStream());
	}
}