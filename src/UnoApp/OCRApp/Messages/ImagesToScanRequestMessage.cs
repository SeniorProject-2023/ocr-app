using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging.Messages;
using OCRApp.Presentation;
using OCRApp.ViewModels;

namespace OCRApp.Messages;

internal class ImagesToScanRequestMessage : RequestMessage<ObservableCollection<ImageWrapper>>
{
}
