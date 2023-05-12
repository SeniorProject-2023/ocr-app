using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using OCRApp.Services;

namespace OCRApp.Presentation;

internal sealed partial class HomeViewModel : ObservableObject
{
    private readonly IImageManagerService _imageManagerService;

    [ObservableProperty]
    private int _selectedIndex;

    public HomeViewModel(IImageManagerService imageManagerService)
    {
        _imageManagerService = imageManagerService;
    }

    public ObservableCollection<ImageWrapper> ImagesToScan => _imageManagerService.ImagesToScan;

    partial void OnSelectedIndexChanged(int oldValue, int newValue)
    {
        if (oldValue > -1 && oldValue < ImagesToScan.Count)
        {
            ImagesToScan[oldValue].Visibility = Visibility.Collapsed;
        }

        if (newValue > -1 && newValue < ImagesToScan.Count)
        {
            ImagesToScan[newValue].Visibility = Visibility.Visible;
        }
    }
}
