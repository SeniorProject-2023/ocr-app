using System.Collections.ObjectModel;
using OCRApp.Presentation;

namespace OCRApp.Services;

internal sealed class ImageManagerService : IImageManagerService
{
    public ObservableCollection<ImageWrapper> ImagesToScan { get; } = new();
}
