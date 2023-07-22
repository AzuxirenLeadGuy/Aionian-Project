using Aionian;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace Testing_AionianLibrary;

[TestClass]
public class UnitTest1
{
	internal BibleLink Link = new("Aionian-Bible", "English", "https://raw.githubusercontent.com/AzuxirenLeadGuy/AionianBible_DataFileStandard/master/Holy-Bible---English---Aionian-Bible---Standard-Edition.noia", false);
	internal const int LinkCount = 214;
	[TestMethod]
	public void BibleLinkFetching()
	{
		(BibleLink Link, ulong SizeInBytes)[] x = BibleLink.GetAllUrlsFromWebsite();
		Assert.IsTrue(x.Length >= LinkCount);
	}
	[TestMethod]
	public void BibleLinkDownloading()
	{
		Bible englishBible = Bible.ExtractBible(Link.DownloadStream());
		Assert.AreEqual("In the beginning, God created the heavens and the earth.", englishBible[BibleBook.Genesis, 1, 1]);
	}
	[TestMethod]
	public void CrossReferenceTest()
	{
		CrossReferenceDatabase x = new();
		x.ReadFromFile(40);
		JsonSerializerOptions options = new() { IncludeFields = true };
		File.WriteAllText("CR.json", JsonSerializer.Serialize(x.AllCrossReferences, options));
	}
	[TestMethod]
	public void RegionalBookNameTest()
	{
		(BibleLink Link, ulong SizeInBytes)[] allLinks = BibleLink.GetAllUrlsFromWebsite();
		BibleLink link = allLinks.First((x) => x.Link.Language == "Albanian-Tosk").Link;
		Bible bible = Bible.ExtractBible(link.DownloadStream());
		Assert.AreEqual("1 i Samuelit", bible.Books[BibleBook.I_Samuel].RegionalBookName);
	}
}