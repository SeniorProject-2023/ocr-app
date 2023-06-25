using System;

namespace OCRApp.Business.Models;

public class HistoryItem
{
    public required DateTime DateTime { get; set; }
    public required string[] Output { get; set; }
}