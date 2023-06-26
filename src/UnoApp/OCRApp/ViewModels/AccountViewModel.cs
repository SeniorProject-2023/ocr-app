using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using OCRApp.Models;
using OCRApp.Services;
using Uno.Extensions.Navigation;

namespace OCRApp.ViewModels;

internal sealed partial class AccountViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _loggedInUsername;

    private readonly IOCRService _ocrService;
    private readonly INavigator _navigator;

    public AccountViewModel(IOCRService ocrService, INavigator navigator)
    {
        _ocrService = ocrService;
        _navigator = navigator;
    }

    internal async Task<SignupResult> SignupAsync(string username, string password)
        => await _ocrService.SignupAsync(username, password);

    internal async Task<bool> LoginAsync(string username, string password)
    {
        var result = await _ocrService.LoginAsync(username, password);
        LoggedInUsername = _ocrService.LoggedInUsername;
        return result;
    }

    internal void Logout()
    {
        _ocrService.Logout();
        Debug.Assert(_ocrService.LoggedInUsername is null);
        LoggedInUsername = null;
    }

    internal async Task NavigateToHistoryAsync()
    {
        await _navigator.NavigateViewModelAsync<HistoryViewModel>(this, "/");
    }
}
