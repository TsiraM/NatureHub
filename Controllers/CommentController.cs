using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using NatureHub.Data;
using NatureHub.Models;

namespace NatureHub.Controllers
{
    [Authorize] // Restrict access to authenticated users
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

       
        public async Task<IActionResult> Create(Comment comment)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    comment.ApplicationUserId = user.Id;
                    comment.CreateDate = DateTime.Now;

                    // Validate DiscussionId before adding the comment
                    if (comment.DiscussionId == 0)
                    {
                        ModelState.AddModelError("", "Invalid discussion.");
                        return View(comment);
                    }

                    _context.Comments.Add(comment);
                    await _context.SaveChangesAsync();

                    // Redirect to the discussion details page after saving
                    return RedirectToAction("Details", "Discussion", new { id = comment.DiscussionId });
                }
            }

            // Return view with validation errors if model state is invalid
            return View(comment);
        }
    }
}
