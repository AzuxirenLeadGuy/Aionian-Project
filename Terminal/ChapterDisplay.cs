using ConsoleTables;
using System;
using System.Collections.Generic;

namespace Aionian.Terminal
{
	public static partial class Program
	{
		public static readonly string[] BookNames = Enum.GetNames(typeof(BibleBook));
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
			void DisplayAvailableBibles()
			{
				int choice = 1;
				ConsoleTable table = new ConsoleTable("ID", "Title", "Language", "Version");
				foreach (BibleLink link in AvailableBibles) _ = table.AddRow(choice++, link.Title, link.Language, link.AionianEdition ? "Aionian" : "Standard");
				table.Write();
			}
			void DisplayChapter(Dictionary<byte, string> chapter, string shortbookname, byte chapterno)
			{
				int len = chapter.Count;
				for (byte i = 1; i <= len; i++)
				{
					string vh = $"{shortbookname} {chapterno}:{i}";
					Console.WriteLine($"{vh,12}|{chapter[i]}");
				}
			}
		}
	}
}