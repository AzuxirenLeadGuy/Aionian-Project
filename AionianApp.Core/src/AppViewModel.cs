using Aionian;
using AionianApp.ViewStates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AionianApp;

/// <summary>The functionalities that an AionianApp ViewModel should provide</summary>
public class AppViewModel<AppState> : IViewModel<AppState>
where AppState : AppViewState, new()
{
	/// <summary>Fetches the data to populate the view</summary>
	public AppState State => _state;
	/// <summary>The data that can be used to populate the view</summary>
	protected AppState _state = new();
	/// <summary>The object to lock with</summary>
	protected SemaphoreSlim __lock = new(1, 1);
	/// <summary>The path of the config directory</summary>
	protected readonly string _configDir;
	/// <summary>The path of the config file</summary>
	protected string ConfigFile => $"{_configDir}/config.json";
	/// <summary>The references that match the search parameters</summary>
	protected readonly List<SearchedVerse> _searchList = new();
	/// <summary>The cached list of the bibles available to download</summary>
	protected readonly List<Listing> _downloadLinks = new();
	/// <summary>The bibles available to the app</summary>
	protected List<BibleDescriptor> _offlineBibles = new();
	/// <summary>The cache to load the chapterwise data from</summary>
	protected (BibleDescriptor bible, BibleBook book, Book value) _cache;
	/// <inheritdoc/>
	public event EventHandler<AppState>? OnUpdate;
	/// <summary>Loads the App from the content data in the Config directory</summary>
	/// <param name="path">The path of the config directory</param>
	public AppViewModel(string path)
	{
		if (!Directory.Exists(path))
		{
			if (File.Exists(path))
				throw new ArgumentException("Expected path of root directory, and not the file!", nameof(path));
			else
				_ = Directory.CreateDirectory(path);
		}
		_configDir = path;
		if (File.Exists(ConfigFile))
		{
			_offlineBibles = JsonSerializer.Deserialize<List<BibleDescriptor>>(
				File.ReadAllText(ConfigFile),
				DefaultOptions()
			) ?? new();
		}
		else { SaveAssetLog(); }
		_cache = (default, default, default);
		_state = new();
		_state.AvailableBibles.AddRange(_offlineBibles);
		_state.ContentState.AvailableLinks = _downloadLinks;
		_state.SearchState.FoundReferences = _searchList;
		_state.RootDir = _configDir;
		_state.CurrentAppState = AppViewState.State.Initialized;
		OnUpdate?.Invoke(this, _state);
	}
	/// <summary>Loads the App from the content data in the Config directory taken as the Local Application Data folder</summary>
	public AppViewModel() : this(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AionianAppData")) { }
	/// <summary>The path of the directory where the contents of the book is stored</summary>
	/// <param name="desc">The descriptor of the bible</param>
	/// <returns>The path of the resource</returns>
	protected virtual string BibleDirectoryPath(BibleDescriptor desc) => $"{_configDir}/{desc.ToString().Replace(' ', '_')}";
	/// <summary>The path of the file where the contents of the book is stored</summary>
	/// <param name="desc">The descriptor of the bible</param>
	/// <param name="book">The book value to load</param>
	/// <returns>The path of the resource</returns>
	protected virtual string BibleBookPath(BibleDescriptor desc, BibleBook book) => $"{BibleDirectoryPath(desc)}/{(byte)book}.json";
	private static JsonSerializerOptions DefaultOptions() => new()
	{
		AllowTrailingCommas = true,
		WriteIndented = true,
		IncludeFields = true
	};
	/// <summary>Save the data within OfflineBibles List</summary>
	protected virtual void SaveAssetLog()
	{
		File.WriteAllText(
			ConfigFile,
			JsonSerializer.Serialize<List<BibleDescriptor>>(
				_offlineBibles,
				DefaultOptions()
			));
		_state.AvailableBibles.Clear();
		_state.AvailableBibles.AddRange(_offlineBibles);
		OnUpdate?.Invoke(this, _state);
	}
	/// <summary>Download and save a link</summary>
	/// <param name="link">The link to download</param>
	/// <param name="progress_updater">Recieves callback to update the download progress</param>
	/// <param name="cancellationToken">Allows cancellation of the download task</param>
	/// <returns>If the download and save process was sucessful, returns true; otherwise false</returns>
	public virtual async Task<Exception?> DownloadBibleAsync(
		BibleLink link,
		IProgress<float>? progress_updater = null,
		CancellationToken cancellationToken = default)
	{
		Exception? exception = null;
		string DirStore = "";
		if (cancellationToken == default) cancellationToken = CancellationToken.None;
		if (__lock.Wait(5000, cancellationToken))
		{
			try
			{
				ProgressMessageHandler handler = new(
					new HttpClientHandler() { AllowAutoRedirect = true });
				if (progress_updater != null)
				{
					handler.HttpReceiveProgress += (_, e) =>
						progress_updater.Report(e.ProgressPercentage);
				}

				DownloadInfo data = Bible.ExtractBible(
					await link.DownloadStreamAsync(handler, cancellationToken));
				BibleDescriptor desc = data.Descriptor;
				DirStore = BibleDirectoryPath(desc);
				Directory.CreateDirectory(DirStore);
				foreach (KeyValuePair<BibleBook, Book> kp in data.Books)
				{
					if (kp.Key == BibleBook.NULL)
						throw new InvalidDataException();
					File.WriteAllText(
					BibleBookPath(desc, kp.Key),
					JsonSerializer.Serialize<Book>(
						kp.Value,
						DefaultOptions()
					));
					cancellationToken.ThrowIfCancellationRequested();
				}
				_offlineBibles.Add(desc);
				_state.CurrentAppState = AppViewState.State.BiblesUpdated;
				SaveAssetLog(); // OnUpdate invoked here
			}
			catch (Exception ex)
			{
				if (DirStore.Length != 0 && Directory.Exists(DirStore))
				{
					Directory.Delete(DirStore, true);
				}
				exception = ex;
			}
			finally { __lock.Release(); }
		}
		else { exception = new AccessViolationException(); }
		return exception;
	}
	/// <summary>Deletes a bible</summary>
	/// <param name="descriptor">The bible to delete</param>
	/// <returns>If the delete process was successful, returns true; otherwise false</returns>
	public virtual bool DeleteBible(BibleDescriptor descriptor)
	{
		if (__lock.Wait(5000))
		{
			bool result = _offlineBibles.Remove(descriptor);
			if (result)
			{
				Directory.Delete(BibleDirectoryPath(descriptor), true);
				_state.CurrentAppState = AppViewState.State.BiblesUpdated;
				SaveAssetLog(); // OnUpdate invoked here
			}
			__lock.Release();
			return result;
		}
		else { return false; }
	}
	/// <summary>Fetches the entire book</summary>
	/// <param name="bible">The bible from which the book is to be loaded</param>
	/// <param name="book">The book to load</param>
	/// <returns>The book of the bible</returns>
	protected virtual Book LoadBook(BibleDescriptor bible, BibleBook book) =>
		JsonSerializer.Deserialize<Book>(
			File.ReadAllText(
				BibleBookPath(bible, book)));
	/// <summary>Performs a search on the bible for the given query</summary>
	/// <param name="bible">The bible to search on</param>
	/// <param name="query">The query to search</param>
	/// <param name="start">The first book to search</param>
	/// <param name="end">The last book to search</param>
	/// <param name="progress_updater">The search process reports the progress/percentage of the search to this object</param>
	/// <param name="cancellationToken">The token that can be used to cancel the search</param>
	/// <returns>Returns the collection of bible verses found</returns>
	public virtual bool SearchVerses(
		BibleDescriptor bible,
		SearchQuery query,
		BibleBook start = BibleBook.NULL,
		BibleBook end = BibleBook.NULL,
		IProgress<float>? progress_updater = null,
		CancellationToken cancellationToken = default)
	{
		_searchList.Clear();
		if (bible.RegionalName.Count == 0) { return true; }
		if (_cache.bible != bible)
			LoadBibleChapter(bible);
		if (cancellationToken == default) cancellationToken = CancellationToken.None;
		if (start == BibleBook.NULL) { start = bible.RegionalName.First().Key; }
		if (end == BibleBook.NULL) { end = bible.RegionalName.Last().Key; }
		if (__lock.Wait(5000, cancellationToken))
		{
			BibleSearchRange range = new()
			{
				SearchBookCount = (byte)end - (byte)start + 1,
				SearchRange = bible.RegionalName.Keys,
				Fetcher = (x) => LoadBook(bible, x),
			};
			_searchList.AddRange(query.GetResults(
				range,
				onProgressUpdate: progress_updater,
				ct: cancellationToken));
			_state.CurrentAppState = AppViewState.State.SearchUpdated;
			OnUpdate?.Invoke(this, _state);
			return true;
		}
		else { return false; }
	}
	/// <summary>Fetches/Loads the downloaded bible</summary>
	/// <param name="bible">The bible to load</param>
	/// <param name="book">The book to load. By default, loads the first book of the bible</param>
	/// <param name="chapter">
	/// The chapter to load. By default, loads the first chapter of the book. <br/>
	/// If chapter value is higher than chapters within book (say, 255), loads the last chapter of the book.
	/// </param>
	/// <returns>Returns true if the required bible chapter is loaded successfully; otherwise false</returns>
	public virtual bool LoadBibleChapter(BibleDescriptor bible, BibleBook book = BibleBook.NULL, byte chapter = 0)
	{
		if (bible.RegionalName.Count == 0) { return false; }
		else if (__lock.Wait(5000))
		{
			try
			{
				if (book == BibleBook.NULL) { book = bible.RegionalName.First().Key; }
				if (bible != _cache.bible || book != _cache.book)
				{
					_cache.value = LoadBook(bible, book);
					_cache.book = book;
					if (bible != _cache.bible)
					{
						_cache.bible = _state.CurrentLoadedBible = bible;
						_searchList.Clear();
					}
				}
				if (chapter == 0) chapter = 1;
				else if (chapter > _cache.value.Chapter.Count) chapter = (byte)_cache.value.Chapter.Count;
				_state.ReadState.CurrentChapterContent = _cache.value[chapter];
				_state.ReadState.CurrentSelectedBook = book;
				_state.ReadState.CurrentSelectedChapter = chapter;
				_state.ReadState.CurrentBookChapterCount = (byte)_cache.value.Chapter.Count;
				_state.CurrentAppState = AppViewState.State.ChapterLoaded;
				OnUpdate?.Invoke(this, _state);
			}
			catch { return false; }
			finally { __lock.Release(); }
			return true;
		}
		else { return false; }
	}
	/// <summary>Fetches the available download links and populates the `_downloadLinks` List</summary>
	/// <returns>If an exception is caught, it is returned, otherwise returns null</returns>
	public virtual Exception? RefreshDownloadLinks()
	{
		Exception? exp = null;
		try
		{
			_downloadLinks.Clear();
			_downloadLinks.AddRange(
				BibleLink.GetAllUrlsFromWebsite(quiet_return: false));
			_state.ContentState.LastRefreshed = DateTime.Now;
			_state.CurrentAppState = AppViewState.State.DownloadLinksUpdated;
			OnUpdate?.Invoke(this, _state);
		}
		catch (Exception ex) { exp = ex; }
		return exp;
	}
	/// <summary>Deletes all configuration files for this app.<br/><br/>Close your application immiedatedly after this method is called</summary>
	/// <returns>True if all files were deleted successfully. Otherwise false</returns>
	public virtual bool DeleteAllData()
	{
		if (__lock.Wait(5000))
		{
			try
			{
				File.Delete(ConfigFile);
				foreach (BibleDescriptor b in _offlineBibles)
				{
					Directory.Delete(BibleDirectoryPath(b), true);
				}
				_offlineBibles.Clear();
				_state.CurrentAppState = AppViewState.State.Uninitialized;
				OnUpdate?.Invoke(this, _state);
				__lock.Dispose();
				__lock = null!; // Make lock unusable for this object
				return true;
			}
			catch { return false; }
			finally { __lock?.Release(); } // If process failed due to some exception, __lock will not be disposed, and thus it needs to be released
		}
		else { return false; }
	}
}