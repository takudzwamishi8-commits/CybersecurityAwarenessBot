namespace CybersecurityAwarenessBot.Models
{
    // Groups a topic's name, keywords, tips and explanation.
    public record Topic(
        string Name,
        string[] Keywords,
        string[] Tips,
        string Explanation
    );
}
