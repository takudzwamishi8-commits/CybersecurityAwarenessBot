using CybersecurityAwarenessBot.Models;

namespace CybersecurityAwarenessBot.Services
{
    public class ChatbotService
    {
        public string GetResponse(string input, User user)
        {
            string message = input.Trim().ToLower();

            if (message == "help")
            {
                return "You can ask me about password safety, phishing, safe browsing, suspicious links, my purpose, or how I am.";
            }

            if (message.Contains("how are you"))
            {
                return $"I'm doing well, {user.Name}! I'm ready to help you stay safe online.";
            }

            if (message.Contains("purpose") ||
                message.Contains("what do you do"))
            {
                return $"My purpose is to help you, {user.Name}, learn about cybersecurity and stay safer online.";
            }

            if (message.Contains("what can i ask"))
            {
                return "You can ask me about password safety, phishing, safe browsing, suspicious links, and general online safety.";
            }

            if (message.Contains("password"))
            {
                return "Use strong and unique passwords for your accounts. Avoid using easy-to-guess information and enable two-factor authentication where possible.";
            }

            if (message.Contains("phishing"))
            {
                return "Phishing is a cyberattack where criminals pretend to be a trusted person or organisation to trick you into giving away information. Be careful with unexpected emails, messages and links.";
            }

            if (message.Contains("safe browsing") ||
                message.Contains("browsing"))
            {
                return "For safer browsing, check website addresses carefully, use trusted websites, keep your browser updated, and avoid downloading files from suspicious sources.";
            }

            if (message.Contains("suspicious link") ||
                message.Contains("link"))
            {
                return "Do not click suspicious links immediately. Check the sender and website address. When unsure, visit the organisation's official website directly.";
            }

            if (message.Contains("hello") ||
                message.Contains("hi") ||
                message.Contains("hey"))
            {
                return $"Hello {user.Name}! How can I help you with cybersecurity today?";
            }

            if (message.Contains("thank"))
            {
                return $"You're welcome, {user.Name}! Stay safe online.";
            }

            return "I didn't quite understand that. Could you rephrase? You can ask me about passwords, phishing, safe browsing or suspicious links.";
        }
    }
}