using ConsoleTables;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Aionian.Terminal
{
	public static partial class Program
	{
		public static string AppDataFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Aionian-Terminal");
		public static string AssetFilePath(string file) => Path.Combine(AppDataFolderPath, file);
		public static string AssetMainFilePath => AssetFilePath("Asset.dat");
		public static string AssetFileName(string title, string lang, bool aionianEdition) => $"{title}-{lang}-{(aionianEdition ? "Aionian" : "Standard")}.dat";
		public static string AssetFileName(this BibleLink link) => AssetFileName(link.Title, link.Language, link.AionianEdition);
		public static string AssetFileName(this Bible link) => AssetFileName(link.Title, link.Language, link.AionianEdition);
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
										Bible downloadedbible = Bible.ExtractBible(link.DownloadStream());
										SaveFileAsJsonAsync(downloadedbible, downloadedbible.AssetFileName()).Wait();
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
			void DisplayAvailableBibles()
			{
				int choice = 1;
				ConsoleTable table = new ConsoleTable("ID", "Title", "Language", "Version", "Location");
				foreach (BibleLink link in AvailableBibles) _ = table.AddRow(choice++, link.Title, link.Language, link.AionianEdition ? "Aionian" : "Standard", AssetFilePath(link.AssetFileName()));
				table.Write();
			}
			BibleLink[] DisplayDownloadable()
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
		private static void DeleteAllAssets() => Directory.Delete(AppDataFolderPath, true);
		public static Bible LoadBible(this BibleLink link) => LoadFileAsJsonAsync<Bible>(link.AssetFileName()).Result;
		public static void WriteAssetLog() => SaveFileAsJsonAsync(AvailableBibles, AssetMainFilePath).Wait();

		public static async Task SaveFileAsJsonAsync<T>(T item, string filename)
		{
			using (FileStream sr = new FileStream(AssetFilePath(filename), FileMode.Create)) await JsonSerializer.SerializeAsync(sr, item, new JsonSerializerOptions() { IncludeFields = true });
		}
		public static async Task<T> LoadFileAsJsonAsync<T>(string filename)
		{
			using (FileStream sr = new FileStream(AssetFilePath(filename), FileMode.Open)) return await JsonSerializer.DeserializeAsync<T>(sr, new JsonSerializerOptions() { IncludeFields = true });
		}
	}
}