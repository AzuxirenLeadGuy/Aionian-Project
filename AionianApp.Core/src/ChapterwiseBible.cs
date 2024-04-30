using Aionian;
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AionianApp;
/// <summary>A bible wrapped with helper functions that load a single chapter of a bible at a time</summary>
public class ChapterwiseBible : Bible
{
	/// <summary>The path of the books of Bible, given the root</summary>
	public static string GetBookPath(string root, BibleBook book) => $"{root}/{(byte)book}.dat";
	///<summary>The root folder where files for this bible is stored</summary>
	public readonly string RootPath;
	/// <summary>
	/// Loads the Bible from the asset
	/// </summary>
	/// <param name="loadedBible">The bible queried to load.</param>
	/// <param name="path">The path of bible books to load.</param>
	/// <returns>The bible object deserialized from the asset file</returns>
	public ChapterwiseBible(BibleDescriptor loadedBible, string path) : base(loadedBible) => RootPath = path;

	/// <summary>Fetches a book from this bible</summary>
	public override Book FetchBook(BibleBook book) =>
		JsonSerializer.Deserialize<Book>(
			File.ReadAllText(
				GetBookPath(RootPath, book)));
	/// <summary>
	/// Loads and returns the chapterwise bible struct from the assets stored in this device.
	/// </summary>
	/// <param name="app">The core app to load the bible from.</param>
	/// <param name="link">The link/ID of the bible to load.</param>
	/// <returns>The `ChapterwiseBible` struct for this link (if it exists)</returns>
	public static ChapterwiseBible LoadChapterwiseBible(
		CoreApp app,
		BibleDescriptor link) => new(
		app.LoadFileAsJson<BibleDescriptor?>(
			app.AssetFileName(link)) ??
			throw new ArgumentException($"Given link is invalid: {app.AssetDirName(link)}"),
		app.AssetPath(
			app.AssetDirName(link)));

	/// <summary>
	/// Downloads, stores a Bible from the given link, and updates
	/// the internal list of Available Bibles
	/// </summary>
	/// <param name="app">The core app to save the bible in.</param>
	/// <param name="link">The link to download</param>
	/// <param name="handler">The HttpMessageHandler for updating the progress if any</param>
	public static async Task<bool> DownloadBibleAsync(
		CoreApp app,
		BibleLink link,
		HttpMessageHandler? handler = null)
	{
		try
		{
			StreamReader stream = await link.DownloadStreamAsync(handler);
			DownloadInfo data = Bible.ExtractBible(stream);
			BibleDescriptor desc = data.Descriptor;
			string path = app.AssetPath(app.AssetDirName(desc));
			Directory.CreateDirectory(path);
			app.SaveFileAsJson(desc, app.AssetFileName(desc));
			foreach (var kp in data.Books)
			{
				app.SaveFileAsJson<Book>(
					kp.Value,
					app.AssetFileName(kp.Key, desc));
			}
			app.AvailableBibles.Add(desc);
			app.SaveAssetLog();
		}
		catch { return false; }
		return true;
	}
}
