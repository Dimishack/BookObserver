﻿using BookObserver.Services.Interfaces;
using System.Windows;

namespace BookObserver.Services
{
    internal class UserDialogService : IUserDialog
    {
        public void ShowInformation(string message, string? caption) =>
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);

        public bool ShowWarning(string message, string? caption = null) =>
            MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning)
            == MessageBoxResult.Yes;
    }
}
