using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using OCRApp.Business.Models;
using OCRApp.Services;

namespace OCRApp.ViewModels;

internal partial class HistoryViewModel : ObservableObject
{
    private readonly IOCRService _ocrService;

    /// <summary>
    /// The list of history items of the users.
    /// Each history items represent a single submission that can contain one or more outputs.
    /// </summary>
    [ObservableProperty]
    private IEnumerable<HistoryItemViewModel> _history = Array.Empty<HistoryItemViewModel>();

    [ObservableProperty]
    private HistoryItemViewModel? _activeHistoryItem = null;

    [ObservableProperty]
    private Visibility _rightGridVisibility = Visibility.Collapsed;

    [ObservableProperty]
    private int _selectedHistoryItemIndex = -1;

    [ObservableProperty]
    private int _selectedOutputIndex = -1;

    [ObservableProperty]
    private int _outputCount;

    public HistoryViewModel(IOCRService ocrService)
    {
        _ocrService = ocrService;
    }

    partial void OnSelectedHistoryItemIndexChanged(int value)
    {
        if (value >= 0 &&
            History.ToArray() is { Length: > 0} historyItems &&
            historyItems[value].Output.Length > 0)
        {
            ActiveHistoryItem = historyItems[value];
            OutputCount = ActiveHistoryItem.Output.Length;
            SelectedOutputIndex = 0;
            RightGridVisibility = Visibility.Visible;
        }
        else
        {
            ActiveHistoryItem = null;
            OutputCount = 0;
            SelectedOutputIndex = -1;
            RightGridVisibility = Visibility.Collapsed;
        }
    }

    internal void GoToPreviousOutput()
    {
        if (ActiveHistoryItem is null || SelectedOutputIndex <= 0)
        {
            return;
        }

        SelectedOutputIndex--;
    }

    internal void GoToNextOutput()
    {
        if (ActiveHistoryItem is null || SelectedOutputIndex >= ActiveHistoryItem.Output.Length - 1)
        {
            return;
        }

        SelectedOutputIndex++;
    }


    internal async Task LoadHistoryAsync()
    {
        History = (await _ocrService.GetHistoryAsync()).HistoryItems.Select(x => new HistoryItemViewModel(x));
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
