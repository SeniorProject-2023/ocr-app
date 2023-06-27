using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Uno.Extensions.Specialized;

namespace OCRApp.ViewModels;

internal partial class ResultsViewModel : ObservableObject
{
    [ObservableProperty]
    private int _selectedIndex = 0;

    public IEnumerable<string> Results { get; }
    public int OutputCount { get; }

    public ResultsViewModel(IEnumerable<string> results)
    {
        Results = results;
        OutputCount = Results.Count();
    }

    public void GoNext()
    {
        if (SelectedIndex < OutputCount - 1)
        {
            SelectedIndex++;
        }
    }

    public void GoPrevious()
    {
        if (SelectedIndex > 0)
        {
            SelectedIndex--;
        }
    }
}
