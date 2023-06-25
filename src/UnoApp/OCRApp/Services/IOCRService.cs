using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OCRApp.Business.Models;
using OCRApp.Models;

namespace OCRApp.Services;

internal interface IOCRService
{
    public string? LoggedInUsername { get; }
    Task<SignupResult> SignupAsync(string username, string password);

    Task<bool> LoginAsync(string username, string password);

    void Logout();

    Task<string> SendImages(IEnumerable<Uri> images);
    Task<IEnumerable<string>?> TryGetResultsForJobIdAsync(string jobId);

    public Task<History> GetHistoryAsync();
}
