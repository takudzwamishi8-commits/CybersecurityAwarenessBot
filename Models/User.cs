namespace CybersecurityAwarenessBot.Models
{
    // Stores information about the user during the conversation.
    public class User
    {
        public string Name { get; set; } = string.Empty;

        public string FavouriteTopic { get; set; } = string.Empty;

        public string CurrentTopic { get; set; } = string.Empty;

        public string LastSentiment { get; set; } = "neutral";
    }
}