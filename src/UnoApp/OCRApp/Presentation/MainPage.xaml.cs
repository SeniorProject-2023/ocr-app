using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OCRApp.Presentation;
using OCRApp.ViewModels;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace OCRApp;

internal sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    public MainViewModel VM => (MainViewModel)DataContext;

    private async void SelectFileButton_Click(object sender, RoutedEventArgs e)
    {
        if (VM.LoggedInUsername == null)
        {
            await AskToLogin();
            return;
        }

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
        await AddStorageFileAsync(file);
    }

    private async Task AddStorageFileAsync(StorageFile? file)
    {
        if (file != null)
        {
            var fileName = $"{Guid.NewGuid()}.jpg";
            await file.CopyAsync(ApplicationData.Current.LocalFolder, fileName);
            var uri = new Uri($"ms-appdata:///Local/{fileName}");
            VM.SendNewImageMessage(uri);
            VM.SendSetSelectedIndexMessage();
        }
    }

    private async Task AskToLogin()
    {
        var dialog = new ContentDialog()
        {
            Title = "Authentication required",
            Content = "You need to login first.",
            XamlRoot = this.XamlRoot,
            PrimaryButtonText = "Ok",
        };
        await dialog.ShowAsync();
    }

    private async void DoneButton_Click(object sender, RoutedEventArgs e)
    {
        if (VM.LoggedInUsername is null)
        {
            await AskToLogin();
            return;
        }

        ObservableCollection<ImageWrapper> imagesToScan = VM.GetImagesToScan();
        if (imagesToScan.Count == 0)
        {
            var dialog = new ContentDialog()
            {
                Title = "No images to scan",
                Content = "You should first add some images to perform OCR on.",
                XamlRoot = this.XamlRoot,
                PrimaryButtonText = "Ok",
            };
            await dialog.ShowAsync();
            return;
        }

        LoadingControl.IsLoading = true;

        var sw = Stopwatch.StartNew();

        var jobId = await VM.SubmitJob(imagesToScan.Select(x => x.Image.UriSource));

        while (true)
        {
            if (await VM.TryGetResultsForJobIdAsync(jobId) is { } results)
            {
                await VM.NavigateToResults(results);
                break;
            }

            await Task.Delay(1000);
        }

        sw.Stop();
        Console.WriteLine(sw.Elapsed.ToString());
        global::System.Diagnostics.Debug.WriteLine(sw.Elapsed.ToString());

        LoadingControl.IsLoading = false;
    }

#if __ANDROID__ || __IOS__
    private async void CaptureFromCameraButton_Click(object sender, RoutedEventArgs e)
    {
        if (VM.LoggedInUsername == null)
        {
            await AskToLogin();
            return;
        }

        var captureUI = new CameraCaptureUI();

        var file = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);
        await AddStorageFileAsync(file);
    }
#endif
}
