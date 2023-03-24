using Microsoft.UI.Xaml.Controls;
using OCRApp.ViewModels;

namespace OCRApp;

public sealed partial class HomePage : Page
{
    public HomeViewModel VM { get; }

    public HomePage(HomeViewModel vm)
    {
        VM = vm;
        this.InitializeComponent();
    }
}
