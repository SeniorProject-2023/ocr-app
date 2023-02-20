using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace OCRApp;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    private async void SelectFileButton_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker()
        {
            FileTypeFilter =
            {
                ".jpg", ".png",
            },
        };

        // Get the current window's HWND by passing a Window object
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        // Associate the HWND with the file picker
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);


        StorageFile file = await picker.PickSingleFileAsync();
        if (file != null)
        {
            using var stream = await file.OpenReadAsync();
            var bitmapImage = new BitmapImage();
            await bitmapImage.SetSourceAsync(stream);
            ImageToScan.Source = bitmapImage;
        }
        else
        {
            ImageToScan.Source = null;
        }
    }

    private async void ScanButton_Click(object sender, RoutedEventArgs e)
    {
        if (ImageToScan.Source is null)
        {
            return;
        }

        var dialog = new ContentDialog()
        {
            Title = "Title",
            Content = "Content",
            XamlRoot = this.XamlRoot,
            PrimaryButtonText = "Ok",
        };
        await dialog.ShowAsync();
    }

#if __ANDROID__ || __IOS__
    private async void CaptureFromCameraButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var captureUI = new CameraCaptureUI();

            var file = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (file != null)
            {
                using var stream = await file.OpenReadAsync();
                var bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(stream);
                ImageToScan.Source = bitmapImage;
            }
            else
            {
                ImageToScan.Source = null;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }
#endif
}
