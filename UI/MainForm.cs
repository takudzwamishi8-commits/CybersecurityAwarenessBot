using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using CybersecurityAwarenessBot.Models;
using CybersecurityAwarenessBot.Services;

namespace CybersecurityAwarenessBot.UI
{
    // Builds the interface in code, so no Designer file is needed.
    public sealed partial class MainForm : Form
    {
        private ChatbotService chatbot = new ChatbotService();
        private User user = new User();

        private readonly AudioService audio = new AudioService();

        private readonly RichTextBox history = new RichTextBox();
        private readonly TextBox input = new TextBox();
        private readonly TextBox nameInput = new TextBox();

        private readonly Label status = new Label();
        private readonly Label memory = new Label();
        private readonly Button voiceButton = new Button();

        private bool started;

        private readonly Color navy = Color.FromArgb(18, 33, 55);
        private readonly Color teal = Color.FromArgb(0, 107, 116);

        public MainForm()
        {
            Text = "Cybersecurity Awareness Assistant";
            Size = new Size(1120, 800);
            MinimumSize = new Size(980, 720);

            StartPosition = FormStartPosition.CenterScreen;

            Font = new Font("Segoe UI", 10);
            BackColor = Color.FromArgb(241, 245, 249);
            AutoScaleMode = AutoScaleMode.Dpi;

            BuildInterface();
            

            AddMessage(
                "Assistant",
                "Welcome to the Cybersecurity Awareness Bot! " +
                "Enter your name and click Start chat."
            );

            Shown += async (_, _) =>
            {
                nameInput.Focus();
                await PlayGreeting();
            };

            FormClosed += (_, _) => audio.Dispose();
        }

        private void BuildInterface()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(18)
            };

            root.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 260)
            );

            root.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100)
            );

            Controls.Add(root);

            root.Controls.Add(CreateSidebar(), 0, 0);
            root.Controls.Add(CreateChatArea(), 1, 0);
        }

        private FlowLayoutPanel CreateSidebar()
        {
            var sidebar = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = navy,
                Padding = new Padding(16)
            };

            Button taskButton = MakeButton("MY TASKS", 220);
            taskButton.Click += (_, _) => OpenPart3Tasks();
            sidebar.Controls.Add(taskButton);

            Button quizButton = MakeButton("CYBERSECURITY QUIZ", 220);
            quizButton.Click += (_, _) => OpenPart3Quiz();
            sidebar.Controls.Add(quizButton);

            Button activityButton = MakeButton("ACTIVITY LOG", 220);
            activityButton.Click += (_, _) => OpenPart3Activity();
            sidebar.Controls.Add(activityButton);

            sidebar.Controls.Add(new Label
            {
                Text = "STAY ALERT.\nSTAY SECURE.",
                ForeColor = Color.White,
                Font = new Font(
                    "Segoe UI",
                    17,
                    FontStyle.Bold
                ),
                Size = new Size(220, 70)
            });

            // A verbatim string preserves the ASCII artwork.
            sidebar.Controls.Add(new Label
            {
                Text = @"     __________
    /          \
   /    ____    \
  |    /    \    |
  |   | LOCK |   |
  |   |  #   |   |
  |    \____/    |
   \            /
    \__________/",

                Font = new Font("Consolas", 11),
                ForeColor = Color.Aquamarine,
                Size = new Size(220, 175)
            });

            sidebar.Controls.Add(new Label
            {
                Text = "EXPLORE A TOPIC",
                ForeColor = Color.White,
                Size = new Size(220, 25)
            });

            foreach (string topic in chatbot.TopicNames)
            {
                Button button = MakeButton(topic, 220);

                button.Click += (_, _) => Send(topic);

                sidebar.Controls.Add(button);
            }

            memory.ForeColor = Color.White;
            memory.Size = new Size(220, 115);
            memory.Text =
                "Memory lasts for this chat only.\n" +
                "Enter your name to begin.";

            sidebar.Controls.Add(memory);

            return sidebar;
        }

        private TableLayoutPanel CreateChatArea()
        {
            var main = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 7,
                Padding = new Padding(18, 0, 0, 0)
            };

            main.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100)
            );

            main.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 75)
            );

            main.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 50)
            );

            main.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100)
            );

            main.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 50)
            );

            main.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 55)
            );

            main.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 50)
            );

            main.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 42)
            );

            main.Controls.Add(new Label
            {
                Text = "Your online safety starts here\n" +
                       "Ask a question. Learn a practical step.",

                Dock = DockStyle.Fill,

                Font = new Font(
                    "Segoe UI",
                    15,
                    FontStyle.Bold
                ),

                ForeColor = navy
            }, 0, 0);

            main.Controls.Add(CreateNameRow(), 0, 1);

            history.Dock = DockStyle.Fill;
            history.ReadOnly = true;
            history.BackColor = Color.White;
            history.BorderStyle = BorderStyle.None;
            history.Font = new Font("Segoe UI", 11);
            history.AccessibleName = "Conversation history";

            main.Controls.Add(history, 0, 2);
            main.Controls.Add(CreateQuickReplies(), 0, 3);
            main.Controls.Add(CreateMessageRow(), 0, 4);
            main.Controls.Add(CreateActions(), 0, 5);

            status.Dock = DockStyle.Fill;
            status.ForeColor = teal;
            status.AutoEllipsis = true;

            main.Controls.Add(status, 0, 6);

            return main;
        }

        private FlowLayoutPanel CreateNameRow()
        {
            var row = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = false
            };

            row.Controls.Add(new Label
            {
                Text = "Your name:",
                AutoSize = true,
                Margin = new Padding(0, 8, 8, 0)
            });

            nameInput.Width = 190;
            nameInput.MaxLength = 50;
            nameInput.AccessibleName = "Your name";

            row.Controls.Add(nameInput);

            Button startButton = MakeButton("Start chat", 120);

            startButton.Click += (_, _) => StartChat();

            row.Controls.Add(startButton);

            return row;
        }

        private FlowLayoutPanel CreateQuickReplies()
        {
            var row = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = false
            };

            string[] options =
            {
                "Help",
                "Another tip",
                "Explain more"
            };

            foreach (string option in options)
            {
                Button button = MakeButton(option, 125);

                button.Click += (_, _) => Send(option);

                row.Controls.Add(button);
            }

            return row;
        }

        private TableLayoutPanel CreateMessageRow()
        {
            var row = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(0, 5, 0, 5)
            };

            row.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100)
            );

            row.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 95)
            );

            input.Dock = DockStyle.Fill;
            input.MaxLength = 2000;

            input.PlaceholderText =
                "Ask about passwords, scams, privacy...";

            input.AccessibleName = "Your message";

            input.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    Send(input.Text);
                }
            };

            row.Controls.Add(input, 0, 0);

            Button sendButton = MakeButton("Send", 85);

            sendButton.Click += (_, _) => Send(input.Text);

            row.Controls.Add(sendButton, 1, 0);

            return row;
        }

        private FlowLayoutPanel CreateActions()
        {
            var row = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = false
            };

            voiceButton.Text = "Replay greeting";
            voiceButton.Size = new Size(145, 36);

            voiceButton.Click += async (_, _) =>
                await PlayGreeting();

            row.Controls.Add(voiceButton);

            Button resetButton = MakeButton("New chat", 115);

            resetButton.Click += (_, _) => ResetChat();

            row.Controls.Add(resetButton);

            Button closeButton = MakeButton("Close", 85);

            closeButton.Click += (_, _) => Close();

            row.Controls.Add(closeButton);

            return row;
        }

        private Button MakeButton(string text, int width)
        {
            return new Button
            {
                Text = text,
                Size = new Size(width, 36),
                BackColor = teal,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 3, 8, 3),
                UseVisualStyleBackColor = false
            };
        }

        private void StartChat()
        {
            string name = nameInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                status.Text = "Please enter your name first.";
                nameInput.Focus();
                return;
            }

            user.Name = name;
            started = true;

            AddMessage(
                "Assistant",
                $"Nice to meet you, {name}! " +
                "Ask about a topic on the left or type 'help'. " +
                "You can also say 'I'm interested in privacy'."
            );

            UpdateMemory();

            status.Text = "Ready. Press Enter or click Send.";
            input.Focus();
        }

        private async void Send(string message)
        {
            if (!started)
            {
                status.Text =
                    "Enter your name and click Start chat first.";

                nameInput.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                status.Text =
                    "Please enter a question or choose a topic.";

                input.Focus();
                return;
            }

            AddMessage(user.Name, message.Trim());

            string response = await HandlePart3Message(message)
    ?? chatbot.GetResponse(message, user);

            AddMessage("Assistant", response);

            nameInput.Text = user.Name;
            input.Clear();

            UpdateMemory();

            status.Text = "Ready. Press Enter or click Send.";
            input.Focus();
        }

        private void UpdateMemory()
        {
            string name = started ? user.Name : "not set";

            string interest =
                string.IsNullOrWhiteSpace(user.FavouriteTopic)
                    ? "not shared"
                    : user.FavouriteTopic;

            memory.Text =
                $"Name: {name}\n" +
                $"Interest: {interest}\n" +
                $"Topic: {user.CurrentTopic}\n" +
                $"Tone: {user.LastSentiment}";
        }

        private void AddMessage(string speaker, string message)
        {
            history.SelectionStart = history.TextLength;

            history.SelectionColor =
                speaker == "Assistant" ? teal : navy;

            using (var boldFont =
                new Font(history.Font, FontStyle.Bold))
            {
                history.SelectionFont = boldFont;

                history.AppendText(
                    speaker + Environment.NewLine
                );
            }

            history.SelectionFont = history.Font;
            history.SelectionColor = navy;

            history.AppendText(
                message +
                Environment.NewLine +
                Environment.NewLine
            );

            history.SelectionStart = history.TextLength;
            history.ScrollToCaret();
        }

        private void ResetChat()
        {
            DialogResult answer = MessageBox.Show(
                "Clear this conversation and its remembered details?",
                "New chat",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (answer != DialogResult.Yes)
            {
                return;
            }

            chatbot = new ChatbotService();
            user = new User();
            started = false;

            history.Clear();
            nameInput.Clear();
            input.Clear();

            UpdateMemory();

            AddMessage(
                "Assistant",
                "Welcome! Enter your name and click Start chat."
            );

            status.Text = "New chat started.";
            nameInput.Focus();
        }

        private async Task PlayGreeting()
        {
            voiceButton.Enabled = false;

            string audioPath = Path.Combine(
                AppContext.BaseDirectory,
                "Media",
                "greeting.wav"
            );

            string result =
                await audio.PlayGreetingAsync(audioPath);

            if (!IsDisposed)
            {
                status.Text = result;
                voiceButton.Enabled = true;
            }
        }
    }
}