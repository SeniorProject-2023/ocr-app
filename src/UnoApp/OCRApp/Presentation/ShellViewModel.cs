using System.Threading.Tasks;
using Uno.Extensions.Navigation;

namespace OCRApp.Presentation
{
    public class ShellViewModel
    {
        private INavigator Navigator { get; }

        public ShellViewModel(INavigator navigator)
        {
            Navigator = navigator;

            _ = Start();
        }

        public async Task Start()
        {
            await Navigator.NavigateViewModelAsync<MainViewModel>(this);
        }
    }
}