using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using OCRApp.Services;

namespace OCRApp.Presentation;

internal sealed partial class MainViewModel : INotifyPropertyChanged
{
    private readonly IOCRService _ocrService;
    private readonly IImageManagerService _imageManagerService;

    public event PropertyChangedEventHandler? PropertyChanged;

    public MainViewModel(IOCRService ocrService, IImageManagerService imageManagerService)
    {
        _ocrService = ocrService;
        _imageManagerService = imageManagerService;
        imageManagerService.SelectedIndexChanged += OnSelectedIndexChanged;
    }

    private void OnSelectedIndexChanged(object? sender, (int OldValue, int newValue) e)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIndex)));

    public int SelectedIndex
    {
        get => _imageManagerService.SelectedIndex;
        set => _imageManagerService.SelectedIndex = value;
    }

    public string? LoggedInUsername => _ocrService.LoggedInUsername;

    public ObservableCollection<ImageWrapper> ImagesToScan => _imageManagerService.ImagesToScan;

    internal async Task<string> SendImages(IEnumerable<Uri> images)
        => await _ocrService.SendImages(images);
}
