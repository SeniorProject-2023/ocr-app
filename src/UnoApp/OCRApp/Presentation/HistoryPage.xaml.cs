using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OCRApp.ViewModels;

namespace OCRApp.Presentation;

public sealed partial class HistoryPage : Page
{
    internal HistoryViewModel VM => (DataContext as HistoryViewModel)!;

    public HistoryPage()
    {
        this.DataContextChanged += HistoryPage_DataContextChanged;
        this.InitializeComponent();
        // TODO: Loading.IsLoading = true;
    }

    private async void HistoryPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        if (VM is HistoryViewModel vm)
        {
            await vm.LoadHistoryAsync();
        }
    }
}
