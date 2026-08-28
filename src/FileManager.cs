using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileManager
{
    public class FileManager
    {
        public string CurrentPath { get; private set; }

        private readonly List<string> history = new();
        private int historyIndex = -1;

        public FileManager()
        {
            CurrentPath = Environment.GetFolderPath(
                Environment.SpecialFolder.UserProfile
            );

            AddToHistory(CurrentPath);
        }

        public List<FileItem> GetItems()
        {
            var items = new List<FileItem>();

            if (!Directory.Exists(CurrentPath))
                return items;

            try
            {
                // Folders
                foreach (string folder in Directory.GetDirectories(CurrentPath))
                {
                    DirectoryInfo info = new DirectoryInfo(folder);

                    items.Add(new FileItem
                    {
                        Name = "📁 " + info.Name,
                        Type = "Folder",
                        Size = "",
                        Modified = info.LastWriteTime.ToString(),
                        FullPath = info.FullName
                    });
                }

                // Files
                foreach (string file in Directory.GetFiles(CurrentPath))
                {
                    FileInfo info = new FileInfo(file);

                    items.Add(new FileItem
                    {
                        Name = "📄 " + info.Name,
                        Type = GetFileType(info.Extension),
                        Size = FormatSize(info.Length),
                        Modified = info.LastWriteTime.ToString(),
                        FullPath = info.FullName
                    });
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore folders/files we don't have permission to access.
            }

            return items
                .OrderByDescending(x => x.Type == "Folder")
                .ThenBy(x => x.Name)
                .ToList();
        }

        public bool OpenFolder(string path)
        {
            if (!Directory.Exists(path))
                return false;

            CurrentPath = Path.GetFullPath(path);

            AddToHistory(CurrentPath);

            return true;
        }

        public bool GoBack()
        {
            if (!CanGoBack())
                return false;

            historyIndex--;

            CurrentPath = history[historyIndex];

            return true;
        }

        public bool GoForward()
        {
            if (!CanGoForward())
                return false;

            historyIndex++;

            CurrentPath = history[historyIndex];

            return true;
        }

        public bool GoUp()
        {
            DirectoryInfo? parent =
                Directory.GetParent(CurrentPath);

            if (parent == null)
                return false;

            return OpenFolder(parent.FullName);
        }

        public void GoHome()
        {
            string home = Environment.GetFolderPath(
                Environment.SpecialFolder.UserProfile
            );

            OpenFolder(home);
        }

        public void Refresh()
        {
            // Nothing needs to happen here.
            // GetItems() reads the folder again.
        }

        public bool CanGoBack()
        {
            return historyIndex > 0;
        }

        public bool CanGoForward()
        {
            return historyIndex < history.Count - 1;
        }

        public bool CanGoUp()
        {
            return Directory.GetParent(CurrentPath) != null;
        }

        private void AddToHistory(string path)
        {
            if (history.Count > 0 &&
                history[historyIndex] == path)
            {
                return;
            }

            if (historyIndex < history.Count - 1)
            {
                history.RemoveRange(
                    historyIndex + 1,
                    history.Count - historyIndex - 1
                );
            }

            history.Add(path);
            historyIndex = history.Count - 1;
        }

        private string GetFileType(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
                return "File";

            return extension
                .TrimStart('.')
                .ToUpperInvariant() + " File";
        }

        private string FormatSize(long bytes)
        {
            if (bytes < 1024)
                return $"{bytes} B";

            if (bytes < 1024 * 1024)
                return $"{bytes / 1024.0:F1} KB";

            if (bytes < 1024 * 1024 * 1024)
                return $"{bytes / (1024.0 * 1024):F1} MB";

            return $"{bytes / (1024.0 * 1024 * 1024):F1} GB";
        }
    }
}
