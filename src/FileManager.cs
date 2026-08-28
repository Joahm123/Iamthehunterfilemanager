using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MyFileManager;

public static class FileManager
{
    public static List<FileItem> GetItems(string path)
    {
        var items = new List<FileItem>();

        if (!Directory.Exists(path))
            return items;

        try
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                try
                {
                    items.Add(new FileItem(directory));
                }
                catch
                {
                    // Ignore inaccessible folders.
                }
            }

            foreach (string file in Directory.GetFiles(path))
            {
                try
                {
                    items.Add(new FileItem(file));
                }
                catch
                {
                    // Ignore inaccessible files.
                }
            }
        }
        catch
        {
            // Directory may require permissions.
        }

        return items
            .OrderByDescending(x => x.IsDirectory)
            .ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static void Open(string path)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }
        catch
        {
            // File could not be opened.
        }
    }
}
