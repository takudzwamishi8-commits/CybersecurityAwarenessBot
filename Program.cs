using CybersecurityAwarenessBot.Models;
using CybersecurityAwarenessBot.Services;
using CybersecurityAwarenessBot.UI;

namespace CybersecurityAwarenessBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Cybersecurity Awareness Bot";

            ConsoleUI ui = new ConsoleUI();
            AudioService audioService = new AudioService();
            ChatbotService chatbot = new ChatbotService();

            string audioPath = Path.Combine(
                AppContext.BaseDirectory,
                "Media",
                "greeting.wav");

            audioService.PlayGreeting(audioPath);

            ui.DisplayLogo();

            ui.DisplaySection("WELCOME");

            ui.TypeMessage(
                "Welcome to the Cybersecurity Awareness Bot!");

            ui.TypeMessage(
                "I am here to help you learn about cybersecurity.");

            ui.TypeMessage(
                "Please enter your name to begin.");

            User user = new User();

            while (string.IsNullOrWhiteSpace(user.Name))
            {
                Console.Write("Name: ");

                user.Name = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(user.Name))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Please enter a valid name.");
                    Console.ResetColor();
                }
            }

            ui.DisplaySection("CHATBOT READY");

            ui.DisplayBotMessage(
                $"Nice to meet you, {user.Name}!");

            ui.DisplayBotMessage(
                "You can ask me about passwords, phishing, safe browsing and suspicious links.");

            ui.DisplayBotMessage(
                "Type 'help' to see the topics I can discuss.");

            ui.DisplayBotMessage(
                "Type 'exit' when you are finished.");

            while (true)
            {
                ui.DisplayUserMessage();

                string input = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine(
                        "Please enter a question or message.");

                    Console.ResetColor();

                    continue;
                }

                if (input.Trim().Equals(
                    "exit",
                    StringComparison.OrdinalIgnoreCase))
                {
                    ui.DisplayBotMessage(
                        $"Goodbye, {user.Name}! Stay safe online.");

                    break;
                }

                string response =
                    chatbot.GetResponse(input, user);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Bot: ");
                Console.ResetColor();

                ui.TypeMessage(response);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
        }
    }
}