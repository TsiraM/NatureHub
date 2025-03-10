using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using NatureHub.Data;
using NatureHub.Models;

namespace NatureHub.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Create(int discussionId)
        {
            ViewBag.DiscussionId = discussionId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Comment comment)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    comment.ApplicationUserId = user.Id;
                    comment.CreateDate = DateTime.Now;
                    _context.Comments.Add(comment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("GetDiscussion", "Home", new { id = comment.DiscussionId });
                }
            }
            return View(comment);
        }
    }
}