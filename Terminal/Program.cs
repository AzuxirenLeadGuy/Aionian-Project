using Aionian;
using AionianApp.ViewStates;
using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AionianApp.Terminal;
public class Program
{
	public static readonly AppViewModel App = new();
	public bool ExitPressed = false;
	public static void Main() => new Program().Run();
	private static void PrintSep()
	{
		int len = Console.WindowWidth;
		for (int i = 0; i < len; i++) Console.Write("=");
	}
	public static string RL() => Console.ReadLine() ?? throw new Exception("Expected non-null console input!");
	public static string InputFor(string message)
	{
		Console.Write($"{message} :");
		return RL();
	}
	public static string SimpleListChoice(string message, uint startId, params string[] options)
	{
		Console.WriteLine(message);
		for (int i = 0; i < options.Length; i++)
		{
			Console.WriteLine($"{i + startId}. {options[i]}");
		}
		return RL();
	}
	private static void PrintError(string message)
	{
		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine(message);
		Console.ResetColor();
	}
	private void Run(/*string[] args*/)
	{
		Console.WriteLine("Welcome to the Aionian Bible.\nSoftware provided to you by Azuxiren\n\nPlease Wait while the assets are loaded");
		//Init the Application
		AppViewState _state = App.State;
		Console.WriteLine($"Application initialized at {_state.RootDir}");
		//Initialization Complete
		if (_state.ReadState.AvailableBibles.Count == 0)
		{
			AssetManagement();
			if (_state.ReadState.AvailableBibles.Count == 0)
			{
				PrintError("No Default Bible selected. Quitting Application");
				return;
			}
		}
		//By Now, we are sure there are bibles available to read.
		//---Main Menu---
		while (!ExitPressed)
		{
			PrintSep();
			var input = SimpleListChoice(
				"Enter program mode", 1,
				"Bible Chapter Reading", // 1
				"Search Bible References", // 2
				"Download/Manage Bible Assets", // 3
				"Exit" // 4
			);
			switch (input[0])
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
	private static void ChapterDisplay()
	{
		AppViewState _state = App.State;
		Console.WriteLine("Loading Available Bibles. Please Wait...");
		DisplayAvailableBibles();
		Console.WriteLine("\nEnter the bible to load: ");
		if (!int.TryParse(Console.ReadLine(), out int bible) || bible < 1 || bible > _state.ReadState.AvailableBibles.Count)
		{
			PrintError("Invalid Input recieved!");
			return;
		}
		BibleDescriptor selectedbible = _state.ReadState.AvailableBibles[--bible];
		App.LoadBibleChapter(selectedbible);
		_state = App.State;
		List<string> allBookNames = new();
		(BibleBook BookVal, string Name)[] books_list = _state.ReadState.AvailableBooksNames.Select(x => (x.Key, x.Value)).ToArray();
		int maxlength = 0, sno = 0;
		foreach ((BibleBook _, string name) in books_list)
		{
			++sno;
			string str = $"{sno}. {name}";
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
		if (!byte.TryParse(
			InputFor("Enter ID of the Book to read "),
			out byte bookid) || bookid < 1 || bookid > books_list.Length)
		{
			PrintError("Invalid ID");
			return;
		}
		byte currentChapter;
		BibleBook currentBook = books_list[bookid - 1].BookVal;
		App.LoadBibleChapter(selectedbible, currentBook);
		int len = App.State.ReadState.CurrentBookChapterCount;
		if (len == 1) { currentChapter = 1; }
		else
		{
			while (
				!byte.TryParse(InputFor($"This book has {len} chapters. Enter the chapter to load"), out currentChapter)
				|| currentChapter == 0
				|| currentChapter > len)
			{
				PrintError("Invalid Chapter! Try again...");
			}
		}
	sr: bool fetching = App.LoadBibleChapter(selectedbible, currentBook, currentChapter);
		if (!fetching) { PrintError("Could not fetch resources!"); return; }
		_state = App.State;
		currentChapter = _state.ReadState.CurrentSelectedChapter;
		Dictionary<byte, string> chap = _state.ReadState.CurrentChapterContent;
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
				case 1: // Next chapter
					if (currentChapter >= _state.ReadState.CurrentBookChapterCount)
					{
						currentChapter = 0;
						int idx = Array.FindIndex(books_list, x => x.BookVal == currentBook) + 1;
						if (idx >= books_list.Length) { idx = 0; }
						currentBook = books_list[idx].BookVal;
					}
					else { currentChapter++; }
					goto sr;
				case 2: // Previous chapter
					if (currentChapter <= 1)
					{
						currentChapter = 255;
						int idx = Array.FindIndex(books_list, x => x.BookVal == currentBook) - 1;
						if (idx < 0) { idx = books_list.Length - 1; }
						currentBook = books_list[idx].BookVal;
					}
					else { currentChapter--; }
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
			foreach (BibleDescriptor link in _state.ContentState.OfflineBibles) _ = table.AddRow(choice++, link.Title, link.Language, link.AionianEdition ? "Aionian" : "Standard");
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
		AppViewState _state = App.State;
		if (_state.ContentState.OfflineBibles.Count == 0) { PrintError("This program requires at least one bible to be installed. Please Install at least once"); }
		else
		{
			Console.WriteLine("Displaying Installed bibles: ");
			DisplayAvailableBibles();
		}
		switch (SimpleListChoice(
			"Enter your choice", 1,
			"Add a bible",
			"Remove a bible",
			"Remove all Bibles and their respective data")[0])
		{
			case '1':
				Console.WriteLine("Fetching data from server. Please wait...");
				BibleLink[]? list = null;
				int files;
				try { list = DisplayDownloadable(); } catch (Exception e) { Console.WriteLine(e.Message); }
				if (list == null || list.Length == 0) { PrintError("Could not connect to the server... "); break; }
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
					if (_state.ContentState.OfflineBibles.Any(x => link.Equals(x)))
					{
						Console.WriteLine($"Bible {link} already exists");
						continue;
					}
					Console.WriteLine($"Downloading file: {link}");
					ConsoleProgressBar progressBar = new((byte)(Console.WindowWidth / 2));
					try
					{
						progressBar.Write();
						Exception? exp = App.DownloadBibleAsync(link, progressBar).Result;
						if (exp != null) throw exp;
						files++;
						_state = App.State;
						Console.WriteLine("\nFile is saved successfully");
					}
					catch (Exception e)
					{
						Console.WriteLine();
						Console.WriteLine(e.Message);
						Console.WriteLine(e.StackTrace);
						Console.WriteLine("\n\nUnexpected Error");
					}
				}
				Console.WriteLine($"Downloaded {files} file(s) Successfully");
				break;
			case '2':
				files = 0;
				foreach (string Id in InputFor(
					"Enter the ID(s) of the bible to remove (Multiple IDs are to be separted by space")
					.Split(' ', StringSplitOptions.RemoveEmptyEntries))
				{
					if (!int.TryParse(Id, out int x) || x < 1 || x > _state.ContentState.OfflineBibles.Count)
					{
						Console.WriteLine($"Ignoring Invalid input {Id}");
						break;
					}
					BibleDescriptor link = _state.ContentState.OfflineBibles[--x];
					if (App.DeleteBible(link))
					{
						files++;
						Console.WriteLine($"Removed {link}");
					}
					else { PrintError($"Could not remove {link}"); }
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
				if (App.DeleteAllData())
				{
					Console.WriteLine("All assets removed");
					ExitPressed = true;
				}
				else { throw new Exception("Failed to clear assets"); }
				break;
			default: Console.WriteLine("Returning to main menu"); break;
		}
		void DisplayAvailableBibles()
		{
			int choice = 1;
			ConsoleTable table = new("ID", "Title", "Language", "Version");
			foreach (BibleDescriptor link in _state.ContentState.OfflineBibles)
				_ = table.AddRow(choice++, link.Title, link.Language, link.AionianEdition ? "Aionian" : "Standard");
			table.Write();
		}
		BibleLink[] DisplayDownloadable()
		{
			var exp = App.RefreshDownloadLinks();
			if (exp != null) { throw exp; }
			_state = App.State;
			Listing[] results = _state.ContentState.AvailableLinks.ToArray();
			ConsoleTable table = new("ID", "Title", "Language", "Size", "|", "ID", "Title", "Language", "Size");
			for (int i = 0; i < results.Length; i += 2)
			{
				BibleLink link = results[i].Link;
				BibleLink lin2 = results[i + 1].Link;
				_ = table.AddRow(i + 1, link.Title, link.Language, $"{results[i].Bytes / 1048576.0f:0.000} MB", "|", i + 2, lin2.Title, lin2.Language, $"{results[i + 1].Bytes / 1048576.0f:0.000} MB");
			}
			table.Options.EnableCount = false;
			table.Write();
			return results.Select(x => x.Link).ToArray();
		}
	}
	private static void WordSearcher()
	{
		Console.WriteLine("\nEnter the bible to perform search on: ");
		AppViewState _state = App.State;
		DisplayAvailableBibles(_state.ContentState.OfflineBibles);
		if (!int.TryParse(Console.ReadLine(), out int bible) || bible < 1 || bible > _state.ContentState.OfflineBibles.Count)
		{
			Console.WriteLine("Invalid input. Aborting process..");
			return;
		}
		string inputline = InputFor("Enter words(s)").Trim();
		if (inputline.Length == 0) { Console.WriteLine("No input recieved"); return; }
		char input = SimpleListChoice(
			"Enter your choice", 1,
			"Search for any of the words.",
			"Search for All of the words",
			"Regex")[0];
		if (input != '1' && input != '2' && input != '3')
		{
			Console.WriteLine("Returning to main menu");
			return;
		}
		SearchMode mode = input == '1' ? SearchMode.MatchAnyWord : (input == '2' ? SearchMode.MatchAllWords : SearchMode.Regex);
		SearchQuery search;
		try { search = new(inputline, mode); } catch (Exception ex) { PrintError($"Invalid input.\n{ex}"); return; }
		BibleDescriptor desc = _state.ContentState.OfflineBibles[--bible];
		App.LoadBibleChapter(desc);
		Console.WriteLine("Starting Search. Please wait...");
		if (!App.SearchVerses(desc, search, BibleBook.NULL, BibleBook.NULL))
		{
			PrintError("Currently unable to request search");
			return;
		}
		foreach (SearchedVerse result in App.State.SearchState.FoundReferences)
		{
			PrintVerse(
				result.Reference.Book,
				result.Reference.Chapter,
				result.Reference.Verse,
				result.VerseContent);
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
public struct ConsoleProgressBar : IProgress<float>
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
	public void Report(float value)
	{
		_perc = (byte)value;
		Write();
	}
}