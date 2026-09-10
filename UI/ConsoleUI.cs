namespace CybersecurityAwarenessBot.UI
{
    public class ConsoleUI
    {
        public void DisplayLogo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine();
            Console.WriteLine("==========================================================");
            Console.WriteLine("              CYBERSECURITY AWARENESS BOT                ");
            Console.WriteLine("==========================================================");
            Console.WriteLine();
            Console.WriteLine("                     __________");
            Console.WriteLine("                    /          \\");
            Console.WriteLine("                   /    ____    \\");
            Console.WriteLine("                  |    /    \\    |");
            Console.WriteLine("                  |   | LOCK |   |");
            Console.WriteLine("                  |   |  #   |   |");
            Console.WriteLine("                  |    \\____/    |");
            Console.WriteLine("                   \\            /");
            Console.WriteLine("                    \\__________/");
            Console.WriteLine();
            Console.WriteLine("                STAY ALERT. STAY SECURE.");
            Console.WriteLine("==========================================================");

            Console.ResetColor();
            Console.WriteLine();
        }

        public void DisplaySection(string title)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine();
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine($"  {title}");
            Console.WriteLine("----------------------------------------------------------");

            Console.ResetColor();
        }

        public void DisplayBotMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Bot: {message}");
            Console.ResetColor();
        }

        public void DisplayUserMessage()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("You: ");
            Console.ResetColor();
        }

        public void TypeMessage(string message)
        {
            foreach (char character in message)
            {
                Console.Write(character);
                Thread.Sleep(10);
            }

            Console.WriteLine();
        }
    }
}