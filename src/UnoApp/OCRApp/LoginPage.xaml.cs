using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OCRApp.Models;
using OCRApp.ViewModels;

namespace OCRApp;

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
        try
        {
            if (await BackendConnector.LoginAsync(UsernameTextBox.Text, PasswordTextBox.Password))
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
        catch (Exception ex)
        {
            var dialog = new ContentDialog()
            {
                Title = "Unknown error",
                Content = ex.ToString(),
                XamlRoot = this.XamlRoot,
                PrimaryButtonText = "Ok",
            };
            await dialog.ShowAsync();
        }
    }

    private void SignupHyperlink_Click(object sender, RoutedEventArgs e)
    {
        VM.ActivePage = new SignUpPage(VM);
    }
}
