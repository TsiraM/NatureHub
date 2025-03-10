﻿using System;
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

        // Properties for user relationship
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}