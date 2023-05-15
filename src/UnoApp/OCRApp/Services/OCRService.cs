using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using OCRApp.Models;
using Windows.Storage.Streams;

namespace OCRApp.Services;

internal sealed class OCRService : IOCRService
{
#if DEBUG
    private const string BaseUri = "https://ocr2023.azurewebsites.net";
    //private const string BaseUri = "http://127.0.0.1:8000";
#else
    private const string BaseUri = "https://ocr2023.azurewebsites.net";
#endif

    private static HttpClient s_httpClient = new();
    private LoginResult _loginResult;

    public string? LoggedInUsername { get; private set; }

    public async Task<SignupResult> SignupAsync(string username, string password)
    {
        try
        {
            // TODO: DONT CREATE JSON LIKE THIS. USE PostAsyJsonAsync INSTEAD!!!
            var message = await s_httpClient.PostAsync($"{BaseUri}/users/signup/", new StringContent($$"""
            { "username": "{{username}}", "password": "{{password}}" }
            """, Encoding.UTF8, "application/json"));
            if (message.StatusCode != HttpStatusCode.OK)
            {
                return new SignupResult(false, await message.Content.ReadAsStringAsync());
            }

            return new SignupResult(true, null);
        }
        catch (Exception ex)
        {
            return new SignupResult(false, ex.ToString());
        }
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        // TODO: DONT CREATE JSON LIKE THIS. USE PostAsyJsonAsync INSTEAD!!!
        var message = await s_httpClient.PostAsync($"{BaseUri}/users/login/", new StringContent($$"""
            { "username": "{{username}}", "password": "{{password}}" }
            """, Encoding.UTF8, "application/json"));

        if (message.StatusCode != HttpStatusCode.OK)
        {
            return false;
        }

        _loginResult = await message.Content.ReadFromJsonAsync<LoginResult>();
        LoggedInUsername = username;
        return true;
    }

    public async Task<string> SendImages(IEnumerable<Uri> images)
    {
        using var content = new MultipartFormDataContent();

        foreach (var image in images)
        {
            var random = RandomAccessStreamReference.CreateFromUri(image);
            var stream = await random.OpenReadAsync();
            content.Add(CreateFileContent(stream.AsStreamForRead(), "image.jpg", "image/jpeg"));
        }

        s_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _loginResult.Access);

        var message = await s_httpClient.PostAsync($"{BaseUri}/api/arabic-ocr/", content);

        return null!;
    }

    private static StreamContent CreateFileContent(Stream stream, string fileName, string contentType)
    {
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = "\"images\"",
            FileName = "\"" + fileName + "\""
        }; // the extra quotes are key here
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        return fileContent;
    }

    public void Logout()
    {
        LoggedInUsername = null;
        _loginResult = default;
    }
}
