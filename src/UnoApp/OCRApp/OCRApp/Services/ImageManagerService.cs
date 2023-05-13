using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using OCRApp.Presentation;

namespace OCRApp.Services;

internal sealed partial class ImageManagerService : ObservableObject, IImageManagerService
{
    public ImageManagerService()
    {    
    }

    public ObservableCollection<ImageWrapper> ImagesToScan { get; } = new();

    [ObservableProperty]
    private int _selectedIndex = -1;

    public event EventHandler<(int OldValue, int newValue)>? SelectedIndexChanged;

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

        SelectedIndexChanged?.Invoke(this, (oldValue, newValue));
    }
}
