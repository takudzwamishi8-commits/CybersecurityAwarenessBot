
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CybersecurityAwarenessBot.Services;

internal enum IntentKind
{
    Advice,
    Tasks,
    AddTask,
    Reminder,
    Quiz,
    Activity
}

internal sealed record ChatIntent(
    IntentKind Kind,
    string Title = "",
    string When = "")
{
    private const RegexOptions Options =
        RegexOptions.IgnoreCase |
        RegexOptions.CultureInvariant;

    private const string TimePattern =
        @"(?:in\s+)?\d+\s+(?:minutes?|hours?|days?|weeks?)|tomorrow|\d{4}-\d{2}-\d{2}(?:\s+\d{2}:\d{2})?";

    public static ChatIntent Parse(string text)
    {
        string message = Regex.Replace(
            text.Trim().TrimEnd('.', '?', '!'),
            @"\s+", " ");

        message = Regex.Replace(
            message,
            @"^(?:(?:can|could|would) you |please )+",
            "",
            Options);

        if (Regex.IsMatch(message,
            @"\b(?:activity log|recent actions|what have you done)\b",
            Options))
        {
            return new ChatIntent(IntentKind.Activity);
        }

        if (Regex.IsMatch(message,
            @"\b(?:quiz|test my knowledge|mini.game)\b",
            Options))
        {
            return new ChatIntent(IntentKind.Quiz);
        }

        if (Regex.IsMatch(message,
            @"^(?:show|view|open|list)?\s*(?:my\s+)?(?:tasks|task list|task assistant)$",
            Options))
        {
            return new ChatIntent(IntentKind.Tasks);
        }

        var reminder = Regex.Match(message,
            @"^(?:remind me|set (?:a )?reminder)" +
            @"(?:\s+(?:to|for))?\s*(?<body>.*)$",
            Options);

        if (reminder.Success)
        {
            string body = reminder.Groups["body"].Value;

            var time = Regex.Match(
                body,
                $@"(?:^|\s)(?<when>{TimePattern})$",
                Options);

            if (time.Success)
            {
                string title = body[..time.Index].Trim();
                return new ChatIntent(
                    IntentKind.Reminder,
                    title,
                    time.Groups["when"].Value);
            }

            return new ChatIntent(
                IntentKind.Reminder,
                body.Trim(),
                "");
        }

        var task = Regex.Match(message,
            @"^(?:add|create|make)\s+(?:a\s+)?(?:new\s+)?" +
            @"(?:task|to-do)(?:\s+to|\s+called|\s+named)?\s*(?<title>.*)$",
            Options);

        if (task.Success)
        {
            return new ChatIntent(
                IntentKind.AddTask,
                task.Groups["title"].Value.Trim());
        }

        if (TryDue(message, DateTime.Now, out _))
        {
            return new ChatIntent(
                IntentKind.Reminder,
                "",
                message);
        }

        return new ChatIntent(IntentKind.Advice);
    }

    public static bool TryDue(
        string text,
        DateTime now,
        out DateTime dueUtc)
    {
        dueUtc = default;
        text = text.Trim().ToLowerInvariant();

        if (text.StartsWith("on "))
            text = text[3..];

        DateTime due;

        if (text == "tomorrow")
        {
            due = now.Date.AddDays(1).AddHours(9);
        }
        else
        {
            var relative = Regex.Match(
                text,
                @"^(?:in\s+)?(\d{1,3})\s+(minutes?|hours?|days?|weeks?)$");

            if (relative.Success)
            {
                int count = int.Parse(
                    relative.Groups[1].Value);

                string unit = relative.Groups[2].Value;

                due = unit.StartsWith("minute")
                    ? now.AddMinutes(count)
                    : unit.StartsWith("hour")
                    ? now.AddHours(count)
                    : unit.StartsWith("week")
                    ? now.AddDays(count * 7)
                    : now.AddDays(count);
            }
            else if (DateTime.TryParseExact(
                text,
                new[] { "yyyy-MM-dd", "yyyy-MM-dd HH:mm" },
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime date))
            {
                due = text.Length == 10
                    ? date.AddHours(9)
                    : date;
            }
            else
            {
                return false;
            }
        }

        if (due <= now)
            return false;

        dueUtc = due.ToUniversalTime();
        return true;
    }
}
