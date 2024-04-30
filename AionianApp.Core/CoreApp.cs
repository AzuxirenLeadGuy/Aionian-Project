using Aionian;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace AionianApp;
/// <summary> The Core App functionalities to build apps upon</summary>
public class CoreApp
{
	/// <summary>
	/// Default and the only constructor for CoreApp.
	/// **Ensure that <c>AvailableBibles</c> contains links. Otherwise prompt the user to download resources**
	/// </summary>
	public CoreApp() : this(
		Path.Combine(
		Environment.GetFolderPath(
			Environment.SpecialFolder.ApplicationData),
		"Aionian-Terminal")
	)
	{ }
	/// <summary>
	/// Default and the only constructor for CoreApp.
	/// **Ensure that <c>AvailableBibles</c> contains links. Otherwise prompt the user to download resources**
	/// </summary>
	public CoreApp(string rootFolder)
	{
		AppDataFolderPath = rootFolder;
		if (!Directory.Exists(AppDataFolderPath))
			_ = Directory.CreateDirectory(AppDataFolderPath);
		if (!File.Exists(AssetMainFilePath)) { SaveAssetLog(); }
		else
		{
			AvailableBibles = LoadFileAsJson<List<BibleDescriptor>>(
				AssetMainFilePath) ?? throw new Exception(
					"Unable to load assets!");
		}
	}
	/// <summary>
	/// This list stores the available bibles this app has downloaded in its system
	/// </summary>
	protected internal readonly List<BibleDescriptor> AvailableBibles = new();
	/// <summary>
	/// Returns a read-only collections of bibles available to read.
	/// </summary>
	public IEnumerable<BibleDescriptor> GetBibles() => AvailableBibles.ToArray();
	/// <summary>
	/// This is the folder path where the Application stores the downloaded files at
	/// </summary>
	/// <returns>(string) Path of the folder</returns>
	public string AppDataFolderPath { get; protected set; }
	/// <summary>
	/// This is the file path of the particular asset stored in the folder given by AppDataFolderPath
	/// </summary>
	/// <param name="name">Name of the file</param>
	/// <returns>(string) Path of the asset file/folder</returns>
	public virtual string AssetPath(string name) => Path.Combine(
		AppDataFolderPath,
		name);
	/// <summary>
	/// This is the file name of the Asset Log file:
	/// the file in which the asset list metadata is stored at
	/// </summary>
	/// <returns>(string) Name of the Asset Log file</returns>
	public virtual string AssetMainFilePath => AssetPath("Asset.dat");
	/// <summary>
	/// Deduces what the filename of the asset (intended to be Bible or BibleLink) will be,
	/// given its properties
	/// </summary>
	/// <param name="title">Title of the bible</param>
	/// <param name="lang">Language of the bible</param>
	/// <param name="aionianEdition">Wheter the bible is aionian edition or not</param>
	/// <returns>(string) Directory path of the bible asset</returns>
	protected virtual string AssetDirName(string title, string lang, bool aionianEdition) =>
		$"{title}-{lang}-{(aionianEdition ? "Aionian" : "Standard")}";
	/// <summary>
	/// Gets the folder name of the files for this Bible. This will be the (default)
	/// download destination of the link
	/// </summary>
	/// <param name="link">BibleLink object to load/download</param>
	/// <returns>(string) Directory path of the bible asset</returns>
	public string AssetDirName(BibleLink link) => AssetDirName(
		link.Title,
		link.Language,
		link.AionianEdition);
	/// <summary>
	/// Gets the folder name of the files for this Bible. This will be the (default)
	/// file path to load/store the bible from
	/// </summary>
	/// <param name="bible">BibleDescriptor object to load/store</param>
	/// <returns>(string) Directory path of the bible asset</returns>
	public string AssetDirName(BibleDescriptor bible) => AssetDirName(
		bible.Title,
		bible.Language,
		bible.AionianEdition);
	/// <summary>
	/// Gets the folder name of the files for this Bible. This will be the (default)
	/// file path to load/store the bible from
	/// </summary>
	/// <param name="bible">BibleDescriptor object to load/store</param>
	/// <returns>(string) Directory path of the bible asset</returns>
	public string AssetFileName(BibleDescriptor bible) =>
		$"{AssetDirName(bible)}/Root.dat";
	/// <summary>Gets the name of the file storing this particular book of the bible</summary>
	/// <param name="book">The book of the bible</param>
	/// <param name="desc">The bible descrption</param>
	/// <returns>(string) File path of the bible asset</returns>
	public string AssetFileName(
		BibleBook book,
		BibleDescriptor desc) => ChapterwiseBible.GetBookPath(
			AssetDirName(desc),
			book);
	/// <summary>
	/// Deletes everything in the AppDataFolderPath
	/// </summary>
	public virtual void DeleteAllAssets() =>
		Directory.Delete(AppDataFolderPath, true);
	/// <summary>
	/// Saves changes to the asset log file
	/// </summary>
	protected internal void SaveAssetLog() =>
		SaveFileAsJson(AvailableBibles, AssetMainFilePath);

	private static JsonSerializerOptions DefaultOptions() => new()
	{
		AllowTrailingCommas = true,
		WriteIndented = true,
		IncludeFields = true
	};
	/// <summary>
	/// Saves a given file as serialized text file at the given location
	/// </summary>
	/// <param name="item">Object to serialize and store</param>
	/// <param name="filename">File path to store the object</param>
	/// <typeparam name="T">The Type of file to store</typeparam>
	protected internal void SaveFileAsJson<T>(
		T item,
		string filename) => File.WriteAllText(
			AssetPath(filename),
			JsonSerializer.Serialize<T>(
				item,
				DefaultOptions()));
	/// <summary>
	/// Loads a given file from the deserialized text file stored in the given location
	/// </summary>
	/// <param name="filename">File path of the file to deserialize and load object from</param>
	/// <typeparam name="T">The type of file to load</typeparam>
	/// <returns>(T) The file loaded from the asset file</returns>
	protected internal T? LoadFileAsJson<T>(string filename) =>
		JsonSerializer.Deserialize<T>(
			File.ReadAllText(
				AssetPath(filename)),
				DefaultOptions());
	/// <summary>
	/// Deletes a given bible and updates the internal list
	/// of Available Bibles
	/// </summary>
	/// <param name="desc">The ID of the bible to delete</param>
	public virtual void Delete(BibleDescriptor desc)
	{
		if (!AvailableBibles.Contains(desc)) return;
		Directory.Delete(AssetPath(AssetDirName(desc)), true);
		AvailableBibles.Remove(desc);
		SaveAssetLog();
	}
	/// <summary> Check if the given link is present in the app or not</summary>
	/// <param name="link">The link to check</param>
	/// <returns>(bool) true if link exists already, otherwise false</returns>
	public virtual bool CheckExists(BibleLink link) =>
		AvailableBibles.Any(
			z =>
				z.Title == link.Title &&
				z.Language == link.Language &&
				z.AionianEdition == link.AionianEdition);
}