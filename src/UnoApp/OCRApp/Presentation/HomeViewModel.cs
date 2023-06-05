using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using OCRApp.Messages;

namespace OCRApp.Presentation;

internal sealed partial class HomeViewModel : ObservableObject
{
    [ObservableProperty]
    private int _selectedIndex;

    public HomeViewModel(IMessenger messenger)
    {
        messenger.Register<SetSelectedIndexMessage>(this, (_, m) => SelectedIndex = m.NewSelectedIndex);
        messenger.Register<HomeViewModel, ImagesToScanRequestMessage>(this, (r, m) => m.Reply(r.ImagesToScan));
        messenger.Register<NewImageMessage>(this, (_, m) => ImagesToScan.Add(new ImageWrapper(m.ImageUri)));
        _imagesToScan = new();
    }

    [ObservableProperty]
    private ObservableCollection<ImageWrapper> _imagesToScan;

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
