using Microsoft.AspNetCore.Mvc;
using NatureHub.Data;
using NatureHub.Models;

namespace NatureHub.Controllers
{
    
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor - initializes database context
        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Displays form to create a new comment
        public IActionResult Create(int discussionId)
        {
            ViewBag.DiscussionId = discussionId;
            return View();
        }

        // POST: Handles new comment submission
        [HttpPost]
        public async Task<IActionResult> Create(Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.CreateDate = DateTime.Now;
                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("GetDiscussion", "Home", new { id = comment.DiscussionId });
            }
            return View(comment);
        }
    }
}
