using Aionian;
using ConsoleTables;
using Progress_bar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Handlers;

namespace AionianApp.Terminal
{
	public class Program : CoreApp
	{
		public bool ExitPressed = false;
		public static void Main() => new Program().Run();
		private static void PrintSep()
		{
			int len = Console.WindowWidth;
			for (int i = 0; i < len; i++) Console.Write("=");
		}
		public static string RL() => Console.ReadLine() ?? throw new Exception("Expected non-null console input!");
		private void Run(/*string[] args*/)
		{
			Console.WriteLine("Welcome to the Aionian Bible.\nSoftware provided to you by Azuxiren\n\nPlease Wait while the assets are loaded");
			//Init the Application
			//Make AppDataFolder and AssetDataFile if it does not already exist
			Console.WriteLine($"Asset path is {AssetMainFilePath}");
			//Initialization Complete
			if (AvailableBibles.Count == 0)
			{
				AssetManagement();
				if (AvailableBibles.Count == 0)
				{
					Console.WriteLine("No Default Bible selected. Quitting Application");
					return;
				}
			}
			//By Now, we are sure there are bibles available to read.
			//---Main Menu---
			while (!ExitPressed)
			{
				PrintSep();
				Console.WriteLine();
				Console.WriteLine("\n1. Bible Chapter Reading\n2. Bible verse search \n3. Download Bible Modules \n4. Exit");
				Console.WriteLine("Enter Your Choice: "); ;
				switch (RL()[0])
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
		private void ChapterDisplay()
		{
			Console.WriteLine("Loading Available Bibles. Please Wait...");
			DisplayAvailableBibles();
			Console.WriteLine("\nEnter the bible to load: ");
			if (int.TryParse(Console.ReadLine(), out int bible) && bible >= 1 && bible <= AvailableBibles.Count)
			{
				ChapterwiseBible chapterwiseBible = new(LoadFileAsJson<Bible>(AssetFileName(AvailableBibles[--bible])));
				List<string> allBookNames = new();
				int maxlength = 0, sno = 0;
				var loaded_bible = chapterwiseBible.LoadedBible ?? throw new ArgumentException("Unable to process the loaded bible!");
				var bookCopy = loaded_bible.Books;
				foreach (BibleBook bk in chapterwiseBible.CurrentAllBooks)
				{
					++sno;
					string str = $"{sno}. {bookCopy[bk].RegionalBookName}";
					allBookNames.Add(str);
					maxlength = maxlength > str.Length + 2 ? maxlength : str.Length + 2;
				}// Console.WriteLine($"{bk.BookIndex}. {bk.RegionalBookName}");
				int columns = Console.WindowWidth / maxlength;
			bk:;
				PrintSep();
				if (columns < 2) foreach (string val in allBookNames) Console.WriteLine(val);
				else
				{
					int j = 0;
					while (sno > j)
					{
						for (int i = 0; i < columns && sno > j; i++)
						{
							Console.Write(allBookNames[j++].PadRight(maxlength));
						}
						Console.WriteLine();
					}
				}
				Console.WriteLine("Enter ID of the Book to read: ");
				if (byte.TryParse(Console.ReadLine(), out byte bookid) && bookid >= 1 && bookid <= 66 && bookCopy.ContainsKey((BibleBook)bookid))
				{
					byte chapter;
					BibleBook id = (BibleBook)bookid;
					int len = bookCopy[id].Chapter.Count;
					if (len == 1) chapter = 1;
					else
					{
					akag: Console.Write($"This book contains {len} chapters. \nEnter Chapter number to read: ");
						if (!byte.TryParse(Console.ReadLine(), out chapter) || chapter < 1 || chapter > len)
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine("Invalid Chapter! Try again...");
							Console.ResetColor();
							goto akag;
						}
					}
				sr: chapterwiseBible.LoadChapter(id, chapter);
					Dictionary<byte, string> chap = chapterwiseBible.LoadedChapter ?? throw new ArgumentException("Chapter is null");
					DisplayChapter(chap, Bible.ShortBookNames[(byte)chapterwiseBible.CurrentBook], chapterwiseBible.CurrentChapter);
					PrintSep();
					Console.WriteLine("1. Read Next Chapter\n2. Read Previous Chapter\n3. Back to book select\n4. Back to main menu");
					if (byte.TryParse(Console.ReadLine(), out byte rgoption))
					{
						switch (rgoption)
						{
							case 3:
								goto bk;
							case 4:
								goto skip;
							case 1:
								chapterwiseBible.NextChapter();
								chapter = chapterwiseBible.CurrentChapter;
								id = chapterwiseBible.CurrentBook;
								goto sr;
							case 2:
								chapterwiseBible.PreviousChapter();
								chapter = chapterwiseBible.CurrentChapter;
								id = chapterwiseBible.CurrentBook;
								goto sr;
							default: break;
						}
					}
				}
			}
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Recieved invalid input. Aborting process...");
			Console.ResetColor();
		skip:;
			void DisplayAvailableBibles()
			{
				int choice = 1;
				ConsoleTable table = new("ID", "Title", "Language", "Version");
				foreach (BibleLink link in AvailableBibles) _ = table.AddRow(choice++, link.Title, link.Language, link.AionianEdition ? "Aionian" : "Standard");
				table.Write();
			}
			void DisplayChapter(Dictionary<byte, string> chapter, string shortbookname, byte chapterno)
			{
				foreach (byte verse in chapter.Keys)
				{
					string vh = $"{shortbookname} {chapterno}:{verse}";
					Console.WriteLine($"{vh,12}|{chapter[verse]}");
				}
			}
		}
		public void AssetManagement()
		{
			if (AvailableBibles.Count == 0) Console.WriteLine("This program requires at least one bible to be installed. Please Install at least once");
			else
			{
				Console.WriteLine("Displaying Installed bibles: ");
				DisplayAvailableBibles();
			}
			Console.WriteLine("\n1. Add a Bible\n2. Remove a Bible\n3. Remove All Bibles and Data\nPress any other key to go back to main menu\nEnter your choice : ");
			switch (RL()[0])
			{
				case '1':
					Console.WriteLine("Fetching data from server. Please wait...");
					BibleLink[]? list = null;
					int files;
					try { list = DisplayDownloadable(); }
					catch (Exception e) { Console.WriteLine(e.Message); }
					if (list == null || list.Length == 0) Console.WriteLine("Could not connect to the server... ");
					else
					{
						Console.WriteLine("Enter the ID(s) of the bible to download (Multiple IDs are to be separted by space");
						files = 0;
						foreach (string Id in RL().Split(' ', StringSplitOptions.RemoveEmptyEntries))
						{
							if (int.TryParse(Id, out int x) && x >= 1 && x <= list.Length)
							{
								BibleLink link = list[--x];
								if (AvailableBibles.Contains(link)) Console.WriteLine($"File {AssetFileName(link)} already exists");
								else
								{
									Console.WriteLine($"Downloading file: {AssetFileName(link)}");
									ConsoleProgressBar progressBar = new((byte)(Console.WindowWidth / 2));
									ProgressMessageHandler handler = new(new HttpClientHandler() { AllowAutoRedirect = true });
									try
									{
										handler.HttpReceiveProgress += OnDownloadProgress;
										progressBar.Write();
										Bible downloadedbible = Bible.ExtractBible(link.DownloadStreamAsync(handler).Result);
										SaveFileAsJson(downloadedbible, AssetFileName(downloadedbible));
										files++;
										AvailableBibles.Add(link);
										Console.WriteLine("\nFile is saved successfully");
									}
									catch (Exception e)
									{
										Console.WriteLine(e.Message);
										Console.WriteLine("\n\nUnexpected Error");
									}
									void OnDownloadProgress(object? o, HttpProgressEventArgs e)
									{
										progressBar.Percentage = (byte)e.ProgressPercentage;
										progressBar.Write();
										if (progressBar.Percentage == 100)
											Console.WriteLine("\nFile Download Complete.");
									}
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
					foreach (string Id in RL().Split(' ', StringSplitOptions.RemoveEmptyEntries))
					{
						if (int.TryParse(Id, out int x) && x >= 1 && x <= AvailableBibles.Count)
						{
							BibleLink link = AvailableBibles[--x];
							try
							{
								File.Delete(AssetFilePath(AssetFileName(link)));
								files++;
								Console.WriteLine($"Removed {AssetFileName(link)}");
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
					char confirm = RL()[0];
					if (confirm == 'Y' || confirm == 'y')
					{
						DeleteAllAssets();
						Console.WriteLine("All assets removed");
						ExitPressed = true;
					}
					else Console.WriteLine("Delete process skipped.");
					break;
				default: Console.WriteLine("Returning to main menu"); break;
			}
			if (!ExitPressed) SaveAssetLog();
			void DisplayAvailableBibles()
			{
				int choice = 1;
				ConsoleTable table = new("ID", "Title", "Language", "Version", "Location");
				foreach (BibleLink link in AvailableBibles) _ = table.AddRow(choice++, link.Title, link.Language, link.AionianEdition ? "Aionian" : "Standard", AssetFilePath(AssetFileName(link)));
				table.Write();
			}
			BibleLink[] DisplayDownloadable()
			{
				(BibleLink Link, ulong SizeInBytes)[] results = BibleLink.GetAllUrlsFromWebsite();
				ConsoleTable table = new("ID", "Title", "Language", "Size", "|", "ID", "Title", "Language", "Size");
				for (int i = 0; i < results.Length; i += 2)
				{
					BibleLink link = results[i].Link;
					BibleLink lin2 = results[i + 1].Link;
					Debug.WriteLine($"link={AssetFileName(link)}; lin2={AssetFileName(lin2)};\n{link.Equals(lin2)} : {link.CompareTo(lin2)}\n\n");
					_ = table.AddRow(i + 1, link.Title, link.Language, $"{results[i].SizeInBytes / 1048576.0f:0.000} MB", "|", i + 2, lin2.Title, lin2.Language, $"{results[i + 1].SizeInBytes / 1048576.0f:0.000} MB");
				}
				table.Options.EnableCount = false;
				table.Write();
				List<BibleLink> links = new();
				foreach ((BibleLink Link, ulong SizeInBytes) in results) links.Add(Link);
				return links.ToArray();
			}

		}
		private void WordSearcher()
		{
			Console.WriteLine("1. Search for Any of the words\n2. Search for All of the words\n3. Regex (Regular Expression)\n Press any other key to return\nEnter your choice: ");
			char input = RL()[0];
			if (input != '1' && input != '2' && input != '3')
			{
				Console.WriteLine("Returning to main menu");
				return;
			}
			Console.WriteLine("Enter words(s) :");
			string inputline = RL().Trim();
			if (inputline.Length == 0) Console.WriteLine("No input recieved");
			else if (Regex.Match(inputline, "[.,\\/#!$%\\^&\\*;:{}=\\-_`~()+='\"<>?/|%]").Success && (input == '1' || input == '2')) Console.WriteLine("Cannot use punctutations for word search. Please use Regex search for that");
			else if (inputline.Count(x => x == ' ') >= 5 && input == '2') Console.WriteLine(@"Option 'Search for All of the words' is not available for more than 5 words.");
			else
			{
				DisplayAvailableBibles();
				Console.WriteLine("\nEnter the bible to load: ");
				if (int.TryParse(Console.ReadLine(), out int bible) && bible >= 1 && bible <= AvailableBibles.Count)
				{
					Bible MyBible = LoadFileAsJson<Bible>(AssetFileName(AvailableBibles[--bible]));
					Console.WriteLine("Starting Search. Please wait...");
					SearchMode mode = input == '1' ? SearchMode.MatchAnyWord : (input == '2' ? SearchMode.MatchAllWords : SearchMode.Regex);
					SearchQuery search = new(inputline, mode);
					foreach (BibleReference result in search.GetResults(MyBible))
					{
						PrintVerse(result.Book, result.Verse, result.Chapter, MyBible[result]);
					}
				}
				else Console.WriteLine("Invalid input. Aborting process..");
			}
			void DisplayAvailableBibles()
			{
				int choice = 1;
				ConsoleTable table = new("ID", "Title", "Language", "Version");
				foreach (BibleLink link in AvailableBibles) _ = table.AddRow(choice++, link.Title, link.Language, link.AionianEdition ? "Aionian" : "Standard");
				table.Write();
			}
			void PrintVerse(BibleBook bibleBook, byte chapterNo, byte verseNo, string verse)
			{
				string vh = $"{bibleBook} {chapterNo}:{verseNo}";
				Console.WriteLine($"{vh,12}|{verse}");
			}
		}
	}
}