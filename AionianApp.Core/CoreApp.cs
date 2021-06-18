using System;
using Aionian;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace AionianApp
{
	/// <summary>
	/// The Core App abstract class that is to be inherited build apps upon
	/// </summary>
	public class CoreApp
	{
		/// <summary>The object of the currently executing CoreApp</summary>
		public static CoreApp current;
		/// <summary>
		/// Default and the only constructor for CoreApp.
		/// **Ensure that <c>AvailableBibles</c> contains links. Otherwise prompt the user to download resources**
		/// </summary>
		public CoreApp()
		{
			current = this;
			if (!Directory.Exists(AppDataFolderPath)) _ = Directory.CreateDirectory(AppDataFolderPath);
			if (!File.Exists(AssetMainFilePath)) SaveAssetLog();
			else AvailableBibles = LoadFileAsJson<List<BibleLink>>(AssetMainFilePath);
		}
		/// <summary>
		/// This list stores the available bibles this app has downloaded in its system
		/// </summary>
		protected readonly List<BibleLink> AvailableBibles = new List<BibleLink>();
		/// <summary>
		/// This is the folder path where the Application stores the downloaded files at
		/// </summary>
		/// <returns>(string) Path of the folder</returns>
		public virtual string AppDataFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Aionian-Terminal");
		/// <summary>
		/// This is the file path of the particular asset stored in the the folder given by AppDataFolderPath
		/// </summary>
		/// <param name="file">Name of the file</param>
		/// <returns>(string) Path of the asset file</returns>
		protected virtual string AssetFilePath(string file) => Path.Combine(AppDataFolderPath, file);
		/// <summary>
		/// This is the file name of the Asset Log file: the file in which the asset list metadata is stored at
		/// </summary>
		/// <returns>(string) Name of the Asset Log file</returns>
		public virtual string AssetMainFilePath => AssetFilePath("Asset.dat");
		/// <summary>
		/// Deduces what the filename of the asset (intended to be Bible or BibleLink) will be, given its properties
		/// </summary>
		/// <param name="title">Title of the bible</param>
		/// <param name="lang">Language of the bible</param>
		/// <param name="aionianEdition">Wheter the bible is aionian edition or not</param>
		/// <returns>(string) File name of the asset</returns>
		protected virtual string AssetFileName(string title, string lang, bool aionianEdition) => $"{title}-{lang}-{(aionianEdition ? "Aionian" : "Standard")}.dat";
		/// <summary>
		/// Gets the filename of the link. This will be the (default) download destination of the link
		/// </summary>
		/// <param name="link">BibleLink object to load/download</param>
		/// <returns>(string) File path (of download) of the Bible asset file</returns>
		public string AssetFileName(BibleLink link) => AssetFileName(link.Title, link.Language, link.AionianEdition);
		/// <summary>
		/// Gets the filename of the Bible.<!----> This will be the (default) file path to load/store the bible from
		/// </summary>
		/// <param name="bible">Bible object to load/store</param>
		/// <returns>(string) File path of the bible asset</returns>
		public string AssetFileName(Bible bible) => AssetFileName(bible.Title, bible.Language, bible.AionianEdition);
		/// <summary>
		/// Deletes everything in the AppDataFolderPath
		/// </summary>
		protected virtual void DeleteAllAssets() => Directory.Delete(AppDataFolderPath, true);
		/// <summary>
		/// Saves changes to the asset log file
		/// </summary>
		protected void SaveAssetLog() => SaveFileAsJson(AvailableBibles, AssetMainFilePath);
		/// <summary>
		/// Saves a given file as serialized text file at the given location
		/// </summary>
		/// <param name="item">Object to serialize and store</param>
		/// <param name="filename">File path to store the object</param>
		/// <typeparam name="T">The Type of file to store</typeparam>
		protected void SaveFileAsJson<T>(T item, string filename) => File.WriteAllText(AssetFilePath(filename), JsonConvert.SerializeObject(item, Formatting.Indented));
		/// <summary>
		/// Loads a given file from the deserialized text file stored in the given location
		/// </summary>
		/// <param name="filename">File path of the file to deserialize and load object from</param>
		/// <typeparam name="T">The type of file to load</typeparam>
		/// <returns>(T) The file loaded from the asset file</returns>
		protected T LoadFileAsJson<T>(string filename) => JsonConvert.DeserializeObject<T>(File.ReadAllText(AssetFilePath(filename)));
		/// <summary>
		/// Contains a string of the names of the books. Use this as an option for "Select the book of bible"
		/// </summary>
		/// <returns></returns>
		public readonly string[] BookNames = Enum.GetNames(typeof(BibleBook));
		/// <summary>
		/// This is the text for the 'About Us' content.
		/// </summary>
		public const string AboutUsText = @"Aionian Bible app brought to you by Azuxiren. This app is made under CC-by-4.0 licence and comes with absolutly no guarantee";
		/// <summary>
		/// The bible to load verses from.
		/// </summary>
		protected Bible LoadedBible;
		/// <summary>
		/// Loads the Bible from the asset
		/// </summary>
		/// <param name="link">The link queried to load. Make sure this link exists</param>
		/// <returns>The bible object deserialized from the asset file</returns>
		protected Bible LoadBible(BibleLink link) => LoadedBible = LoadFileAsJson<Bible>(AssetFileName(link));
		/// <summary>
		/// The currently loaded chapter.
		/// </summary>
		public Dictionary<byte, string> LoadedChapter { get; protected set; }
		/// <summary>
		/// Sets the `LoadedChapter` object with the given values
		/// </summary>
		/// <param name="book">BibleBook to load</param>
		/// <param name="chapter">Selected chapter</param>
		protected void LoadChapter(BibleBook book, byte chapter) => LoadedChapter = LoadedBible.Books[book].Chapter[chapter];
	}
}