using Microsoft.UI.Xaml.Controls;
using Uno.Extensions.Hosting;
using Uno.Toolkit.UI;

namespace OCRApp.Presentation
{
    public sealed partial class Shell : UserControl, IContentControlProvider
    {
        public Shell()
        {
            this.InitializeComponent();
        }

        public ContentControl ContentControl => Splash;
    }
}