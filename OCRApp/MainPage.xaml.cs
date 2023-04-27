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
            await file.DeleteAsync();
            var uri = new Uri($"ms-appdata:///Local/{fileName}");
            VM.ImagesToScan.Add(new ImageWrapper(uri));
            VM.SelectedIndex = VM.ImagesToScan.Count - 1;
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

    private void DoneButton_Click(object sender, RoutedEventArgs e)
    {

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

        // Workaround https://github.com/unoplatform/uno/issues/11935
        var temp = this.content.Content;
        this.content.Content = null;
        this.content.Content = temp;
    }
#endif
}
