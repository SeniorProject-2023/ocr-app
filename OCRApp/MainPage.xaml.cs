using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OCRApp.ViewModels;
using Uno.Toolkit.UI;
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

    public HomeViewModel VM { get; set; } = new();

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
        await AddStorageFileAsync(file);
    }

    private async Task AddStorageFileAsync(StorageFile? file)
    {
        if (file != null)
        {
            using var stream = await file.OpenStreamForReadAsync();
            var bytes = new byte[(int)stream.Length];
            stream.Read(bytes, 0, (int)stream.Length);

            VM.ImagesToScan.Add(new ByteArrayWrapper(bytes, VM));
            VM.SelectedIndex = VM.ImagesToScan.Count - 1;
        }
    }

    private async void ScanButton_Click(object sender, RoutedEventArgs e)
    {
        if (VM.ImagesToScan.Count == 0)
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

    private void TabBar_SelectionChanged(TabBar sender, TabBarSelectionChangedEventArgs args)
    {
        if (ReferenceEquals(args.NewItem, HomeTabBarItem))
        {
            VM.ActivePage = new HomePage(VM);
        }
        else if (ReferenceEquals(args.NewItem, AccountTabBarItem))
        {
            VM.ActivePage = VM.LoggedInUsername is null ? new LoginPage(VM) : new WelcomePage(VM);
        }
    }

#if __ANDROID__ || __IOS__
    private async void CaptureFromCameraButton_Click(object sender, RoutedEventArgs e)
    {
        var captureUI = new CameraCaptureUI();

        var file = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);
        await AddStorageFileAsync(file);

        // Workaround https://github.com/unoplatform/uno/issues/11935
        var temp = this.content.Content;
        this.content.Content = null;
        this.content.Content = temp;
    }
#endif
}
