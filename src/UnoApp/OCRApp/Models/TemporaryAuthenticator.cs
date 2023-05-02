namespace OCRApp.Models;

internal sealed class TemporaryAuthenticator : IAuthenticator
{
    public static IAuthenticator Instance { get; } = new TemporaryAuthenticator();

    public bool IsValidCredentials(string username, string password)
    {
        if (username == "admin" && password == "admin")
        {
            return true;
        }

        return false;
    }
}
