using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using OCRApp.Services;

namespace OCRApp.Presentation;

internal sealed partial class MainViewModel : ObservableObject
{
    private readonly IOCRService _ocrService;

    public MainViewModel(IOCRService ocrService)
    {
        _ocrService = ocrService;
    }

    public string? LoggedInUsername => _ocrService.LoggedInUsername;

    internal async Task<string> SendImages(IEnumerable<Uri> images)
        => await _ocrService.SendImages(images);
}
