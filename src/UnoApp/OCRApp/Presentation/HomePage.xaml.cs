using Microsoft.UI.Xaml.Controls;
using OCRApp.ViewModels;
using Uno.Extensions.Navigation.UI.Controls;

namespace OCRApp;

internal sealed partial class HomePage : Page
{
    public HomeViewModel? VM => DataContext as HomeViewModel;

    public HomePage()
    {
        this.InitializeComponent();
    }

    private void DeleteCurrentImageButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (VM!.SelectedIndex < 0 || VM.ImagesToScan.Count - 1 < VM.SelectedIndex)
        {
            return;
        }

        VM.ImagesToScan.RemoveAt(VM.SelectedIndex);
    }

    private void MoveCurrentImageToLeftButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var selectedIndex = VM!.SelectedIndex;
        if (selectedIndex <= 0)
        {
            return;
        }

        
        VM.ImagesToScan.Move(selectedIndex, selectedIndex - 1);
        VM.SelectedIndex = selectedIndex - 1;
    }

    private void MoveCurrentImageToRightButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var selectedIndex = VM!.SelectedIndex;
        if (selectedIndex < 0 || selectedIndex >= VM.ImagesToScan.Count - 1)
        {
            return;
        }

        VM.ImagesToScan.Move(selectedIndex, selectedIndex + 1);
        VM.SelectedIndex = selectedIndex + 1;
    }
}
