using Aionian;
using System.Collections.Generic;

namespace AionianApp.ViewStates;
/// <summary>Represents the ViewState of content management of the app</summary>
public struct ContentViewState
{
	/// <summary>The links available to download</summary>
	public IEnumerable<Listing> AvailableLinks { internal set; get; }
	/// <summary>The bibles already downloaded</summary>
	public IEnumerable<BibleDescriptor> DownloadedBibles { internal set; get; }
	/// <summary>The Download progress in percent (out of 100). When not downloading, value is 0</summary>
	public float Progress { internal set; get; }
	/// <summary>Returns true if download is ongoing, otherwise false</summary>
	public readonly bool IsDownloading => Progress > 0;
	/// <summary>The link currently selected (if any)</summary>
	public Listing? SelectedLink { internal set; get; }
	/// <summary>The offline bible description currently selected (if any)</summary>
	public BibleDescriptor? SelectedBible { internal set; get; }
}
