
using CybersecurityAwarenessBot.Services;

namespace CybersecurityAwarenessBot.UI;

internal sealed class TaskForm : Form
{
    private readonly TaskStore store;
    private readonly Action<string> record;

    private readonly TextBox title = new()
    {
        Width = 245,
        MaxLength = 150,
        PlaceholderText = "Task title"
    };

    private readonly TextBox description = new()
    {
        Width = 375,
        MaxLength = 1000,
        PlaceholderText = "Description"
    };

    private readonly DateTimePicker date = new()
    {
        Width = 190,
        Format = DateTimePickerFormat.Custom,
        CustomFormat = "yyyy-MM-dd HH:mm",
        ShowCheckBox = true
    };

    private readonly Label status = new()
    {
        Dock = DockStyle.Fill,
        AutoEllipsis = true
    };

    private readonly FlowLayoutPanel actions = new()
    {
        Dock = DockStyle.Fill
    };

    private readonly DataGridView grid = new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AutoGenerateColumns = false,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        MultiSelect = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        RowHeadersVisible = false,
        BackgroundColor = Color.White
    };

    public TaskForm(
        TaskStore store,
        Action<string> record,
        string initialTitle = "")
    {
        this.store = store;
        this.record = record;

        Text = "Cybersecurity Task Assistant";
        ClientSize = new Size(900, 510);
        MinimumSize = new Size(740, 435);
        StartPosition = FormStartPosition.CenterParent;
        Font = new Font("Segoe UI", 10);
        BackColor = Color.FromArgb(241, 245, 249);

        title.Text = initialTitle;
        date.Value = DateTime.Now.AddDays(1);
        date.Checked = false;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 5,
            Padding = new Padding(16)
        };

        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));

        var details = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill
        };

        details.Controls.AddRange(new Control[]
        {
            title, description
        });

        var reminder = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill
        };

        reminder.Controls.AddRange(new Control[]
        {
            new Label
            {
                Text = "Optional reminder:",
                AutoSize = true
            },
            date,
            MakeButton("Add task", Add)
        });

        layout.Controls.Add(details, 0, 0);
        layout.Controls.Add(reminder, 0, 1);
        layout.Controls.Add(grid, 0, 2);
        layout.Controls.Add(actions, 0, 3);
        layout.Controls.Add(status, 0, 4);

        Controls.Add(layout);

        AddColumn("Title", nameof(SafetyTask.Title), 25);
        AddColumn("Description", nameof(SafetyTask.Description), 40);
        AddColumn("Reminder", nameof(SafetyTask.Reminder), 21);
        AddColumn("Status", nameof(SafetyTask.Status), 14);

        grid.DefaultCellStyle.WrapMode =
            DataGridViewTriState.True;

        grid.AutoSizeRowsMode =
            DataGridViewAutoSizeRowsMode.AllCells;

        actions.Controls.AddRange(new Control[]
        {
            MakeButton("Mark completed", () => Change(false)),
            MakeButton("Delete selected", () => Change(true)),
            MakeButton("Refresh", RefreshTasks)
        });

        Shown += async (_, _) => await Run(RefreshTasks);
    }

    private Button MakeButton(
        string text, Func<Task> action)
    {
        var button = new Button
        {
            Text = text,
            AutoSize = true
        };

        button.Click += async (_, _) => await Run(action);
        return button;
    }

    private void AddColumn(
        string heading, string property, float width)
    {
        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = heading,
            DataPropertyName = property,
            FillWeight = width
        });
    }

    private async Task Run(Func<Task> action)
    {
        var layout = Controls[0];

        if (!layout.Enabled)
            return;

        layout.Enabled = false;
        status.Text = "Working...";

        try
        {
            await action();
        }
        catch (Exception ex)
        {
            if (!IsDisposed)
                status.Text = "Error: " + ex.Message;
        }
        finally
        {
            if (!IsDisposed)
                layout.Enabled = true;
        }
    }

    private async Task RefreshTasks()
    {
        var tasks = await store.Read();

        if (IsDisposed)
            return;

        grid.DataSource = tasks;
        grid.ClearSelection();

        status.Text = $"{tasks.Count} task(s) found.";
    }

    private async Task Add()
    {
        string taskTitle = title.Text.Trim();
        string notes = description.Text.Trim();

        if (taskTitle.Length == 0 || notes.Length == 0)
        {
            status.Text = "Enter a title and description.";
            return;
        }

        DateTime? due = null;

        if (date.Checked)
        {
            if (date.Value <= DateTime.Now)
            {
                status.Text = "Choose a future reminder date.";
                return;
            }

            due = date.Value.ToUniversalTime();
        }

        await store.Add(taskTitle, notes, due);

        record($"Added task: {taskTitle}");

        if (due.HasValue)
        {
            record($"Reminder set for {taskTitle}");
        }

        title.Clear();
        description.Clear();
        date.Checked = false;

        await RefreshTasks();
    }

    private async Task Change(bool delete)
    {
        if (grid.SelectedRows.Count == 0 ||
            grid.SelectedRows[0].DataBoundItem is not SafetyTask task)
        {
            status.Text = "Select a task first.";
            return;
        }

        if (delete &&
            MessageBox.Show(
                $"Delete {task.Title}?",
                "Confirm",
                MessageBoxButtons.YesNo) != DialogResult.Yes)
        {
            return;
        }

        bool changed = delete
            ? await store.Delete(task.Id)
            : await store.Complete(task.Id);

        if (changed)
        {
            record(delete
                ? $"Deleted task: {task.Title}"
                : $"Completed task: {task.Title}");
        }

        await RefreshTasks();
    }
}

