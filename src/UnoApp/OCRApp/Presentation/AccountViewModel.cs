using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using OCRApp.Models;
using OCRApp.Services;

namespace OCRApp.Presentation;

internal sealed partial class AccountViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _loggedInUsername;

    private readonly IOCRService _ocrService;

    public AccountViewModel(IOCRService ocrService)
    {
        _ocrService = ocrService;
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
}
