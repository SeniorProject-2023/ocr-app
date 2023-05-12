using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace OCRApp.Presentation;

public sealed partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    private Page? _activePage;

    [ObservableProperty]
    private int _selectedIndex;

    [ObservableProperty]
    public string? _loggedInUsername;

    public ObservableCollection<ImageWrapper> ImagesToScan { get; } = new();

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
