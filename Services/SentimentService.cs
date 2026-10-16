using System.Collections.Generic;

namespace CybersecurityAwarenessBot.Services
{
    // Detects simple emotions using predefined words and phrases.
    public class SentimentService
    {
        private readonly Dictionary<string, string[]> keywords =
            new Dictionary<string, string[]>
            {
                ["worried"] = new[]
                {
                    "worried", "scared", "anxious",
                    "afraid", "nervous"
                },

                ["frustrated"] = new[]
                {
                    "frustrated", "annoyed",
                    "angry", "overwhelmed"
                },

                ["confused"] = new[]
                {
                    "confused", "don't understand",
                    "do not understand", "unsure", "not sure"
                },

                ["curious"] = new[]
                {
                    "curious", "interested",
                    "keen", "excited"
                }
            };

        public string Detect(string input)
        {
            foreach (var group in keywords)
            {
                foreach (string word in group.Value)
                {
                    bool containsEmotion =
                        ChatbotService.HasPhrase(input, word);

                    bool emotionIsNegated =
                        ChatbotService.HasPhrase(input, "not " + word);

                    if (containsEmotion && !emotionIsNegated)
                    {
                        return group.Key;
                    }
                }
            }

            return "neutral";
        }

        public string Support(string sentiment)
        {
            switch (sentiment)
            {
                case "worried":
                    return "It's understandable to feel worried. " +
                           "Let's focus on a practical step you can take. ";

                case "frustrated":
                    return "That sounds frustrating. " +
                           "We can take this one step at a time. ";

                case "confused":
                    return "That's okay; let's use a simpler explanation. ";

                case "curious":
                    return "It's good that you want to learn more! ";

                default:
                    return string.Empty;
            }
        }
    }
}