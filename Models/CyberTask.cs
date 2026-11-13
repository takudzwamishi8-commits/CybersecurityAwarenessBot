
using System;

namespace CybersecurityAwarenessBot.Models
{
    // Stores the details of a cybersecurity task.
    public class CyberTask
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // A reminder is optional.
        public DateTime? ReminderAt { get; set; }

        // Tracks whether the task is finished.
        public bool IsCompleted { get; set; } = false;

        // Records when the task was created.
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
