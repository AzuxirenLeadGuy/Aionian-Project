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
public class AppViewModel<AppState>
where AppState : AppViewState, new()
{
	/// <summary>Fetches the data to populate the view</summary>
	public AppState State => _state;
	/// <summary>The data that can be used to populate the view</summary>
	protected AppState _state = new();
	/// <summary>The object to lock with</summary>
	protected SemaphoreSlim Lock = new(0, 1);
	/// <summary>The path of the config directory</summary>
	protected readonly string ConfigDir;
	/// <summary>The path of the config file</summary>
	protected string ConfigFile => $"{ConfigDir}/config.json";
	/// <summary>The cache to load the chapterwise data from</summary>
	protected (BibleDescriptor bible, BibleBook book, Book value) _cache;
	/// <summary>The references that match the search parameters</summary>
	protected readonly List<BibleReference> _searchList = new();
	/// <summary>The cached list of the bibles available to download</summary>
	protected readonly List<Listing> _downloadLinks = new();
	/// <summary>Loads the App from the content data in the Config directory</summary>
	/// <param name="path">The path of the config directory</param>
	public AppViewModel(string path)
	{
		if (!Directory.Exists(path))
		{
			if (File.Exists(path))
				throw new ArgumentException("Expected path of root directory, and not the file!", nameof(path));
			else
				Directory.CreateDirectory(path);
		}
		ConfigDir = path;
		if (File.Exists(ConfigFile))
		{
			OfflineBibles = JsonSerializer.Deserialize<List<BibleDescriptor>>(
				File.ReadAllText(ConfigFile),
				DefaultOptions()
			) ?? new();
		}
		else { SaveAssetLog(); }
		_cache = (default, default, default);
		_state = new()
		{
			ContentState = new()
			{
				AvailableLinks = _downloadLinks,
				DownloadedBibles = OfflineBibles,
				Progress = 0,
				SelectedBible = null,
				SelectedLink = null,
			},
			SearchState = new()
			{
				AvailableBibles = OfflineBibles,
				SearchProgress = 0,
				SearchText = "",
				SelectedBible = default,
				SelectedMode = SearchMode.MatchAnyWord,
				FoundReferences = _searchList,
			},
			ReadState = new()
			{
				CurrentChapterContent = new(),
				CurrentLoadedBible = default,
				CurrentSelectedBook = 0,
				CurrentSelectedChapter = 1,
				LoadedBookNames = Array.Empty<string>(),
				LoadedBibles = OfflineBibles.Select(x => x.ToString()),
			},
		};
	}
	/// <summary>Loads the App from the content data in the Config directory taken as the Local Application Data folder</summary>
	public AppViewModel() : this(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)) { }
	/// <summary>The bibles available to the app</summary>
	protected List<BibleDescriptor> OfflineBibles = new();
	/// <summary>The path of the directory where the contents of the book is stored</summary>
	/// <param name="desc">The descriptor of the bible</param>
	/// <returns>The path of the resource</returns>
	protected string BibleDirectoryPath(BibleDescriptor desc) => $"{ConfigDir}/{desc.ToString().Replace(' ', '_')}";
	/// <summary>The path of the file where the contents of the book is stored</summary>
	/// <param name="desc">The descriptor of the bible</param>
	/// <param name="book">The book value to load</param>
	/// <returns>The path of the resource</returns>
	protected string BibleBookPath(BibleDescriptor desc, BibleBook book) => $"{BibleDirectoryPath(desc)}/{(byte)book}.json";
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
				OfflineBibles,
				DefaultOptions()
			));
		_state.ReadState.LoadedBibles = OfflineBibles.Select(x => x.ToString());
		_state.ContentState.DownloadedBibles = OfflineBibles;
		_state.SearchState.AvailableBibles = OfflineBibles;
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
		if (Lock.Wait(5000, cancellationToken))
		{
			try
			{
				ProgressMessageHandler handler = new(
					new HttpClientHandler() { AllowAutoRedirect = true });
				if (progress_updater != null)
				{
					handler.HttpReceiveProgress += (_, e)
						=> progress_updater.Report(e.ProgressPercentage);
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
				OfflineBibles.Add(desc);
				SaveAssetLog();
			}
			catch (Exception ex)
			{
				if (DirStore.Length != 0 && Directory.Exists(DirStore))
				{
					Directory.Delete(DirStore, true);
				}
				exception = ex;
			}
			finally { Lock.Release(); }
		}
		else { exception = new AccessViolationException(); }
		return exception;
	}
	/// <summary>Deletes a bible</summary>
	/// <param name="descriptor">The bible to delete</param>
	/// <returns>If the delete process was successful, returns true; otherwise false</returns>
	public virtual bool DeleteBible(BibleDescriptor descriptor)
	{
		if (Lock.Wait(5000))
		{
			bool result = OfflineBibles.Remove(descriptor);
			Lock.Release();
			if (result) { SaveAssetLog(); }
			return result;
		}
		else { return false; }
	}

	/// <summary>Fetches the entire book</summary>
	/// <param name="bible">The bible from which the book is to be loaded</param>
	/// <param name="book">The book to load</param>
	/// <returns>The book of the bible</returns>
	protected Book LoadBook(BibleDescriptor bible, BibleBook book) =>
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
		BibleBook start,
		BibleBook end,
		IProgress<float>? progress_updater = null,
		CancellationToken cancellationToken = default)
	{
		if (bible.RegionalName.Count == 0)
		{
			_searchList.Clear();
			return true;
		}
		if (_cache.bible != bible)
			LoadBibleChapter(bible);
		if (cancellationToken == default) cancellationToken = CancellationToken.None;
		if (Lock.Wait(5000, cancellationToken))
		{
			_searchList.Clear();
			BibleSearchRange range = new()
			{
				SearchBookCount = (byte)end - (byte)start + 1,
				SearchRange = Enumerable.Range((byte)start, (byte)end + 1).Select(x => (BibleBook)(byte)x),
				Fetcher = (x) => LoadBook(bible, x),
			};
			_searchList.AddRange(query.GetResults(
				range,
				onProgressUpdate: progress_updater,
				ct: cancellationToken));
			return true;
		}
		else { return false; }
	}
	/// <summary>Fetches/Loads the downloaded bible</summary>
	/// <param name="bible">The bible to load</param>
	/// <param name="book">The book to load. By default, loads the first book of the bible</param>
	/// <param name="chapter">The chapter to load. By default, loads the first chapter of the book</param>
	/// <returns>Returns true if the required bible chapter is loaded successfully; otherwise false</returns>
	protected virtual bool LoadBibleChapter(BibleDescriptor bible, BibleBook book = BibleBook.NULL, byte chapter = 0)
	{
		if (bible.RegionalName.Count == 0) { return false; }
		else if (Lock.Wait(5000))
		{
			try
			{
				Book value;
				if (chapter == 0) chapter = 1;
				if (book == BibleBook.NULL) { book = bible.RegionalName.First().Key; }
				if (bible == _cache.bible && book == _cache.book) { value = _cache.value; }
				else
				{
					value = LoadBook(bible, book);
					_cache.book = book;
					if (bible != _cache.bible)
					{
						_cache.bible = bible;
						_state.ReadState.LoadedBookNames = bible.RegionalName.Values;
						_state.SearchState.AvailableRanges = bible.RegionalName.Values;
						_state.SearchState.BookStart = (byte)bible.RegionalName.First().Key;
						_state.SearchState.BookEnd = (byte)bible.RegionalName.Last().Key;
						_state.SearchState.SelectedBible = bible;
					}
				}
				_state.ReadState.CurrentChapterContent = value[chapter];
				_state.ReadState.CurrentLoadedBible = bible;
				_state.ReadState.CurrentSelectedBook = book;
				_state.ReadState.CurrentSelectedChapter = chapter;
			}
			catch { return false; }
			finally { Lock.Release(); }
			return true;
		}
		else { return false; }
	}
}