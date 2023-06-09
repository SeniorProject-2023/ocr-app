﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OCRApp.ViewModels;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace OCRApp.Presentation;

public sealed partial class HistoryPage : Page
{
    internal HistoryViewModel VM => (DataContext as HistoryViewModel)!;

    public HistoryPage()
    {
        this.DataContextChanged += HistoryPage_DataContextChanged;
        this.InitializeComponent();
    }

    private async void HistoryPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        if (VM is HistoryViewModel)
        {
            await RefreshAsync();
        }
    }

    private void Previous_Click(object sender, RoutedEventArgs e)
    {
        VM.GoToPreviousOutput();
    }

    private void Next_Click(object sender, RoutedEventArgs e)
    {
        VM.GoToNextOutput();
    }

    private async void GoBack_Click(object sender, RoutedEventArgs e)
    {
        await VM.GoBack();
    }

    private async Task RefreshAsync()
    {
        try
        {
            LoadingControl.IsLoading = true;
            await VM.LoadHistoryAsync();
        }
        catch (Exception ex)
        {
            var dialog = new ContentDialog()
            {
                Title = "Error retrieving history",
                Content = ex.Message,
                XamlRoot = this.XamlRoot,
                PrimaryButtonText = "Ok",
            };

            await dialog.ShowAsync();
        }
        finally
        {
            LoadingControl.IsLoading = false;
        }
    }

    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        await RefreshAsync();
    }

    private string PlusOne(int selectedIndex)
        => (selectedIndex + 1).ToString();

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (myListView.ItemsSource is not IEnumerable<HistoryItemViewModel> items)
        {
            return;
        }

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
            await FileIO.WriteTextAsync(saveFile, string.Join(Environment.NewLine, items));
            await CachedFileManager.CompleteUpdatesAsync(saveFile);
        }
    }
}
