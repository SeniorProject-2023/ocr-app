using System.Collections.ObjectModel;
using System.ComponentModel;
using OCRApp.Services;

namespace OCRApp.Presentation;

internal sealed partial class HomeViewModel : INotifyPropertyChanged
{
    private readonly IImageManagerService _imageManagerService;

    public event PropertyChangedEventHandler? PropertyChanged;

    public HomeViewModel(IImageManagerService imageManagerService)
    {
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


    public ObservableCollection<ImageWrapper> ImagesToScan => _imageManagerService.ImagesToScan;
}
