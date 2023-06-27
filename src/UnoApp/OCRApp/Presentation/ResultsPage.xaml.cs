using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OCRApp.ViewModels;
using Uno.Extensions.Navigation.UI.Controls;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace OCRApp.Presentation;

public sealed partial class ResultsPage : Page
{
    internal ResultsViewModel? VM => DataContext as ResultsViewModel;

    public ResultsPage()
    {
        this.InitializeComponent();
    }

    private void Previous_Click(object sender, RoutedEventArgs e) => VM!.GoPrevious();

    private void Next_Click(object sender, RoutedEventArgs e) => VM!.GoNext();

    private string PlusOne(int selectedIndex)
        => (selectedIndex + 1).ToString();

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var fileSavePicker = new FileSavePicker
        {
            SuggestedFileName = "ocr-result.txt",
        };

        fileSavePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
        // For Uno.WinUI-based apps
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(fileSavePicker, hwnd);
        StorageFile saveFile = await fileSavePicker.PickSaveFileAsync();
        if (saveFile != null)
        {
            CachedFileManager.DeferUpdates(saveFile);
            // Save file was picked, you can now write in it
            await FileIO.WriteTextAsync(saveFile, string.Join(Environment.NewLine, VM!.Results));
            await CachedFileManager.CompleteUpdatesAsync(saveFile);
        }
    }
}
