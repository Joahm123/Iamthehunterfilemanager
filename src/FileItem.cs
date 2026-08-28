using System;
using System.IO;

namespace MyFileManager;

public class FileItem
{
    public string Name { get; }
    public string FullPath { get; }
    public bool IsDirectory { get; }
    public string Type { get; }
    public string Size { get; }
    public DateTime Modified { get; }

    public FileItem(string path)
    {
        FullPath = path;
        Name = Path.GetFileName(path);

        if (string.IsNullOrEmpty(Name))
            Name = path;

        IsDirectory = Directory.Exists(path);

        if (IsDirectory)
        {
            Type = "Folder";
            Size = "";
            Modified = Directory.GetLastWriteTime(path);
        }
        else
        {
            Type = Path.GetExtension(path).ToUpperInvariant();

            if (string.IsNullOrEmpty(Type))
                Type = "File";

            try
            {
                Size = FormatSize(new FileInfo(path).Length);
                Modified = File.GetLastWriteTime(path);
            }
            catch
            {
                Size = "Unknown";
                Modified = DateTime.MinValue;
            }
        }
    }

    private static string FormatSize(long bytes)
    {
        if (bytes < 1024)
            return $"{bytes} B";

        if (bytes < 1024 * 1024)
            return $"{bytes / 1024.0:F1} KB";

        if (bytes < 1024L * 1024L * 1024L)
            return $"{bytes / (1024.0 * 1024):F1} MB";

        return $"{bytes / (1024.0 * 1024 * 1024):F1} GB";
    }
}
