using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ConsoleTables;
namespace Aionian.Terminal
{
	public static class Program
	{
		public static List<BibleLink> AvailableBibles = new List<BibleLink>();
		public static string AppDataFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Aionian-Terminal");
		public static string AssetFilePath(string file) => Path.Combine(AppDataFolderPath, file);
		public static string AssetMainFilePath => AssetFilePath("Asset.dat");
		public static string AssetFileName(this BibleLink link) => $"{link.Title}-{link.Language}-{(link.AionianEdition ? "Aionian" : "Standard")}.dat";
		public static string AssetFileName(this Bible link) => $"{link.Title}-{link.Language}-{(link.AionianEdition ? "Aionian" : "Standard")}.dat";
		public static bool ExitPressed = false;
		static void Init()
		{
			//Make AppDataFolder and AssetDataFile if it does not already exist
			if (!Directory.Exists(AppDataFolderPath)) Directory.CreateDirectory(AppDataFolderPath);
			if (!File.Exists(AssetMainFilePath)) WriteAssetLog();
			else
			{
				using var stream = new FileStream(AssetMainFilePath, FileMode.Open);
				var formatter = new BinaryFormatter();
				AvailableBibles = (List<BibleLink>)formatter.Deserialize(stream);
			}
		}
		static void DeleteAllAssets() => Directory.Delete(AppDataFolderPath, true);
		static void Main(/*string[] args*/)
		{
			Console.WriteLine("Welcome to the Aionian Bible.\nSoftware provided to you by Azuxiren\n\nPlease Wait while the assets are loaded");
			//Display ConsoleBar
			Init();
			if (AvailableBibles.Count == 0)
			{
				AssetManagement();
				if (AvailableBibles.Count == 0)
				{
					Console.WriteLine("No Default Bible selected. Quitting Application");
					return;
				}
			}
			Console.WriteLine("\n1. Bible Chapter Reading\n2. Bible verse search \n3. Download Bible Modules \n4. Exit");
			while (!ExitPressed)
			{
				Console.WriteLine("Enter Your Choice: ");
				switch (Console.ReadKey(true).KeyChar)
				{
					case '1': ChapterDisplay(); break;
					case '2': WordSearcher(); break;
					case '3': AssetManagement(); break;
					case '4': ExitPressed = true; break;
					default: Console.WriteLine("Invaild Input. Press the number of the options given above. Try Again."); break;
				}
				var len = Console.LargestWindowWidth;
				for (int i = 0; i < len; i++) Console.Write("=");
				Console.WriteLine();
			}
			Console.WriteLine("Thank you for using Aionian-Terminal, brought to you by AzuxirenLeadGuy");
		}
		static void AssetManagement()
		{
			if (AvailableBibles.Count == 0) Console.WriteLine("This program requires at least one bible to be installed. Please Install at least once");
			else
			{
				Console.WriteLine("Displaying Installed bibles: ");
				DisplayAvailableBibles();
			}
			Console.WriteLine("\n1. Add a Bible\n2. Remove a Bible\n3. Remove All Bibles and Data\nPress any other key to go back to main menu\nEnter your choice : ");
			switch (Console.ReadKey(true).KeyChar)
			{
				case '2':
					Console.WriteLine("Enter the ID(s) of the bible to remove (Multiple IDs are to be separted by space");
					int files = 0;
					foreach (var Id in Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries))
					{
						if (int.TryParse(Id, out var x) && x >= 1 && x <= AvailableBibles.Count)
						{
							var link = AvailableBibles[--x];
							try
							{
								File.Delete(AssetFilePath(AssetFileName(link)));
								files++;
								Console.WriteLine($"Removed {link.AssetFileName()}");
								AvailableBibles.Remove(link);
							}
							catch (Exception e) { Console.WriteLine(e.Message); }
						}
						else Console.WriteLine($"Ignoring Invalid input {Id}");
					}
					Console.WriteLine($"Removed {files} file(s) Successfully");
					break;
				case '1':
					BibleLink[] list = null;
					try { list = DisplayDownloadable(); }
					catch (Exception e) { Console.WriteLine(e.Message); }
					if (list == null || list.Length == 0) Console.WriteLine("Could not connect to the server... ");
					else
					{
						Console.WriteLine("Enter the ID(s) of the bible to download (Multiple IDs are to be separted by space");
						files = 0;
						foreach (var Id in Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries))
						{
							if (int.TryParse(Id, out var x) && x >= 1 && x <= AvailableBibles.Count)
							{
								var link = list[--x];
								Console.WriteLine($"Downloading file: {link.AssetFileName()}");
								try
								{
									SaveBible(ABD.ExtractBible(link.DownloadStream()));
									files++;
									AvailableBibles.Add(link);
									Console.WriteLine($"Download Sucessful");
								}
								catch (Exception e) { Console.WriteLine(e.Message); }
							}
							else Console.WriteLine($"Ignoring Invalid input {Id}");
						}
						Console.WriteLine($"Downloaded {files} file(s) Successfully");
					}
					break;
				case '3':
					DeleteAllAssets();
					Console.WriteLine("All assets removed");
					ExitPressed = true;
					break;
				default: Console.WriteLine("Returning to main menu"); break;
			}
			static void DisplayAvailableBibles()
			{
				int choice = 1;
				var table = new ConsoleTable("ID", "Title", "Language", "Version");
				foreach (var link in AvailableBibles) table.AddRow(choice++, link.Title, link.Language, link.AionianEdition ? "Aionian" : "Standard");
				table.Write();
			}
			static BibleLink[] DisplayDownloadable()
			{
				var results = ABD.GetAllUrls();
				int choice = 1;
				var table = new ConsoleTable("ID", "Title", "Language", "Version");
				foreach (var link in results) if (AvailableBibles.Contains(link)) table.AddRow(choice++, link.Title, link.Language, link.AionianEdition ? "Aionian" : "Standard");
				table.Write();
				return results;
			}
		}
		static void ChapterDisplay()
		{
			Console.WriteLine("Loading Available Bibles. Please Wait...");
			DisplayAvailableBibles();
			Console.WriteLine("\nEnter the bible to load: ");
			if (int.TryParse(Console.ReadLine(), out var bible) && bible >= 1 && bible <= AvailableBibles.Count)
			{
				var Bible = LoadBible(AvailableBibles[--bible]);
				var z = Enum.GetNames(typeof(BibleBook));
				Console.WriteLine("Enter Book to read: ");
				for (int i = 1; i <= 66; i++) Console.Write($"{i}.{z[i]}\t");
				Console.WriteLine();
				if (byte.TryParse(Console.ReadLine(), out var book) && book >= 1 && book <= 66)
				{
					var Book = Bible.Books[(BibleBook)book];
					var len = Book.Chapter.Count;
					if (len == 1)
					{
						DisplayChapter(Book.Chapter[1], Book.ShortBookName, 1);
						goto skip;
					}
					else
					{
						Console.WriteLine("Enter Chapter");
						if (byte.TryParse(Console.ReadLine(), out var chapter) && chapter >= 1 && chapter <=len)
						{
							DisplayChapter(Book.Chapter[1], Book.ShortBookName, chapter);
							goto skip;
						}
					}
				}
			}
			Console.WriteLine("Recieved invalid input. Aborting process...");
		skip:;
			static void DisplayAvailableBibles()
			{
				int choice = 1;
				var table = new ConsoleTable("ID", "Title", "Language", "Version");
				foreach (var link in AvailableBibles) table.AddRow(choice++, link.Title, link.Language, link.AionianEdition ? "Aionian" : "Standard");
				table.Write();
			}
			static void DisplayChapter(Dictionary<byte, string> chapter, string shortbookname, byte chapterno)
			{
				var len = chapter.Count;
				for (byte i = 1; i <= len; i++)
				{
					Console.WriteLine($"{shortbookname} {chapterno}:{i} | {chapter[i]}");
				}
			}
		}
		static void WordSearcher()
		{

		}
		public static void SaveBible(this Bible bible)
		{
			using var stream = new FileStream(bible.AssetFileName(), FileMode.Create);
			var formatter = new BinaryFormatter();
			formatter.Serialize(stream, bible);
			stream.Close();
		}
		public static Bible LoadBible(this BibleLink link)
		{
			using var stream = new FileStream(link.AssetFileName(), FileMode.Open);
			var formatter = new BinaryFormatter();
			return (Bible)formatter.Deserialize(stream);
		}
		public static void WriteAssetLog()
		{
			using var stream = new FileStream(AssetMainFilePath, FileMode.Create);
			var formatter = new BinaryFormatter();
			formatter.Serialize(stream, AvailableBibles);
			stream.Close();
		}
	}
}