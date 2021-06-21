using Aionian;
using System.IO;
using System.Linq;
using System.Text.Json;
using Xunit;
namespace TestingAionianLibrary
{
	public class UnitTesting
	{
		internal BibleLink Link = new("Aionian-Bible", "English", "https://raw.githubusercontent.com/AzuxirenLeadGuy/AionianBible_DataFileStandard/master/Holy-Bible---English---Aionian-Bible---Standard-Edition.noia", false);
		internal const int LinkCount = 214;
		[Fact]
		public void BibleLinkFetching()
		{
			(BibleLink Link, ulong SizeInBytes)[] x = BibleLink.GetAllUrlsFromWebsite();
			Assert.True(x.Length >= LinkCount);
		}
		[Fact]
		public void BibleLinkDownloading()
		{
			Bible englishBible = Bible.ExtractBible(Link.DownloadStream());
			Assert.Equal("In the beginning God created the heavens and the earth.", englishBible[BibleBook.Genesis, 1, 1]);
		}
		[Fact]
		public void CrossReferenceTest()
		{
			CrossReferenceDatabase x = new();
			x.ReadFromFile(40);
			JsonSerializerOptions options = new() { IncludeFields = true };
			File.WriteAllText("CR.json", JsonSerializer.Serialize(x.AllCrossReferences, options));
		}
		[Fact]
		public void RegionalBookNameTest()
		{
			(BibleLink Link, ulong SizeInBytes)[] allLinks = BibleLink.GetAllUrlsFromWebsite();
			BibleLink link = allLinks.Where((x) => x.Link.Language == "Albanian-Tosk").First().Link;
			Bible bible = Bible.ExtractBible(link.DownloadStream());
			Assert.Equal("1 i Samuelit", bible.Books[BibleBook.I_Samuel].RegionalBookName);
		}
	}
}