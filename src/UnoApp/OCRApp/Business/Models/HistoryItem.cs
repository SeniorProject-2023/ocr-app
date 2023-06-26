using System;
using System.Text.Json.Serialization;

namespace OCRApp.Business.Models;

public class HistoryItem
{
    [JsonPropertyName("date_and_time")]
    public required DateTime DateTime { get; set; }

    [JsonPropertyName("elements")]
    public required string[] Output { get; set; }
}