
using CybersecurityAwarenessBot.Services;

namespace CybersecurityAwarenessBot.UI;

internal sealed class ActivityForm : Form
{
    private const int PageSize = 8;

    public ActivityForm(ActivityLog log)
    {
        Text = "Cybersecurity Activity Log";
        ClientSize = new Size(750, 405);
        StartPosition = FormStartPosition.CenterParent;
        Font = new Font("Segoe UI", 11);
        BackColor = Color.White;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            Padding = new Padding(18)
        };

        layout.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 32));

        layout.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        layout.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 46));

        var range = new Label
        {
            AutoSize = true
        };

        var list = new ListBox
        {
            Dock = DockStyle.Fill,
            HorizontalScrollbar = true
        };

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill
        };

        var previous = new Button
        {
            Text = "Previous",
            AutoSize = true
        };

        var next = new Button
        {
            Text = "Show more",
            AutoSize = true
        };

        buttons.Controls.AddRange(new Control[]
        {
            previous, next
        });

        layout.Controls.Add(range, 0, 0);
        layout.Controls.Add(list, 0, 1);
        layout.Controls.Add(buttons, 0, 2);

        Controls.Add(layout);

        int page = 0;

        previous.Click += (_, _) =>
        {
            page--;
            Render();
        };

        next.Click += (_, _) =>
        {
            page++;
            Render();
        };

        Render();

        void Render()
        {
            list.Items.Clear();

            list.Items.AddRange(
                log.Page(page, PageSize).ToArray());

            range.Text = log.Count == 0
                ? "No activity recorded yet."
                : $"Actions {page * PageSize + 1}–" +
                  $"{page * PageSize + list.Items.Count} " +
                  $"of {log.Count}";

            previous.Enabled = page > 0;

            next.Enabled =
                (page + 1) * PageSize < log.Count;
        }
    }
}

