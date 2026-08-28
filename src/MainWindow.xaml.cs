using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace MyFileManager;

public partial class MainWindow : Window
{
    private string _currentPath;
    private readonly Stack<string> _history = new();

    public MainWindow()
    {
        InitializeComponent();

        _currentPath = Environment.GetFolderPath(
            Environment.SpecialFolder.UserProfile);

        NavigateTo(_currentPath);
    }

    private void NavigateTo(string path, bool addHistory = true)
    {
        if (!Directory.Exists(path))
            return;

        if (addHistory && !string.Equals(
                _currentPath,
                path,
                StringComparison.OrdinalIgnoreCase))
        {
            _history.Push(_currentPath);
        }

        _currentPath = path;
        PathBox.Text = path;

        FileList.ItemsSource = FileManager.GetItems(path);

        BackButton.IsEnabled = _history.Count > 0;
    }

    private void FileList_MouseDoubleClick(
        object sender,
        MouseButtonEventArgs e)
    {
        if (FileList.SelectedItem is not FileItem item)
            return;

        if (item.IsDirectory)
        {
            NavigateTo(item.FullPath);
        }
        else
        {
            FileManager.Open(item.FullPath);
        }
    }

    private void BackButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_history.Count == 0)
            return;

        string previousPath = _history.Pop();

        NavigateTo(previousPath, false);

        BackButton.IsEnabled = _history.Count > 0;
    }

    private void UpButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            DirectoryInfo? parent =
                Directory.GetParent(_currentPath);

            if (parent != null)
                NavigateTo(parent.FullName);
        }
        catch
        {
            // No parent or access denied.
        }
    }

    private void RefreshButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        NavigateTo(_currentPath, false);
    }

    private void PathBox_KeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
            return;

        string path = PathBox.Text.Trim();

        if (Directory.Exists(path))
        {
            NavigateTo(path);
        }
        else if (File.Exists(path))
        {
            FileManager.Open(path);
        }
        else
        {
            MessageBox.Show(
                "That location doesn't exist.",
                "My File Manager",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            PathBox.Text = _currentPath;
        }
    }
}
