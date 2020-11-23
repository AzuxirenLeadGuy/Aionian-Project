using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Text;
using ConsoleTables;
using System.Linq;

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
				case '1':
					Console.WriteLine("Fetching data from server. Please wait...");
					BibleLink[] list = null;
					int files;
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
								if (AvailableBibles.Contains(link)) Console.WriteLine($"File {link.AssetFileName()} already exists");
								else
								{
									Console.WriteLine($"Downloading file: {link.AssetFileName()}");
									try
									{
										SaveBible(Bible.ExtractBible(link.DownloadStream()));
										files++;
										AvailableBibles.Add(link);
										Console.WriteLine($"Download Sucessful");
									}
									catch (Exception e) { Console.WriteLine(e.Message); }
								}
							}
							else Console.WriteLine($"Ignoring Invalid input {Id}");
						}
						Console.WriteLine($"Downloaded {files} file(s) Successfully");
					}
					break;
				case '2':
					Console.WriteLine("Enter the ID(s) of the bible to remove (Multiple IDs are to be separted by space");
					files = 0;
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
				case '3':
					Console.WriteLine("This will delete all the content of this tool. Are you sure (y/n)?");
					if (Console.ReadKey(true).Key == ConsoleKey.Y)
					{
						DeleteAllAssets();
						Console.WriteLine("All assets removed");
						ExitPressed = true;
					}
					else Console.WriteLine("Delete process skipped.");
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
				BibleLink[] results = BibleLink.GetAllUrlsFromWebsite();
				ConsoleTable table = new ConsoleTable("ID", "Title", "Language", "Version", "|", "ID", "Title", "Language", "Version");
				for (int i = 0; i < results.Length; i += 2)
				{
					BibleLink link = results[i];
					BibleLink lin2 = results[i + 1];
					System.Diagnostics.Debug.WriteLine($"link={link.AssetFileName()}; lin2={lin2.AssetFileName()};\n{link.Equals(lin2)} : {link.CompareTo(lin2)}\n\n");
					_ = table.AddRow(i + 1, link.Title, link.Language, link.AionianEdition ? "Aionian" : "Standard", "|", i + 2, lin2.Title, lin2.Language, lin2.AionianEdition ? "Aionian" : "Standard");
				}
				table.Options.EnableCount = false;
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
				ConsoleTable table = new ConsoleTable("ID", "Book", "|", "ID", "Book", "|", "ID", "Book");
				for (int i = 1; i < BookNames.Length; i += 3) _ = table.AddRow(i, BookNames[i], "|", i + 1, BookNames[i + 1], "|", i + 2, BookNames[i + 2]);
				table.Options.EnableCount = false;
				table.Write();
				Console.WriteLine("Enter ID of the Book to read: ");
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
						Console.Write($"This book contains {len} chapters. \nEnter Chapter number to read: ");
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
			Console.WriteLine("1. Search for Any of the words\n2. Search for All of the words\n3. Regex (Regular Expression)\n Press any other key to return\nEnter your choice: ");
			char input = Console.ReadKey(true).KeyChar;
			if (input != '1' && input != '2' && input != '3')
			{
				Console.WriteLine("Returning to main menu");
				return;
			}
			Console.WriteLine("Enter words(s) :");
			string inputline = Console.ReadLine().Trim();
			if (inputline.Length == 0) Console.WriteLine("No input recieved");
			else if (Regex.Match(inputline, "[.,\\/#!$%\\^&\\*;:{}=\\-_`~()+='\"<>?/|%]").Success && (input == '1' || input == '2')) Console.WriteLine("Cannot use punctutations for word search. Please use Regex search for that");
			else if (inputline.Count(x => x == ' ') >= 5 && input == '2') Console.WriteLine(@"Option 'Search for All of the words' is not available for more than 5 words.");
			else
			{
				DisplayAvailableBibles();
				Console.WriteLine("\nEnter the bible to load: ");
				if (int.TryParse(Console.ReadLine(), out int bible) && bible >= 1 && bible <= AvailableBibles.Count)
				{
					Bible Bible = LoadBible(AvailableBibles[--bible]);
					Console.WriteLine("Starting Search. Please wait...");
					if (inputline.Contains(' '))//Only if there are multiple words does the input value matters
					{
						if (input == '1') inputline = new StringBuilder("(").Append(inputline).Replace(" ", ")|(").Append(")").ToString();//Use regex that matches any one of the words
						else if (input == '2')//Create the large regex for matching all of the words
						{
							string[] words = inputline.Split(' ', StringSplitOptions.RemoveEmptyEntries);
							int length = words.Length;
							int factorial = length switch
							{
								2 => 2,
								3 => 6,
								4 => 24,
								5 => 120,
								6 => 720,
								_ => throw new ArgumentException($"Did not expect {length} words in option 2 of search"),
							};
							StringBuilder regexpreparer = new StringBuilder();
							for (int i = 1; i <= factorial; i++, _ = NextPermutation(words))
							{
								_ = regexpreparer.Append(@"((\W|^)").Append(words[0]);
								for (int j = 1; j < length; j++) _ = regexpreparer.Append($" .* {words[j]}");
								_ = regexpreparer.Append(@"(\W|$))");
								if (i != factorial) _ = regexpreparer.Append("|");
							}
							inputline = regexpreparer.ToString();
						}
					}
					Regex r = new Regex(inputline, RegexOptions.IgnoreCase);
					foreach ((KeyValuePair<BibleBook, Book> bk, KeyValuePair<byte, Dictionary<byte, string>> ch, KeyValuePair<byte, string> v) in
						from KeyValuePair<BibleBook, Book> bk in Bible.Books
						from KeyValuePair<byte, Dictionary<byte, string>> ch in bk.Value.Chapter
						from KeyValuePair<byte, string> v in ch.Value
						where r.Match(v.Value).Success
						select (bk, ch, v)) PrintVerse(bk.Key, ch.Key, v.Key, v.Value);
				}
				else Console.WriteLine("Invalid input. Aborting process..");
			}
			static void DisplayAvailableBibles()
			{
				int choice = 1;
				ConsoleTable table = new ConsoleTable("ID", "Title", "Language", "Version");
				foreach (BibleLink link in AvailableBibles) _ = table.AddRow(choice++, link.Title, link.Language, link.AionianEdition ? "Aionian" : "Standard");
				table.Write();
			}
			static bool NextPermutation<T>(T[] array) where T : IComparable<T>
			{
				// Find non-increasing suffix
				int i = array.Length - 1;
				while (i > 0 && array[i - 1].CompareTo(array[i]) >= 0) i--;
				if (i <= 0)
				{
					Array.Sort(array);
					return false;
				}
				// Find successor to pivot
				int j = array.Length - 1;
				while (array[j].CompareTo(array[i - 1]) <= 0) j--;
				T temp = array[i - 1];
				array[i - 1] = array[j];
				array[j] = temp;
				// Reverse suffix
				j = array.Length - 1;
				while (i < j)
				{
					temp = array[i];
					array[i] = array[j];
					array[j] = temp;
					i++;
					j--;
				}
				return true;
			}
			static void PrintVerse(BibleBook bibleBook, byte chapterNo, byte verseNo, string verse)
			{
				string vh = $"{bibleBook} {chapterNo}:{verseNo}";
				Console.WriteLine($"{vh,12}|{verse}");
			}
		}
		public static void SaveBible(this Bible bible)
		{
			using FileStream stream = new FileStream(AssetFilePath(bible.AssetFileName()), FileMode.Create);
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, bible);
			stream.Close();
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