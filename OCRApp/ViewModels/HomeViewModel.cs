using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

namespace OCRApp.ViewModels;

public sealed class HomeViewModel : BindableBase
{
    public ObservableCollection<BitmapImage> ImagesToScan { get; } = new();

    private Page _activePage;
    public Page ActivePage
    {
        get => _activePage;
        
        [MemberNotNull(nameof(_activePage))]
        set => SetProperty(ref _activePage!, value);
    }

    public HomeViewModel()
    {
        ActivePage = new HomePage(this);
    }
}
