using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OCRApp.Models;
using OCRApp.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OCRApp;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LoginPage : Page
{
    public HomeViewModel VM { get; }

    public LoginPage(HomeViewModel vm)
    {
        VM = vm;
        this.InitializeComponent();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        var authenticator = TemporaryAuthenticator.Instance;
        if (authenticator.IsValidCredentials(UsernameTextBox.Text, PasswordTextBox.Password))
        {
            VM.LoggedInUsername = UsernameTextBox.Text;
            VM.ActivePage = new WelcomePage(VM);
        }
        else
        {
            var dialog = new ContentDialog()
            {
                Title = "Incorrect credentials",
                Content = "The entered username or password is not correct.",
                XamlRoot = this.XamlRoot,
                PrimaryButtonText = "Ok",
            };
            await dialog.ShowAsync();
        }
    }
}
