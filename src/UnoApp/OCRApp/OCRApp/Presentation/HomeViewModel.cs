using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using OCRApp.Messages;

namespace OCRApp.Presentation;

internal sealed partial class HomeViewModel : ObservableObject
{
    [ObservableProperty]
    private int _selectedIndex;

    public HomeViewModel()
    {
        WeakReferenceMessenger.Default.Register<SetSelectedIndexMessage>(this, (_, m) => SelectedIndex = m.NewSelectedIndex);
        WeakReferenceMessenger.Default.Register<HomeViewModel, ImagesToScanRequestMessage>(this, (r, m) => m.Reply(r.ImagesToScan));
        WeakReferenceMessenger.Default.Register<NewImageMessage>(this, (_, m) => ImagesToScan.Add(new ImageWrapper(m.ImageUri)));
        _imagesToScan = new();
    }

    [ObservableProperty]
    private ObservableCollection<ImageWrapper> _imagesToScan;
}
