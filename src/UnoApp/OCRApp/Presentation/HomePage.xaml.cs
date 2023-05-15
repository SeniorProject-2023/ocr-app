using Microsoft.UI.Xaml.Controls;
using OCRApp.Presentation;

namespace OCRApp;

internal sealed partial class HomePage : Page
{
    public HomeViewModel? VM => DataContext as HomeViewModel;

    public HomePage()
    {
        this.InitializeComponent();
        // Workaround. This should not be needed.
        DataContextChanged += (_, _) =>
        {
            if (VM is not null)
                Bindings.Update();
        };
    }
}
