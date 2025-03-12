using System;
using System.Collections.Generic;

namespace NatureHub.Models
{
    public class Discussion
    {
        public int DiscussionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageFilename { get; set; } = null;
        public DateTime CreateDate { get; set; }

        // Foreign key for the user who created the discussion
        public string? ApplicationUserId { get; set; }

        // Navigation property to the user who created the discussion
        public ApplicationUser? ApplicationUser { get; set; }

        // Collection of comments associated with the discussion
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}