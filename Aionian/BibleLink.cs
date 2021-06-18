using System.Collections.Generic;
using System.IO;
using System.Net;
using System;
using System.Threading.Tasks;
namespace Aionian
{
	/// <summary>Represents a single link of an Aionian Bible</summary>
	[Serializable]
	public struct BibleLink : IComparable<BibleLink>, IEquatable<BibleLink>
	{
		/// <summary>The Title of the bible</summary>
		public readonly string Title;
		/// <summary>The Language of the bible</summary>
		public readonly string Language;
		/// <summary>The URL of the bible to download</summary>
		public readonly string URL;
		/// <summary>Indicates whether the bible edition is Aionian or not</summary>
		public readonly bool AionianEdition;
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
		public override string ToString() => $"{Language}|{Title}";
		/// <summary>
		/// Returns a touple of all links available for download. Needless to say, this function requres internet
		/// </summary>
		/// <returns>Returs an array of every link avaialble to download in the Aionian</returns>
		public static (BibleLink Link, ulong SizeInBytes)[] GetAllUrlsFromWebsite()
		{
			string ResourceSite = @"https://raw.githubusercontent.com/AzuxirenLeadGuy/AionianBible_DataFileStandard/master/";
			List<(BibleLink Link, ulong SizeInBytes)> links = new List<(BibleLink Link, ulong SizeInBytes)>();
			using (StreamReader sr = new StreamReader(WebRequest.Create(ResourceSite + "Content.txt").GetResponse().GetResponseStream()))
			{
				while (!sr.EndOfStream)
				{
					string[] responsestring = sr.ReadLine().Split('\t');
					string url = ResourceSite + responsestring[0];
					ulong size = ulong.Parse(responsestring[1]);
					bool aionianEdition = responsestring[0].EndsWith("Aionian-Edition.noia");
					string[] tl = responsestring[0].Replace("---", "|").Split('|');
					string language = tl[1];
					string title = tl[2];
					links.Add((new BibleLink(title, language, url, aionianEdition), size));
				}
			}
			return links.ToArray();
		}
		/// <summary>
		/// This is an unofficial way to download the stream which can be further used to convert into the Bible Class object.
		/// Needless to say, this is not the necessary way to download and should not be used if it does not meet
		/// any requiements of the developer.
		/// </summary>
		/// <returns>The stream of the *.noia file is returned</returns>
		/// <example>
		/// <code>
		/// BibleLink link = ... // obtain a link
		/// Bible downloadedBible = Bible.ExtractBible(link.DownloadStream()); //Convert the downloaded .noia file into a Bible
		/// </code>
		/// </example>
		public StreamReader DownloadStream() => new StreamReader(WebRequest.Create(URL).GetResponse().GetResponseStream());
		/// <summary>
		/// Preview method for downloading and keeping track of progress of download
		/// </summary>
		/// <param name="progressChanged">The event called when the download progress is changed</param>
		/// <param name="downloadDataCompleted">The event called when the download is completed</param>
		/// <returns>Returns the FileStream of the .noia database of Bible</returns>
		public async Task<StreamReader> DownloadStreamAsync(DownloadProgressChangedEventHandler progressChanged = null, DownloadDataCompletedEventHandler downloadDataCompleted = null)
		{
			WebClient client = new WebClient();
			if (progressChanged != null) client.DownloadProgressChanged += progressChanged;
			if (downloadDataCompleted != null) client.DownloadDataCompleted += downloadDataCompleted;
			string result = await client.DownloadStringTaskAsync(new Uri(URL));
			return new StreamReader(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(result)));
		}
	}
}