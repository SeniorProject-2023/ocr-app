namespace OCRApp.Models;

internal interface IAuthenticator
{
    bool IsValidCredentials(string username, string password);
}
