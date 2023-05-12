using Microsoft.UI.Xaml.Controls;
using OCRApp.Presentation;

namespace OCRApp;

internal sealed partial class HomePage : Page
{
    public HomeViewModel VM => (HomeViewModel)DataContext;

    public HomePage()
    {
        this.InitializeComponent();
    }
}
