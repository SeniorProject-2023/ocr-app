using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using OCRApp.Messages;
using OCRApp.Services;
using Uno.Extensions.Navigation;

namespace OCRApp.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    private readonly IOCRService _ocrService;
    private readonly INavigator _navigator;
    private readonly IMessenger _messenger;

    public MainViewModel(IOCRService ocrService, INavigator navigator, IMessenger messenger)
    {
        _ocrService = ocrService;
        _navigator = navigator;
        _messenger = messenger;
    }

    public string? LoggedInUsername => _ocrService.LoggedInUsername;

    internal async Task NavigateToResults(IEnumerable<string> results)
    {
        await _navigator.NavigateViewModelAsync<ResultsViewModel>(this, data: results);
    }

    internal async Task<string> SubmitJob(IEnumerable<Uri> images)
        => await _ocrService.SendImages(images);

    internal void SendNewImageMessage(Uri uri)
    {
        _messenger.Send(new NewImageMessage(uri));
    }

    internal void SendSetSelectedIndexMessage()
    {
        ObservableCollection<ImageWrapper> imagesToScan = GetImagesToScan();
        _messenger.Send(new SetSelectedIndexMessage(imagesToScan.Count - 1));
    }

    internal ObservableCollection<ImageWrapper> GetImagesToScan()
    {
        return _messenger.Send<ImagesToScanRequestMessage>();
    }

    internal async Task<IEnumerable<string>?> TryGetResultsForJobIdAsync(string jobId)
        => await _ocrService.TryGetResultsForJobIdAsync(jobId);
}
