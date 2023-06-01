using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using OCRApp.Services;
using Uno.Extensions.Navigation;
using Windows.Storage;

namespace OCRApp.Presentation;

internal sealed partial class MainViewModel : ObservableObject
{
    private readonly IOCRService _ocrService;
    private readonly INavigator _navigator;

    public MainViewModel(IOCRService ocrService, INavigator navigator)
    {
        _ocrService = ocrService;
        _navigator = navigator;
    }

    public string? LoggedInUsername => _ocrService.LoggedInUsername;

    internal async Task NavigateToResults(IEnumerable<string> results)
    {
        await _navigator.NavigateViewModelAsync<ResultsViewModel>(this, data: results);
    }

    internal async Task<IEnumerable<string>> SendImages(IEnumerable<Uri> images)
        => await _ocrService.SendImages(images);
}
