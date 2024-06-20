using System;
namespace AionianApp;

/// <summary>A basic Viewmodel type</summary>
/// <typeparam name="ViewState">The state of the Viewmodel is encapsulated in this type of object</typeparam>
public interface IViewModel<ViewState>
where ViewState : class
{
	/// <summary>This event allows views to get registered whenever the viewmodel is updated</summary>
	event EventHandler<ViewState>? OnUpdate;
}