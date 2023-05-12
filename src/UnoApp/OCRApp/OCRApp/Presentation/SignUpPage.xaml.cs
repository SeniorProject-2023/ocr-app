using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OCRApp.Models;
using OCRApp.Presentation;

namespace OCRApp;

public sealed partial class SignUpPage : Page
{
    public HomeViewModel VM { get; }

    public SignUpPage(HomeViewModel vm)
    {
        VM = vm;
        this.InitializeComponent();
    }

    private async void SignupButton_Click(object sender, RoutedEventArgs e)
    {
        if (PasswordTextBox.Password != ConfirmPasswordTextBox.Password)
        {
            SignupInfoBar.Message = "Password and Confirm Password fields don't match";
            SignupInfoBar.IsOpen = true;
            return;
        }

        var result = await BackendConnector.SignupAsync(UsernameTextBox.Text, PasswordTextBox.Password);
        if (!result.Success)
        {
            SignupInfoBar.Message = result.Error!;
            SignupInfoBar.IsOpen = true;
            return;
        }

        var dialog = new ContentDialog()
        {
            Title = "Account created successfully",
            Content = "You can now login with your new account!",
            XamlRoot = this.XamlRoot,
            PrimaryButtonText = "Ok",
        };

        await dialog.ShowAsync();
        VM.ActivePage = new LoginPage(VM);
    }

    private void LoginHyperLink_Click(object sender, RoutedEventArgs e)
    {
        VM.ActivePage = new LoginPage(VM);
    }
}
