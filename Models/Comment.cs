using System;

namespace NatureHub.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; } = DateTime.Now;

        // Foreign key for Discussion
        public int DiscussionId { get; set; }
        // Navigation property to the parent discussion
        public Discussion? Discussion { get; set; }

        // Foreign key for ApplicationUser
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}