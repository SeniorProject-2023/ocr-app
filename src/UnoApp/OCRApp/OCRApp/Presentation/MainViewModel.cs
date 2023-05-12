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
    private readonly IImageManagerService _imageManagerService;

    public MainViewModel(IOCRService ocrService, IImageManagerService imageManagerService)
    {
        _ocrService = ocrService;
        _imageManagerService = imageManagerService;
    }

    public string? LoggedInUsername => _ocrService.LoggedInUsername;

    public ObservableCollection<ImageWrapper> ImagesToScan => _imageManagerService.ImagesToScan;

    internal async Task<string> SendImages(IEnumerable<Uri> images)
        => await _ocrService.SendImages(images);
}
