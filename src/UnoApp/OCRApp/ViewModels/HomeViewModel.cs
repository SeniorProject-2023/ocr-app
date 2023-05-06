using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
namespace OCRApp.ViewModels;

public sealed class HomeViewModel : BindableBase
{
    public ObservableCollection<ImageWrapper> ImagesToScan { get; } = new();

    private Page? _activePage;
    private int _selectedIndex;
    private string? _loggedInUsername;
 
    

    public Page? ActivePage
    {
        get => _activePage;
        set => SetProperty(ref _activePage, value);
    }

    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            var oldSelected = _selectedIndex;
            SetProperty(ref _selectedIndex, value);

            if (oldSelected > -1 && oldSelected < ImagesToScan.Count)
            {
                ImagesToScan[oldSelected].Visibility = Visibility.Collapsed;
            }

            if (value > -1 && value < ImagesToScan.Count)
            {
                ImagesToScan[value].Visibility = Visibility.Visible;
            }
        }
    }

    public string? LoggedInUsername
    {
        get => _loggedInUsername;
        set
        {
            SetProperty(ref _loggedInUsername, value);
        }
    }

 
    public ObservableCollection<string> OcrResults { get; set; } = new();

    public ObservableCollection<string> OcrResult
    {
        get { return OcrResults; }
        set
        {
            OcrResults = value;
        }
    }
}
