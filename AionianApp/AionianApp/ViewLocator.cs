using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using AionianApp.ViewModels;

namespace AionianApp;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
            return null;
        string name = data
            .GetType()
            .FullName!
            .Replace(
                "ViewModel",
                "View",
                StringComparison.Ordinal);
        Type? type = Type.GetType(name);
        return type == null ?
            new TextBlock { Text = "Not Found: " + name } :
            Activator.CreateInstance(type) as Control;
    }
    public bool Match(object? data) => data is ViewModelBase;
}
