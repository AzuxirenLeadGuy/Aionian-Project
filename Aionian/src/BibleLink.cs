using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Aionian;
/// <summary>The listing information available</summary>
public readonly record struct Listing
{
	/// <summary>The link for the bibles</summary>
	public BibleLink Link { get; init; }
	/// <summary>The size of the download in bytes</summary>
	public ulong Bytes { get; init; }
}
/// <summary>Represents a single link of an Aionian Bible</summary>
public readonly record struct BibleLink : IComparable<BibleLink>, IEquatable<BibleLink>
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
	/// Returns Hash Code
	/// </summary>
	/// <returns>Returns Hash Code</returns>
	public override int GetHashCode() => URL.GetHashCode();
	/// <summary>
	/// Returns the string represntation of this object
	/// </summary>
	/// <returns>Returns the URL</returns>
	public override string ToString() => $"{Language}|{Title}";
	/// <summary> The default URL for downloading the assets </summary>
	public const string default_url = "https://raw.githubusercontent.com/AzuxirenLeadGuy/AionianBible_DataFileStandard/master/";
	/// <summary> /// Returns a touple of all links available for download. Needless to say, this function requres internet</summary>
	/// <param name="resourceSite">The site to download the resources from</param>
	/// <param name="quiet_return">If true, any exception thrown in the process is caught, and an empty list is returned instead</param>
	/// <returns>Returs an array of every link avaialble to download in the Aionian</returns>
	public static Listing[] GetAllUrlsFromWebsite(string resourceSite = default_url, bool quiet_return = true)
	{
		List<Listing> links = new();
		string base_url = resourceSite + "Content.txt";
		try
		{
			HttpClient clinet = new();
			using (StreamReader sr = new(
				clinet.GetStreamAsync(
					base_url).Result))
			{
				while (!sr.EndOfStream)
				{
					string[]? responsestring = (sr.ReadLine()?.Split('\t')) ??
						throw new ArgumentException(
							"Unable to parse data from online dataset");
					string url = resourceSite + responsestring[0];
					ulong size = ulong.Parse(responsestring[1]);
					bool ae = responsestring[0].EndsWith(
						"Aionian-Edition.noia");
					string[] tl = responsestring[0].Replace(
						"---",
						"|").Split(
							'|');
					string lg = tl[1];
					string title = tl[2];
					links.Add(
						new Listing()
						{
							Link = new BibleLink(title,lg,url,ae),
							Bytes = size
						});
				}
			}
			return links.ToArray();
		}
		catch when (quiet_return) { return Array.Empty<Listing>(); }
	}
	/// <summary>
	/// Preview method for downloading and keeping track of progress of download
	/// </summary>
	/// <param name="handler">The event called when the download progress is changed</param>
	/// <param name="cancellationToken">The event called when the download is completed</param>
	/// <returns>Returns the FileStream of the .noia database of Bible</returns>
	public async Task<StreamReader> DownloadStreamAsync(
		HttpMessageHandler? handler = null,
		CancellationToken cancellationToken = default)
	{
		using HttpClient client = handler != null ?
			new(handler) :
			new();
		Stream st = await client.GetStreamAsync(
			URL,
			cancellationToken);
		return new StreamReader(st);
	}
	/// <summary>Compares the two instances</summary>
	/// <param name="left">BibleLink instance</param>
	/// <param name="right">BibleLink instance</param>
	public static bool operator <(BibleLink left, BibleLink right) =>
		left.CompareTo(right) < 0;
	/// <summary>Compares the two instances</summary>
	/// <param name="left">BibleLink instance</param>
	/// <param name="right">BibleLink instance</param>
	public static bool operator <=(BibleLink left, BibleLink right) =>
		left.CompareTo(right) <= 0;
	/// <summary>Compares the two instances</summary>
	/// <param name="left">BibleLink instance</param>
	/// <param name="right">BibleLink instance</param>
	public static bool operator >(BibleLink left, BibleLink right) =>
		left.CompareTo(right) > 0;
	/// <summary>Compares the two instances</summary>
	/// <param name="left">BibleLink instance</param>
	/// <param name="right">BibleLink instance</param>
	public static bool operator >=(BibleLink left, BibleLink right) =>
		left.CompareTo(right) >= 0;
}