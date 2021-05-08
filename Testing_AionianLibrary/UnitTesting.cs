using Aionian;
using Xunit;
namespace TestingAionianLibrary
{
	public class UnitTesting
	{
		internal BibleLink Link = new("Aionian-Bible", "English", "http://resources.aionianbible.org/Holy-Bible---English---Aionian-Bible---Standard-Edition.noia", false);
		internal const int LinkCount = 380;
		public UnitTesting() { }
		[Fact]
		public void BibleLinkFetching()
		{
			BibleLink[] x = BibleLink.GetAllUrlsFromWebsite();
			Assert.True(x.Length >= LinkCount);
		}
		[Fact]
		public void BibleLinkDownloading()
		{
			Bible englishBible = Bible.ExtractBible(Link.DownloadStream());
			Assert.Equal("In the beginning God created the heavens and the earth.", englishBible[BibleBook.Genesis, 1, 1]);
		}
	}
}