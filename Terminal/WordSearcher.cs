using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Aionian.Terminal
{
	public static partial class Program
	{
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
							int factorial = 0;
							switch (length)
							{
								case 2:
									factorial = 2;
									break;
								case 3:
									factorial = 6;
									break;
								case 4:
									factorial = 24;
									break;
								case 5:
									factorial = 120;
									break;
								case 6:
									factorial = 720;
									break;
								default: throw new ArgumentException($"Did not expect {length} words in option 2 of search");
							}
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
			void DisplayAvailableBibles()
			{
				int choice = 1;
				ConsoleTable table = new ConsoleTable("ID", "Title", "Language", "Version");
				foreach (BibleLink link in AvailableBibles) _ = table.AddRow(choice++, link.Title, link.Language, link.AionianEdition ? "Aionian" : "Standard");
				table.Write();
			}
			bool NextPermutation<T>(T[] array) where T : IComparable<T>
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
			void PrintVerse(BibleBook bibleBook, byte chapterNo, byte verseNo, string verse)
			{
				string vh = $"{bibleBook} {chapterNo}:{verseNo}";
				Console.WriteLine($"{vh,12}|{verse}");
			}
		}
	}
}