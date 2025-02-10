using System;

namespace NatureHub.Models
{
    public class Comment
    {
        public int CommentId { get; set; }

        public string AuthorName { get; set; } = string.Empty; 
        public string Content { get; set; } = string.Empty; 

        public DateTime CreateDate { get; set; } = DateTime.Now;

        // Foreign key
        public int DiscussionId { get; set; }

        // Navigation property to the parent discussion
        public Discussion? Discussion { get; set; }
    }
}
