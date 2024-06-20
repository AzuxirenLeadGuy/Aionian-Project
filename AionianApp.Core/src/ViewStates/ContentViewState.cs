using Aionian;
using System;
using System.Collections.Generic;

namespace AionianApp.ViewStates;
/// <summary>Represents the ViewState of content management of the app</summary>
public struct ContentViewState
{
	/// <summary>The links available to download</summary>
	public IEnumerable<Listing> AvailableLinks { internal set; get; }
	/// <summary>If true, the download link is loaded</summary>
	public DateTime LastRefreshed { internal set; get; }
}
