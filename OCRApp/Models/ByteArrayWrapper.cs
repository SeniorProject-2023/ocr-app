using Microsoft.UI.Xaml;
using OCRApp.ViewModels;

namespace OCRApp;

public sealed class ByteArrayWrapper : BindableBase
{
    private readonly HomeViewModel _viewModel;

    public ByteArrayWrapper(byte[] bytes, HomeViewModel viewModel)
    {
        _viewModel = viewModel;
        Bytes = bytes;
    }

    public byte[] Bytes { get; }

    public Visibility Visibility => _viewModel.ImagesToScan[_viewModel.SelectedIndex].Bytes == this.Bytes ? Visibility.Visible : Visibility.Collapsed;

    internal void NotifyVisibilityChanged()
        => OnPropertyChanged(nameof(Visibility));
}
