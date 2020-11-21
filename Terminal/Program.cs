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
		public static string AssetFileName(string title, string lang, bool aionianEdition) => $".{title}-{lang}-{(aionianEdition ? "Aionian" : "Standard")}.dat";
		public static string AssetFileName(this BibleLink link) => AssetFileName(link.Title, link.Language, link.AionianEdition);
		public static string AssetFileName(this Bible link) => AssetFileName(link.Title, link.Language, link.AionianEdition);
		public static bool ExitPressed = false;
		public static readonly string[] BookNames = Enum.GetNames(typeof(BibleBook));
		private static void Init()
		{
			//Make AppDataFolder and AssetDataFile if it does not already exist
			Console.WriteLine($"Asset path is {AssetMainFilePath}");
			if (!Directory.Exists(AppDataFolderPath)) _ = Directory.CreateDirectory(AppDataFolderPath);
			if (!File.Exists(AssetMainFilePath)) WriteAssetLog();
			else
			{
				using FileStream stream = new FileStream(AssetMainFilePath, FileMode.Open);
				BinaryFormatter formatter = new BinaryFormatter();
				AvailableBibles = (List<BibleLink>)formatter.Deserialize(stream);
			}
		}
		private static void DeleteAllAssets() => Directory.Delete(AppDataFolderPath, true);
		private static void Main(/*string[] args*/)
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
			while (!ExitPressed)
			{
				int len = Console.LargestWindowWidth;
				for (int i = 0; i < len; i++) Console.Write("=");
				Console.WriteLine();
				Console.WriteLine("\n1. Bible Chapter Reading\n2. Bible verse search \n3. Download Bible Modules \n4. Exit");
				Console.WriteLine("Enter Your Choice: ");
				switch (Console.ReadKey(true).KeyChar)
				{
					case '1': ChapterDisplay(); break;
					case '2': WordSearcher(); break;
					case '3': AssetManagement(); break;
					case '4': ExitPressed = true; break;
					default: Console.WriteLine("Invaild Input. Press the number of the options given above. Try Again."); break;
				}
			}
			Console.WriteLine("Thank you for using Aionian-Terminal, brought to you by AzuxirenLeadGuy");
		}
		private static void AssetManagement()
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
					foreach (string Id in Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries))
					{
						if (int.TryParse(Id, out int x) && x >= 1 && x <= AvailableBibles.Count)
						{
							BibleLink link = AvailableBibles[--x];
							try
							{
								File.Delete(AssetFilePath(AssetFileName(link)));
								files++;
								Console.WriteLine($"Removed {link.AssetFileName()}");
								_ = AvailableBibles.Remove(link);
							}
							catch (Exception e) { Console.WriteLine(e.Message); }
						}
						else Console.WriteLine($"Ignoring Invalid input {Id}");
					}
					Console.WriteLine($"Removed {files} file(s) Successfully");
					break;
				case '1':
					Console.WriteLine("Fetching data from server. Please wait...");
					BibleLink[] list = null;
					try { list = DisplayDownloadable(); }
					catch (Exception e) { Console.WriteLine(e.Message); }
					if (list == null || list.Length == 0) Console.WriteLine("Could not connect to the server... ");
					else
					{
						Console.WriteLine("Enter the ID(s) of the bible to download (Multiple IDs are to be separted by space");
						files = 0;
						foreach (string Id in Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries))
						{
							if (int.TryParse(Id, out int x) && x >= 1 && x <= list.Length)
							{
								BibleLink link = list[--x];
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
			if (!ExitPressed) WriteAssetLog();
			static void DisplayAvailableBibles()
			{
				int choice = 1;
				ConsoleTable table = new ConsoleTable("ID", "Title", "Language", "Version", "Location");
				foreach (BibleLink link in AvailableBibles) _ = table.AddRow(choice++, link.Title, link.Language, link.AionianEdition ? "Aionian" : "Standard", AssetFilePath(link.AssetFileName()));
				table.Write();
			}
			static BibleLink[] DisplayDownloadable()
			{
				BibleLink[] results = ABD.GetAllUrls();
				ConsoleTable table = new ConsoleTable("ID", "Title", "Language", "Version", "|", "ID", "Title", "Language", "Version");
				for (int i = 0; i < results.Length; i += 2)
				{
					BibleLink link = results[i];
					BibleLink lin2 = results[i];
					_ = table.AddRow(i + 1, link.Title, link.Language, link.AionianEdition ? "Aionian" : "Standard", "|", i + 2, lin2.Title, lin2.Language, lin2.AionianEdition ? "Aionian" : "Standard");
				}

				table.Write();
				return results;
			}
		}
		private static void ChapterDisplay()
		{
			Console.WriteLine("Loading Available Bibles. Please Wait...");
			DisplayAvailableBibles();
			Console.WriteLine("\nEnter the bible to load: ");
			if (int.TryParse(Console.ReadLine(), out int bible) && bible >= 1 && bible <= AvailableBibles.Count)
			{
				Bible Bible = LoadBible(AvailableBibles[--bible]);
				Console.WriteLine("Enter Book to read: ");
				ConsoleTable table = new ConsoleTable("ID", "Book", "|", "ID", "Book", "|", "ID", "Book");
				for (int i = 1; i < BookNames.Length; i += 3) _ = table.AddRow(i, BookNames[i], "|", i + 1, BookNames[i + 1], "|", i + 2, BookNames[i + 2]);
				table.Write();
				Console.WriteLine();
				if (byte.TryParse(Console.ReadLine(), out byte book) && book >= 1 && book <= 66)
				{
					Book Book = Bible.Books[(BibleBook)book];
					int len = Book.Chapter.Count;
					if (len == 1)
					{
						DisplayChapter(Book.Chapter[1], Book.ShortBookName, 1);
						goto skip;
					}
					else
					{
						Console.WriteLine("Enter Chapter");
						if (byte.TryParse(Console.ReadLine(), out byte chapter) && chapter >= 1 && chapter <= len)
						{
							DisplayChapter(Book.Chapter[chapter], Book.ShortBookName, chapter);
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
				ConsoleTable table = new ConsoleTable("ID", "Title", "Language", "Version");
				foreach (BibleLink link in AvailableBibles) _ = table.AddRow(choice++, link.Title, link.Language, link.AionianEdition ? "Aionian" : "Standard");
				table.Write();
			}
			static void DisplayChapter(Dictionary<byte, string> chapter, string shortbookname, byte chapterno)
			{
				int len = chapter.Count;
				for (byte i = 1; i <= len; i++)
				{
					string vh = $"{shortbookname} {chapterno}:{i}";
					Console.WriteLine($"{vh,12}|{chapter[i]}");
				}
			}
		}
		private static void WordSearcher()
		{

		}
		public static void SaveBible(this Bible bible)
		{
			using FileStream stream = new FileStream(AssetFilePath(bible.AssetFileName()), FileMode.Create);
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, bible);
			stream.Close();
			File.WriteAllText(bible.AssetFileName(), System.Text.Json.JsonSerializer.Serialize(bible));
		}
		public static Bible LoadBible(this BibleLink link)
		{
			using FileStream stream = new FileStream(AssetFilePath(link.AssetFileName()), FileMode.Open);
			BinaryFormatter formatter = new BinaryFormatter();
			return (Bible)formatter.Deserialize(stream);
		}
		public static void WriteAssetLog()
		{
			using FileStream stream = new FileStream(AssetMainFilePath, FileMode.Create);
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, AvailableBibles);
			stream.Close();
		}
	}
}