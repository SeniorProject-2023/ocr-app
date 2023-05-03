using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

namespace OCRApp.Models;

internal record struct SignupResult(bool Success, string? Error);

internal record struct LoginResult(string Refresh, string Access);

internal static class BackendConnector
{
    private const string BaseUri = "http://127.0.0.1:8000";
    private static HttpClient s_httpClient = new();
    private static LoginResult s_loginResult;

    public static async Task<SignupResult> SignupAsync(string username, string password)
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

    public static async Task<bool> LoginAsync(string username, string password)
    {
        // TODO: DONT CREATE JSON LIKE THIS. USE PostAsyJsonAsync INSTEAD!!!
        var message = await s_httpClient.PostAsync($"{BaseUri}/users/login/", new StringContent($$"""
            { "username": "{{username}}", "password": "{{password}}" }
            """, Encoding.UTF8, "application/json"));

        if (message.StatusCode != HttpStatusCode.OK)
        {
            return false;
        }

        s_loginResult = await message.Content.ReadFromJsonAsync<LoginResult>();
        return true;
    }

    public static async Task<string> SendImages(IEnumerable<ImageWrapper> images)
    {
        using var content = new MultipartFormDataContent();

        foreach (var image in images)
        {
            var random = RandomAccessStreamReference.CreateFromUri(image.Image.UriSource);
            var stream = await random.OpenReadAsync();
            content.Add(CreateFileContent(stream.AsStreamForRead(), "image.jpg", "image/jpeg"));
        }

        s_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", s_loginResult.Access);

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
}
