using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging.Messages;
using OCRApp.Presentation;

namespace OCRApp.Messages;

internal class ImagesToScanRequestMessage : RequestMessage<ObservableCollection<ImageWrapper>>
{
}
