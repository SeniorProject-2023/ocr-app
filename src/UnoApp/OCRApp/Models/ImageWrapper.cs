using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using OCRApp.ViewModels;

namespace OCRApp;

public sealed class ImageWrapper : BindableBase
{
    private readonly Uri _uri;
    private Visibility _visibility;

    public ImageWrapper(Uri uri)
    {
        _uri = uri;
    }

    public BitmapImage Image => new(_uri);

    public Visibility Visibility
    {
        get => _visibility;
        set => SetProperty(ref _visibility, value);
    }
}
