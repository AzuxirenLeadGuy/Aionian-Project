using System;
using System.Collections.Generic;
using System.IO;
namespace Aionian.Terminal
{
	public static partial class Program
	{
		public static List<BibleLink> AvailableBibles = new List<BibleLink>();
		public static bool ExitPressed = false;
		private static void Main(/*string[] args*/)
		{
			Console.WriteLine("Welcome to the Aionian Bible.\nSoftware provided to you by Azuxiren\n\nPlease Wait while the assets are loaded");
			//Init the Application
			//Make AppDataFolder and AssetDataFile if it does not already exist
			Console.WriteLine($"Asset path is {AssetMainFilePath}");
			if (!Directory.Exists(AppDataFolderPath)) _ = Directory.CreateDirectory(AppDataFolderPath);
			if (!File.Exists(AssetMainFilePath)) WriteAssetLog();
			else AvailableBibles = LoadFileAsJson<List<BibleLink>>(AssetMainFilePath);
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
	}
}