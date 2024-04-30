using Aionian;
using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Text.RegularExpressions;

namespace AionianApp.Terminal;
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
			Console.WriteLine("Enter Your Choice: ");
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
		if (!int.TryParse(Console.ReadLine(), out int bible) || bible < 1 || bible > AvailableBibles.Count)
		{
			PrintError("Invalid Input recieved!");
			return;
		}
		AppViewModel chapterwiseBible = new(this, AvailableBibles[--bible]);
		List<string> allBookNames = new();
		BibleBook[] books_list = chapterwiseBible.AvailableBooks;
		int maxlength = 0, sno = 0;
		foreach (var bkpair in chapterwiseBible.AvailableBookNames)
		{
			++sno;
			string str = $"{sno}. {bkpair}";
			allBookNames.Add(str);
			maxlength = maxlength > str.Length + 2 ? maxlength : str.Length + 2;
		}
		int columns = Console.WindowWidth / maxlength;
	bk:;
		PrintSep();
		if (columns < 2)
		{
			foreach (string val in allBookNames) Console.WriteLine(val);
		}
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
		if (!byte.TryParse(Console.ReadLine(), out byte bookid) || bookid < 1 || bookid > chapterwiseBible.BookCount)
		{
			PrintError("Invalid ID");
			return;
		}
		byte currentChapter;
		BibleBook currentBook = books_list[bookid - 1];
		chapterwiseBible.LoadReading(currentBook);
		int len = chapterwiseBible.AvailableChapters;
		if (len == 1)
		{
			currentChapter = 1;
		}
		else
		{
			while (
				!byte.TryParse(Console.ReadLine(), out currentChapter)
				|| currentChapter == 0
				|| currentChapter > len)
			{
				PrintError("Invalid Chapter! Try again...");
			}
		}
	sr: chapterwiseBible.LoadReading(currentBook, currentChapter);
		Dictionary<byte, string> chap = chapterwiseBible.CurrentReading;
		DisplayChapter(
			chap,
			Enum.GetName(
				typeof(BibleBook),
				currentBook) ?? "<UNKNOWN>",
			currentChapter);
		PrintSep();
		Console.WriteLine("1. Read Next Chapter\n2. Read Previous Chapter\n3. Back to book select\n4. Back to main menu");
		if (byte.TryParse(Console.ReadLine(), out byte rgoption))
		{
			switch (rgoption)
			{
				case 3:
					goto bk;
				case 4:
					return;
				case 1:
					chapterwiseBible.NextChapter();
					currentBook = chapterwiseBible.CurrentBook;
					currentChapter = chapterwiseBible.CurrentChapter;
					goto sr;
				case 2:
					chapterwiseBible.PrevChapter();
					currentBook = chapterwiseBible.CurrentBook;
					currentChapter = chapterwiseBible.CurrentChapter;
					goto sr;
				default:
					PrintError("Invalid Input entered");
					goto case 4;
			}
		}

		void DisplayAvailableBibles()
		{
			int choice = 1;
			ConsoleTable table = new("ID", "Title", "Language", "Version");
			foreach (BibleDescriptor link in AvailableBibles) _ = table.AddRow(choice++, link.Title, link.Language, link.AionianEdition ? "Aionian" : "Standard");
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
	private static void PrintError(string message)
	{
		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine(message);
		Console.ResetColor();
	}
	public void AssetManagement()
	{
		if (AvailableBibles.Count == 0)
		{
			Console.WriteLine("This program requires at least one bible to be installed. Please Install at least once");
		}
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
				if (list == null || list.Length == 0)
				{
					Console.WriteLine("Could not connect to the server... ");
					break;
				}
				Console.WriteLine("Enter the ID(s) of the bible to download (Multiple IDs are to be separted by space");
				files = 0;
				foreach (string Id in RL().Split(' ', StringSplitOptions.RemoveEmptyEntries))
				{
					if (!int.TryParse(Id, out int x) || x < 1 || x > list.Length)
					{
						Console.WriteLine($"Ignoring Invalid input {Id}");
						continue;
					}
					BibleLink link = list[--x];
					if (CheckExists(link))
					{
						Console.WriteLine($"File {AssetDirName(link)} already exists");
						continue;
					}
					Console.WriteLine($"Downloading file: {AssetDirName(link)}");
					ConsoleProgressBar progressBar = new((byte)(Console.WindowWidth / 2));
					ProgressMessageHandler handler = new(new HttpClientHandler() { AllowAutoRedirect = true });
					try
					{
						handler.HttpReceiveProgress += OnDownloadProgress;
						progressBar.Write();
						bool result = ChapterwiseBible.DownloadBibleAsync(
							this,
							link,
							handler).Result; //Bible.ExtractBible(link.DownloadStreamAsync(handler).Result);
						if (!result) throw new Exception("Inner exception while downloading and storing");
						files++;
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
				Console.WriteLine($"Downloaded {files} file(s) Successfully");
				break;
			case '2':
				Console.WriteLine("Enter the ID(s) of the bible to remove (Multiple IDs are to be separted by space");
				files = 0;
				foreach (string Id in RL().Split(' ', StringSplitOptions.RemoveEmptyEntries))
				{
					if (!int.TryParse(Id, out int x) || x < 1 || x > AvailableBibles.Count)
					{
						Console.WriteLine($"Ignoring Invalid input {Id}");
						break;
					}
					BibleDescriptor link = AvailableBibles[--x];
					try
					{
						File.Delete(AssetPath(AssetDirName(link)));
						files++;
						Console.WriteLine($"Removed {AssetDirName(link)}");
						_ = AvailableBibles.Remove(link);
					}
					catch (Exception e) { Console.WriteLine(e.Message); }
				}
				Console.WriteLine($"Removed {files} file(s) Successfully");
				break;
			case '3':
				Console.WriteLine("This will delete all the content of this tool. Are you sure (y/n)?");
				char confirm = RL()[0];
				if (confirm != 'Y' && confirm != 'y')
				{
					Console.WriteLine("Delete process skipped.");
					break;
				}
				DeleteAllAssets();
				Console.WriteLine("All assets removed");
				ExitPressed = true;

				break;
			default: Console.WriteLine("Returning to main menu"); break;
		}
		if (!ExitPressed) SaveAssetLog();
		void DisplayAvailableBibles()
		{
			int choice = 1;
			ConsoleTable table = new("ID", "Title", "Language", "Version", "Location");
			foreach (BibleDescriptor link in AvailableBibles)
				_ = table.AddRow(choice++, link.Title, link.Language, link.AionianEdition ? "Aionian" : "Standard", AssetPath(AssetDirName(link)));
			table.Write();
		}
		BibleLink[] DisplayDownloadable()
		{
			Listing[] results = BibleLink.GetAllUrlsFromWebsite();
			ConsoleTable table = new("ID", "Title", "Language", "Size", "|", "ID", "Title", "Language", "Size");
			for (int i = 0; i < results.Length; i += 2)
			{
				BibleLink link = results[i].Link;
				BibleLink lin2 = results[i + 1].Link;
				Debug.WriteLine($"link={AssetDirName(link)}; lin2={AssetDirName(lin2)};\n{link.Equals(lin2)} : {link.CompareTo(lin2)}\n\n");
				_ = table.AddRow(i + 1, link.Title, link.Language, $"{results[i].Bytes / 1048576.0f:0.000} MB", "|", i + 2, lin2.Title, lin2.Language, $"{results[i + 1].Bytes / 1048576.0f:0.000} MB");
			}
			table.Options.EnableCount = false;
			table.Write();
			List<BibleLink> links = new();
			foreach (Listing item in results) links.Add(item.Link);
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
		if (inputline.Length == 0)
		{
			Console.WriteLine("No input recieved");
			return;
		}
		else if (Regex.Match(inputline, "[.,\\/#!$%\\^&\\*;:{}=\\-_`~()+='\"<>?/|%]").Success && (input == '1' || input == '2'))
		{
			Console.WriteLine("Cannot use punctutations for word search. Please use Regex search for that");
			return;
		}
		else if (inputline.Count(x => x == ' ') >= 5 && input == '2')
		{
			Console.WriteLine("Option 'Search for All of the words' is not available for more than 5 words.");
			return;
		}
		DisplayAvailableBibles(AvailableBibles);
		Console.WriteLine("\nEnter the bible to load: ");
		if (!int.TryParse(Console.ReadLine(), out int bible) || bible < 1 || bible > AvailableBibles.Count)
		{
			Console.WriteLine("Invalid input. Aborting process..");
			return;
		}
		ChapterwiseBible Mybible = ChapterwiseBible.LoadChapterwiseBible(
			this,
			AvailableBibles[--bible]);
		Console.WriteLine("Starting Search. Please wait...");
		SearchMode mode = input == '1' ? SearchMode.MatchAnyWord : (input == '2' ? SearchMode.MatchAllWords : SearchMode.Regex);
		SearchQuery search = new(inputline, mode);
		foreach (BibleReference result in search.GetResults(Mybible))
		{
			PrintVerse(result.Book, result.Verse, result.Chapter, Mybible[result]);
		}
	}
	private static void DisplayAvailableBibles(IEnumerable<BibleDescriptor> bibles)
	{
		int choice = 1;
		ConsoleTable table = new("ID", "Title", "Language", "Version");
		foreach (BibleDescriptor link in bibles)
		{
			_ = table.AddRow(
				choice++,
				link.Title,
				link.Language,
				link.AionianEdition ? "Aionian" : "Standard");
		}
		table.Write();
	}
	private static void PrintVerse(BibleBook bibleBook, byte chapterNo, byte verseNo, string verse)
	{
		string vh = $"{bibleBook} {chapterNo}:{verseNo}";
		Console.WriteLine($"{vh,12}|{verse}");
	}
}

[Flags]
public enum ProgressFormat : byte
{
	None,
	BarOnly = 1,
	PercOnly = 2,
	RevolverOnly = 4
}
public struct ConsoleProgressBar
{
	public static readonly char block = '■';
	private readonly bool _fixedLength;
	private byte _length;
	public byte Progress { get; private set; }
	private bool _lastRevolveState;
	public byte Percentage
	{
		readonly get => _perc;
		set
		{
			_perc = value;
			Progress = (byte)(_perc * _length / 100);
		}
	}
	public ConsoleProgressBar(byte l)
	{
		_length = l;
		_fixedLength = l >= 2;
		_perc = 0;
		Progress = 0;
		_written = 0;
		_lastRevolveState = false;
	}
	private byte _perc;
	private byte _written;
	private void Erase()
	{
		int x = _written;
		while (--x >= 0) Console.Write('\b');
		_written = 0;
	}
	private void UpdateBar()
	{
		Console.Write("[");
		_written += 2;
		for (byte i = 0; i < _length; i++)
		{
			if (100 * i < Percentage * _length) Console.Write(block);
			else Console.Write(" ");
		}
		_written += _length;
		Console.Write("]");
	}
	private void UpdatePerc()
	{
		Console.Write(_perc.ToString().PadLeft(3, ' ') + '%');
		_written += 4;
	}
	private void UpdateRevolver()
	{
		if (_lastRevolveState) Console.Write("+");
		else Console.Write("x");
		_lastRevolveState = !_lastRevolveState;
		_written++;
	}
	public void Write(ProgressFormat format = ProgressFormat.BarOnly | ProgressFormat.PercOnly | ProgressFormat.RevolverOnly)
	{
		if (_written != 0) Erase();
		if (!_fixedLength) _length = (byte)((Console.WindowWidth > 255 ? 255 : Console.WindowWidth) * 4 / 5);
		if ((format & ProgressFormat.BarOnly) != 0) UpdateBar();
		if ((format & ProgressFormat.PercOnly) != 0) UpdatePerc();
		if ((format & ProgressFormat.RevolverOnly) != 0) UpdateRevolver();
	}
}