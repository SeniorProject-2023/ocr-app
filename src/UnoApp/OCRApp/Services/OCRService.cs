﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using OCRApp.Business.Models;
using OCRApp.Models;
using Windows.Storage.Streams;

#if __ANDROID__
using Android.Graphics;
#endif

namespace OCRApp.Services;

internal sealed class OCRService : IOCRService
{
    private sealed class SubmitJobResult
    {
        [JsonPropertyName("job_token")]
        public string JobToken { get; set; } = null!;
    }

#if DEBUG
    private const string BaseUri = "https://ocr2023.azurewebsites.net";
    //private const string BaseUri = "http://192.168.1.4:8000";
#else
    private const string BaseUri = "https://ocr2023.azurewebsites.net";
#endif

    private static HttpClient s_httpClient = new() { Timeout = TimeSpan.FromSeconds(60) };
    private LoginResult _loginResult;

    public string? LoggedInUsername { get; private set; }

    public async Task<SignupResult> SignupAsync(string username, string password)
    {
        // TODO: DONT CREATE JSON LIKE THIS. USE PostAsyJsonAsync INSTEAD!!!
        var message = await s_httpClient.PostAsync($"{BaseUri}/users/signup/", new StringContent($$"""
        { "username": "{{username}}", "password": "{{password}}" }
        """, Encoding.UTF8, "application/json")).ConfigureAwait(false);
        if (message.StatusCode != HttpStatusCode.OK)
        {
            return new SignupResult(false, await message.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        return new SignupResult(true, null);
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        // TODO: Remove. Have it temporarily for ease of debugging in certain scenarios.
        if (username == "admin" && password == "admin")
        {
            LoggedInUsername = username;
            _loginResult = new LoginResult("Fake", "Fake");
            return true;
        }

        // TODO: DONT CREATE JSON LIKE THIS. USE PostAsyJsonAsync INSTEAD!!!
        var message = await s_httpClient.PostAsync($"{BaseUri}/users/login/", new StringContent($$"""
            { "username": "{{username}}", "password": "{{password}}" }
            """, Encoding.UTF8, "application/json")).ConfigureAwait(false);

        if (message.StatusCode != HttpStatusCode.OK)
        {
            return false;
        }

        _loginResult = await message.Content.ReadFromJsonAsync<LoginResult>().ConfigureAwait(false);
        s_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _loginResult.Access);
        LoggedInUsername = username;
        return true;
    }

    private async Task<HttpResponseMessage> GetResponseMessageAfterRefreshIfNeededAsync(Func<Task<HttpResponseMessage>> getMessage)
    {
        var message = await getMessage().ConfigureAwait(false);
        if (message.StatusCode == HttpStatusCode.Unauthorized && await RefreshAsync().ConfigureAwait(false))
        {
            message = await getMessage().ConfigureAwait(false);
        }

        return message;

        async Task<bool> RefreshAsync()
        {
            s_httpClient.DefaultRequestHeaders.Authorization = null;
            var message = await s_httpClient.PostAsync($"{BaseUri}/users/refresh-token/", new StringContent($$"""
            { "refresh": "{{_loginResult.Refresh}}" }
            """, Encoding.UTF8, "application/json")).ConfigureAwait(false);

            var refreshResult = await message.Content.ReadFromJsonAsync<RefreshTokenResult>().ConfigureAwait(false);
            _loginResult.Access = refreshResult.Access;
            s_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _loginResult.Access);
            return message.StatusCode == HttpStatusCode.OK;
        }
    }

    public async Task<string> SendImages(IEnumerable<Uri> images)
    {
        using var content = new MultipartFormDataContent();

        foreach (var image in images)
        {
            var random = RandomAccessStreamReference.CreateFromUri(image);
            var stream = (await random.OpenReadAsync()).AsStreamForRead();
#if __ANDROID__
            if (stream.Length >= 300 * 1024 && await BitmapFactory.DecodeStreamAsync(stream).ConfigureAwait(false) is Bitmap bitmap)
            {
                bitmap = Bitmap.CreateScaledBitmap(bitmap, 1300, 1300, false)!;
                // https://stackoverflow.com/questions/9814206/how-to-change-a-android-graphics-bitmap-into-a-byte-array-in-c-sharp
                var memoryStream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 50, memoryStream);
                stream = memoryStream;
            }
#endif
            content.Add(CreateFileContent(stream, "image.jpg", "image/jpeg"));
        }

        var message = await GetResponseMessageAfterRefreshIfNeededAsync(() => s_httpClient.PostAsync($"{BaseUri}/api/arabic-ocr/", content)).ConfigureAwait(false);
        var jobResult = await message.Content.ReadFromJsonAsync<SubmitJobResult>().ConfigureAwait(false);
        return jobResult!.JobToken;
    }

    private static StreamContent CreateFileContent(Stream stream, string fileName, string contentType)
    {
        if (stream.CanSeek && stream.Position != 0)
        {
            // Defensive condition. It shouldn't be hit, but just in case.
            stream.Position = 0;
        }

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

    public async Task<IEnumerable<string>?> TryGetResultsForJobIdAsync(string jobId)
    {
        var message = await GetResponseMessageAfterRefreshIfNeededAsync(() => s_httpClient.PostAsync($"{BaseUri}/api/check-for-job/", new StringContent($$"""
                { "job_token": "{{jobId}}" }
                """))).ConfigureAwait(false);
        if (message.StatusCode == HttpStatusCode.Accepted)
        {
            return null;
        }

        var response = await message.Content.ReadFromJsonAsync<OCRResults>().ConfigureAwait(false);
        return response!.Results.Values;
    }

    public async Task<History> GetHistoryAsync()
    {
        var message = await GetResponseMessageAfterRefreshIfNeededAsync(() => s_httpClient.GetAsync($"{BaseUri}/api/history/")).ConfigureAwait(false);
        var response = await message.Content.ReadFromJsonAsync<HistoryItem[]>().ConfigureAwait(false);
        return new History
        {
            HistoryItems = response!,
        };
    }
}

