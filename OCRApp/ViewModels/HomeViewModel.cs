using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;

namespace OCRApp.ViewModels;

public sealed class HomeViewModel : BindableBase
{
    public ObservableCollection<ByteArrayWrapper> ImagesToScan { get; } = new();

    private Page? _activePage;
    private int _selectedIndex;

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
            SetProperty(ref _selectedIndex, value);
            foreach (var image in ImagesToScan)
            {
                image.NotifyVisibilityChanged();
            }
        }
    }
}
