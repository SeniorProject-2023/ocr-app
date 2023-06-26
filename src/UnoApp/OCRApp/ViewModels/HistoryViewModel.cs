using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using OCRApp.Business.Models;
using OCRApp.Services;

namespace OCRApp.ViewModels;

internal partial class HistoryViewModel : ObservableObject
{
    private readonly IOCRService _ocrService;

    public HistoryViewModel(IOCRService ocrService)
    {
        _ocrService = ocrService;
    }

    internal async Task<IEnumerable<HistoryItemViewModel>> GetHistoryAsync()
    {
        return (await _ocrService.GetHistoryAsync()).HistoryItems.Select(x => new HistoryItemViewModel(x));
    }
}

internal class HistoryItemViewModel
{
    public DateTime DateTime { get; }
    public string[] Output { get; }

    public string DisplayString => $"{Output.Length} output(s)";

    public HistoryItemViewModel(HistoryItem historyItem)
    {
        DateTime = historyItem.DateTime;
        Output = historyItem.Output;
    }
}
