using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OCRApp.ViewModels;

namespace OCRApp;

public sealed partial class TextScreenPage : Page{
    public HomeViewModel VM { get; }

    public TextScreenPage(HomeViewModel vm)
    {
        VM = vm;
        this.InitializeComponent();
    }


    private void SaveButton_Click(object sender , RoutedEventArgs e){
        
    }

}