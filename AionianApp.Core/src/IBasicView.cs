using System;
namespace AionianApp;

/// <summary>A View prepared for a given type of ViewModel</summary>
/// <typeparam name="ViewState">The type of ViewState with which this View gets updated</typeparam>
public interface IBasicView<ViewState>
where ViewState : struct
{
    /// <summary>
    /// Given the current ViewState, update the view
    /// to match the current ViewState. This should
    /// be called by the viewmodel itself
    /// </summary>
    /// <param name="state">The current ViewState</param>
    void UpdateView(ViewState state);
}
