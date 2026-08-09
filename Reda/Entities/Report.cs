    using System.ComponentModel.DataAnnotations.Schema;

    namespace Reda.Entities
    {
        public class Report
        {
            public int Id { get; set; }
            public string Category { get; set; } = string.Empty;
            public string Subject { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string? Screenshot { get; set; }
            public int Status { get; set; } = 0;
            public DateTime SentAt { get; set; } = DateTime.UtcNow;
            public int UserId { get; set; }

            [ForeignKey(nameof(UserId))]
            public User User { get; set; }
        }
    }
