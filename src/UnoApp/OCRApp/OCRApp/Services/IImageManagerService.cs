using System;
using System.Collections.ObjectModel;
using OCRApp.Presentation;

namespace OCRApp.Services;

internal interface IImageManagerService
{
    ObservableCollection<ImageWrapper> ImagesToScan { get; }
}
