using System.Collections.Generic;

namespace OCRApp.ViewModels;

internal partial class ResultsViewModel
{
    public IEnumerable<string> Results { get; }

    public ResultsViewModel(IEnumerable<string> results)
    {
        Results = results;
    }
}
