
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CybersecurityAwarenessBot.Services;

internal sealed class ActivityLog
{
    private readonly List<string> entries = new();

    private readonly string filePath = Path.Combine(
        Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData),
        "CybersecurityAwarenessBot",
        "activity.log");

    public ActivityLog()
    {
        try
        {
            if (File.Exists(filePath))
            {
                entries.AddRange(
                    File.ReadAllLines(filePath).Reverse());
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    public int Count => entries.Count;

    public void Record(string action)
    {
        string safeAction = action
            .Replace("\r", " ")
            .Replace("\n", " ");

        string entry =
            $"{DateTime.Now:yyyy-MM-dd HH:mm} | {safeAction}";

        entries.Insert(0, entry);

        try
        {
            Directory.CreateDirectory(
                Path.GetDirectoryName(filePath)!);

            File.AppendAllText(
                filePath,
                entry + Environment.NewLine);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    public IEnumerable<string> Page(int page, int pageSize)
    {
        return entries
            .Skip(page * pageSize)
            .Take(pageSize);
    }
}
