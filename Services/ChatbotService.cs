using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CybersecurityAwarenessBot.Models;

namespace CybersecurityAwarenessBot.Services
{
    // Handles the conversation separately from the graphical interface.
    public class ChatbotService
    {
        private readonly List<Topic> topics = TopicCatalogue.Create();

        private readonly SentimentService sentiments =
            new SentimentService();

        private readonly Random random = new Random();

        // Remembers the last tip selected for each topic.
        private readonly Dictionary<string, int> previousTips =
            new Dictionary<string, int>();

        // A delegate defines how personalised replies are formatted.
        public delegate string ResponseFormatter(
            string response,
            User user
        );

        public ResponseFormatter FormatResponse { get; set; } =
            (response, user) =>
                string.IsNullOrWhiteSpace(user.Name)
                    ? response
                    : $"{user.Name}, {response}";

        public IReadOnlyList<string> TopicNames
        {
            get
            {
                return topics.Select(topic => topic.Name).ToList();
            }
        }

        // Makes input easier to compare.
        public static string Normalise(string input)
        {
            string message = input
                .Replace('’', '\'')
                .Trim()
                .ToLowerInvariant();

            return Regex.Replace(message, @"\s+", " ");
        }

        // Matches complete words or phrases.
        // For example, "hi" must not match inside "phishing".
        public static bool HasPhrase(string input, string phrase)
        {
            string pattern =
                @"(?<!\w)" + Regex.Escape(phrase) + @"(?!\w)";

            return Regex.IsMatch(
                input,
                pattern,
                RegexOptions.IgnoreCase
            );
        }

        private Topic? FindTopic(string input)
        {
            return topics.FirstOrDefault(topic =>
                topic.Keywords.Any(keyword =>
                    HasPhrase(input, keyword)));
        }

        public string GetResponse(string? input, User user)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "Please enter a question or message.";
            }

            if (input.Length > 2000)
            {
                return "Please keep your message under 2,000 characters.";
            }

            string message = Normalise(input);

            user.LastSentiment = sentiments.Detect(message);

            string support =
                sentiments.Support(user.LastSentiment);

            // Allows the user to introduce or change their name.
            Match nameMatch = Regex.Match(
                input.Trim(),
                @"^(?:my name is|call me)\s+(.+?)[.!]?$",
                RegexOptions.IgnoreCase
            );

            if (nameMatch.Success)
            {
                string name = nameMatch.Groups[1].Value.Trim();

                if (name.Length > 50)
                {
                    return "Please use a name of 50 characters or fewer.";
                }

                user.Name = name;

                return $"Nice to meet you, {user.Name}! " +
                       "I'll remember your name during this chat.";
            }

            if (message == "what is my name" ||
                message == "what is my name?")
            {
                if (string.IsNullOrWhiteSpace(user.Name))
                {
                    return "You haven't told me your name yet.";
                }

                return $"Your name is {user.Name}.";
            }

            // Recalls a previously stored interest.
            if (message.Contains("what do you remember") ||
                message.Contains("what is my favourite topic") ||
                message.Contains("what is my favorite topic"))
            {
                if (string.IsNullOrEmpty(user.FavouriteTopic))
                {
                    return FormatResponse(
                        "you haven't shared a favourite topic yet.",
                        user
                    );
                }

                return FormatResponse(
                    "I remember that you're interested in " +
                    user.FavouriteTopic + ".",
                    user
                );
            }

            if (message == "help" ||
                message.Contains("what can i ask"))
            {
                return "Ask about " +
                       string.Join(", ", TopicNames) +
                       ". Try 'another tip', 'explain more', " +
                       "'my name is Sam', or " +
                       "'I'm interested in privacy'.";
            }

            if (message.Contains("how are you"))
            {
                return FormatResponse(
                    "I'm doing well and ready to help you stay safe online!",
                    user
                );
            }

            if (message.Contains("purpose") ||
                message.Contains("what do you do"))
            {
                return "My purpose is to help you understand " +
                       "cybersecurity and practise safer online habits.";
            }

            if (message == "exit" ||
                message == "bye" ||
                message == "goodbye")
            {
                return FormatResponse(
                    "goodbye! Stay safe online. " +
                    "Use the Close button when you're finished.",
                    user
                );
            }

            Topic? topic = FindTopic(message);

            bool sharingInterest =
                message.Contains("interested in") ||
                message.Contains("favourite topic is") ||
                message.Contains("favorite topic is");

            // Stores the user's interest and provides advice immediately.
            if (sharingInterest && topic != null)
            {
                user.FavouriteTopic = topic.Name;
                user.CurrentTopic = topic.Name;

                return FormatResponse(
                    support +
                    "I'll remember that you're interested in " +
                    topic.Name + ". " +
                    PickTip(topic),
                    user
                );
            }

            string[] followUpPhrases =
            {
                "another tip",
                "explain more",
                "tell me more",
                "more details",
                "continue",
                "another one",
                "what else"
            };

            bool followUp = followUpPhrases.Any(phrase =>
                HasPhrase(message, phrase));

            bool confused =
                user.LastSentiment == "confused";

            // Uses the previous topic when the user asks a follow-up.
            if (topic == null &&
                (followUp ||
                 confused ||
                 user.LastSentiment == "worried" ||
                 user.LastSentiment == "frustrated"))
            {
                topic = topics.FirstOrDefault(item =>
                    item.Name == user.CurrentTopic);
            }

            if (topic != null)
            {
                user.CurrentTopic = topic.Name;

                string response;

                if (confused ||
                    HasPhrase(message, "explain more"))
                {
                    response = topic.Explanation;
                }
                else
                {
                    response = PickTip(topic);
                }

                if (topic.Name == user.FavouriteTopic)
                {
                    response =
                        "Since you're interested in " +
                        topic.Name + ", " + response;
                }

                // Support and useful advice appear in the same reply.
                return FormatResponse(
                    support + response,
                    user
                );
            }

            if (followUp)
            {
                return "Which topic would you like another tip about? " +
                       "Try passwords, phishing, scams or privacy.";
            }

            if (user.LastSentiment != "neutral")
            {
                return support +
                       "Which cybersecurity topic would you like help with?";
            }

            if (HasPhrase(message, "hello") ||
                HasPhrase(message, "hi") ||
                HasPhrase(message, "hey"))
            {
                return FormatResponse(
                    "hello! What would you like to learn about cybersecurity?",
                    user
                );
            }

            if (HasPhrase(message, "thanks") ||
                HasPhrase(message, "thank you"))
            {
                return FormatResponse(
                    "you're welcome! Would you like another safety tip?",
                    user
                );
            }

            // Default response for unrecognised input.
            return "I'm not sure I understand. Can you try rephrasing? " +
                   "Type 'help' to see the cybersecurity topics I cover.";
        }

        private string PickTip(Topic topic)
        {
            // Handles missing topic data without crashing.
            if (topic.Tips.Length == 0)
            {
                return topic.Explanation;
            }

            if (topic.Tips.Length == 1)
            {
                return topic.Tips[0];
            }

            bool hasPrevious = previousTips.TryGetValue(
                topic.Name,
                out int previous
            );

            // Excludes the previous tip to avoid immediate repetition.
            int[] choices = Enumerable
                .Range(0, topic.Tips.Length)
                .Where(index => !hasPrevious || index != previous)
                .ToArray();

            int selected = choices[random.Next(choices.Length)];

            previousTips[topic.Name] = selected;

            return topic.Tips[selected];
        }
    }
}