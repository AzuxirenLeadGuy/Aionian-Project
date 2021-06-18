using Aionian;
using System.IO;
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
		public void TestName()
		{
			CrossReferenceDatabase x = new();
			x.ReadFromFile(40);
			JsonSerializerOptions options = new() { IncludeFields = true };
			File.WriteAllText("CR.json", JsonSerializer.Serialize(x.AllCrossReferences, options));
		}
	}
}