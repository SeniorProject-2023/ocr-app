using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OCRApp.Presentation;

namespace OCRApp;

public sealed partial class WelcomePage : Page
{
    public HomeViewModel VM { get; }

    public WelcomePage(HomeViewModel vm)
    {
        VM = vm;
        this.InitializeComponent();
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        VM.LoggedInUsername = null;
        VM.ActivePage = new LoginPage(VM);
    }
}
