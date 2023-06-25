using System;
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

    [ObservableProperty]
    private History _history = null!;

    internal async Task LoadHistoryAsync()
    {
        await _ocrService.GetHistoryAsync();
    }
}
