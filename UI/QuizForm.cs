
namespace CybersecurityAwarenessBot.UI;

internal sealed class QuizForm : Form
{
    private sealed record Question(
        string Prompt,
        string[] Options,
        int Answer,
        string Explanation);

    private static readonly Question[] Questions =
    {
        new(
            "What should you do if an email asks for your password?",
            new[] {
                "Reply with your password",
                "Report it as phishing",
                "Forward it to friends"
            },
            1,
            "Never share passwords through suspicious emails."),

        new(
            "A familiar display name proves an email is genuine.",
            new[] { "True", "False" },
            1,
            "Scammers can copy names and email appearances."),

        new(
            "Which password habit is safest?",
            new[] {
                "Reuse the same password",
                "Use your birthday",
                "Use unique passwords"
            },
            2,
            "Unique passwords protect your other accounts."),

        new(
            "Password managers can store unique passwords.",
            new[] { "True", "False" },
            0,
            "Password managers help you manage strong passwords."),

        new(
            "How should you verify a suspicious parcel message?",
            new[] {
                "Click the message link",
                "Use the courier's official website",
                "Share your card details"
            },
            1,
            "Use official websites to verify delivery information."),

        new(
            "HTTPS alone proves a website is trustworthy.",
            new[] { "True", "False" },
            1,
            "Scam websites can also use HTTPS."),

        new(
            "Someone asks for your bank OTP. What should you do?",
            new[] {
                "Share it",
                "Send it once",
                "Refuse and contact your bank"
            },
            2,
            "Never share your one-time password."),

        new(
            "You should approve an unexpected login notification.",
            new[] { "True", "False" },
            1,
            "Unexpected login requests should be denied."),

        new(
            "Where should you download browser updates?",
            new[] {
                "Official browser updater",
                "Random advertisements",
                "Unknown attachments"
            },
            0,
            "Use official software update tools."),

        new(
            "A relative urgently asks for money. What should you do?",
            new[] {
                "Send money immediately",
                "Call their usual phone number",
                "Ignore all messages"
            },
            1,
            "Verify unexpected financial requests."),

        new(
            "Locking your device reduces security risks.",
            new[] { "True", "False" },
            0,
            "Locking devices helps prevent unauthorised access."),

        new(
            "How should changed banking details be verified?",
            new[] {
                "Use the new email address",
                "Trust the message",
                "Contact a previously verified number"
            },
            2,
            "Verify financial changes using trusted contact details.")
    };

    private readonly Label progress = new();
    private readonly Label question = new();
    private readonly Label feedback = new();

    private readonly FlowLayoutPanel options = new()
    {
        AutoSize = true,
        FlowDirection = FlowDirection.TopDown
    };

    private readonly Button next = new()
    {
        AutoSize = true
    };

    private readonly Action<string> record;

    private int index;
    private int score;

    public QuizForm(Action<string> record)
    {
        this.record = record;

        Text = "Cybersecurity Quiz";
        ClientSize = new Size(650, 430);
        StartPosition = FormStartPosition.CenterParent;
        Font = new Font("Segoe UI", 11);
        BackColor = Color.White;

        var body = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            Padding = new Padding(22)
        };

        question.Font = new Font(Font, FontStyle.Bold);
        question.MaximumSize = new Size(590, 0);
        question.AutoSize = true;

        progress.AutoSize = true;
        feedback.AutoSize = true;
        feedback.MaximumSize = new Size(590, 0);

        foreach (Control control in new Control[]
        {
            progress, question, options, feedback, next
        })
        {
            control.Margin = new Padding(0, 6, 0, 10);
            body.Controls.Add(control);
        }

        Controls.Add(body);

        next.Click += (_, _) =>
        {
            if (index == Questions.Length)
            {
                Close();
            }
            else if (++index == Questions.Length)
            {
                Finish();
            }
            else
            {
                ShowQuestion();
            }
        };

        ShowQuestion();
    }

    private void ShowQuestion()
    {
        var item = Questions[index];

        progress.Text =
            $"Question {index + 1} of {Questions.Length}";

        question.Text = item.Prompt;
        feedback.Text = "";

        next.Text = index == Questions.Length - 1
            ? "See results"
            : "Next question";

        next.Enabled = false;

        options.Controls.Clear();

        for (int i = 0; i < item.Options.Length; i++)
        {
            int answer = i;

            var button = new Button
            {
                AutoSize = true,
                Text = item.Options[i]
            };

            button.Click += (_, _) =>
            {
                bool correct = answer == item.Answer;

                if (correct)
                    score++;

                foreach (Control option in options.Controls)
                    option.Enabled = false;

                feedback.Text =
                    (correct
                        ? "Correct! "
                        : $"Correct answer: {item.Options[item.Answer]}. ")
                    + item.Explanation;

                record(
                    $"Quiz question {index + 1}: " +
                    (correct ? "Correct" : "Incorrect"));

                next.Enabled = true;
            };

            options.Controls.Add(button);
        }
    }

    private void Finish()
    {
        progress.Text = "Quiz complete";

        question.Text =
            $"Final score: {score}/{Questions.Length}";

        options.Visible = false;

        feedback.Text = score >= 10
            ? "Great job! You are a cybersecurity pro!"
            : score >= 6
                ? "Good work! Keep learning."
                : "Keep practising to stay safe online!";

        record(
            $"Completed quiz: {score}/{Questions.Length}");

        next.Text = "Close quiz";
    }
}
