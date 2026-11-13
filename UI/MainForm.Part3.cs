
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CybersecurityAwarenessBot.Services;

namespace CybersecurityAwarenessBot.UI;

public sealed partial class MainForm
{
    private readonly TaskStore part3TaskStore = new();
    private readonly ActivityLog part3Activity = new();

    private readonly System.Windows.Forms.Timer part3Timer =
        new() { Interval = 15000 };

    private readonly HashSet<string> shownReminders = new();

    private long? lastPart3Task;
    private string lastPart3Title = "";
    private string pendingPart3Title = "";
    private bool checkingPart3Reminders;

    private void SetupPart3()
    {
        var root = Controls.OfType<TableLayoutPanel>()
            .FirstOrDefault();

        var sidebar = root?
            .Controls.OfType<FlowLayoutPanel>()
            .FirstOrDefault();

        if (sidebar == null)
            throw new InvalidOperationException(
                "Could not find the chatbot sidebar.");

        sidebar.AutoScroll = true;
        sidebar.WrapContents = false;
        sidebar.FlowDirection = FlowDirection.TopDown;

        var heading = new Label
        {
            Text = "PART 3 FEATURES",
            ForeColor = Color.White,
            Size = new Size(220, 30)
        };

        sidebar.Controls.Add(heading);

        var tasks = MakeButton("My Tasks", 220);
        tasks.Click += (_, _) => OpenPart3Tasks();
        sidebar.Controls.Add(tasks);

        var quiz = MakeButton("Cybersecurity Quiz", 220);
        quiz.Click += (_, _) => OpenPart3Quiz();
        sidebar.Controls.Add(quiz);

        var activity = MakeButton("Activity Log", 220);
        activity.Click += (_, _) => OpenPart3Activity();
        sidebar.Controls.Add(activity);

        sidebar.Controls.SetChildIndex(heading, 0);
        sidebar.Controls.SetChildIndex(tasks, 1);
        sidebar.Controls.SetChildIndex(quiz, 2);
        sidebar.Controls.SetChildIndex(activity, 3);

        sidebar.AutoScroll = true;
        sidebar.WrapContents = false;
        sidebar.FlowDirection = FlowDirection.TopDown;


        part3Timer.Tick += async (_, _) =>
            await CheckPart3Reminders();

        part3Timer.Start();

        Shown += async (_, _) =>
            await CheckPart3Reminders();

        FormClosed += (_, _) =>
            part3Timer.Dispose();
    }

    private void OpenPart3Tasks(string title = "")
    {
        using var form = new TaskForm(
            part3TaskStore,
            part3Activity.Record,
            title);

        form.ShowDialog(this);
    }

    private void OpenPart3Quiz()
    {
        part3Activity.Record("Started quiz");

        using var form = new QuizForm(
            part3Activity.Record);

        form.ShowDialog(this);
    }

    private void OpenPart3Activity()
    {
        part3Activity.Record("Viewed activity log");

        using var form = new ActivityForm(
            part3Activity);

        form.ShowDialog(this);
    }

    private async Task<string?> HandlePart3Message(
        string message)
    {
        ChatIntent intent = ChatIntent.Parse(message);

        try
        {
            switch (intent.Kind)
            {
                case IntentKind.Tasks:
                    OpenPart3Tasks();
                    return "Your Task Assistant is open.";

                case IntentKind.Quiz:
                    OpenPart3Quiz();
                    return "You can play the quiz again anytime.";

                case IntentKind.Activity:
                    OpenPart3Activity();
                    return "Your Activity Log is open.";

                case IntentKind.AddTask:
                    if (string.IsNullOrWhiteSpace(
                        intent.Title))
                    {
                        OpenPart3Tasks();
                        return "Enter your task details.";
                    }

                    if (intent.Title.Length > 150)
                        return "Task title is too long.";

                    await SavePart3Task(
                        intent.Title,
                        null);

                    return $"Task added: {intent.Title}. " +
                           "Would you like a reminder?";

                case IntentKind.Reminder:
                    return await SetPart3Reminder(intent);

                default:
                    return null;
            }
        }
        catch (Exception ex)
        {
            return "The action could not be completed: " +
                   ex.Message;
        }
    }

    private async Task SavePart3Task(
        string title,
        DateTime? due)
    {
        string description =
            "Cybersecurity action: " + title;

        long id = await part3TaskStore.Add(
            title,
            description,
            due);

        lastPart3Task = id;
        lastPart3Title = title;
        pendingPart3Title = "";

        part3Activity.Record(
            $"Added task: {title}");
    }

    private async Task<string> SetPart3Reminder(
        ChatIntent intent)
    {
        if (!string.IsNullOrWhiteSpace(intent.Title))
            pendingPart3Title = intent.Title;

        if (string.IsNullOrWhiteSpace(intent.When))
        {
            return "When should I remind you? " +
                   "Try tomorrow or in 3 days.";
        }

        if (!ChatIntent.TryDue(
            intent.When,
            DateTime.Now,
            out DateTime due))
        {
            return "Enter a future reminder date.";
        }

        if (!string.IsNullOrWhiteSpace(
            pendingPart3Title))
        {
            string title = pendingPart3Title;

            if (title.Length > 150)
                return "Task title is too long.";

            await SavePart3Task(title, due);

            part3Activity.Record(
                $"Set reminder for {title}");

            return $"Reminder set for {title} on " +
                   $"{due.ToLocalTime():yyyy-MM-dd HH:mm}.";
        }

        if (lastPart3Task.HasValue)
        {
            bool updated = await part3TaskStore.Remind(
                lastPart3Task.Value,
                due);

            if (updated)
            {
                part3Activity.Record(
                    $"Set reminder for {lastPart3Title}");

                return $"Reminder set for {lastPart3Title} " +
                       $"on {due.ToLocalTime():yyyy-MM-dd HH:mm}.";
            }
        }

        return "Please add a task before setting a reminder.";
    }

    private async Task CheckPart3Reminders()
    {
        if (checkingPart3Reminders)
            return;

        if (string.IsNullOrWhiteSpace(
            Environment.GetEnvironmentVariable(
                "CYBERBOT_MYSQL")))
        {
            return;
        }

        checkingPart3Reminders = true;

        try
        {
            var tasks = await part3TaskStore.Read();

            if (IsDisposed)
                return;

            foreach (var task in tasks)
            {
                if (task.Completed ||
                    task.Due == null ||
                    task.Due > DateTime.UtcNow)
                {
                    continue;
                }

                string key =
                    $"{task.Id}:{task.Due.Value.Ticks}";

                if (!shownReminders.Add(key))
                    continue;

                part3Activity.Record(
                    $"Reminder displayed: {task.Title}");

                AddMessage(
                    "Assistant",
                    $"REMINDER: {task.Title}\n" +
                    task.Description);
            }
        }
        catch (Exception)
        {
            // Database errors appear when using the task window.
        }
        finally
        {
            checkingPart3Reminders = false;
        }
    }
}
