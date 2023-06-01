using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;

namespace OCRApp.Presentation;

[Bindable(true)]
public sealed partial class ImageWrapper : ObservableObject
{
    private readonly Uri _uri;

    [ObservableProperty]
    private Visibility _visibility;

    public ImageWrapper(Uri uri)
    {
        _uri = uri;
    }

    public BitmapImage Image => new(_uri);
}
