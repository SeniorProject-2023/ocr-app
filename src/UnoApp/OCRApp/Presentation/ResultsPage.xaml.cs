﻿using Microsoft.UI.Xaml.Controls;

namespace OCRApp.Presentation;

public sealed partial class ResultsPage : Page
{
    internal ResultsViewModel? VM => DataContext as ResultsViewModel;

    public ResultsPage()
    {
        // Workaround.
        // Generally, I think Loaded should be used instead of DataContextChanged, and the VM null check shouldn't be needed.
        this.DataContextChanged += (_, _) =>
        {
            if (VM is null)
            {
                return;
            }

            Bindings.Update();
        };

        this.InitializeComponent();
    }

    private void Previous_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (flipView.SelectedIndex > 0)
        {
            flipView.SelectedIndex--;
        }
    }

    private void Next_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (flipView.SelectedIndex < flipView.Items.Count - 1)
        {
            flipView.SelectedIndex++;
        }
    }

    private string PlusOne(int selectedIndex)
        => (selectedIndex + 1).ToString();
}
