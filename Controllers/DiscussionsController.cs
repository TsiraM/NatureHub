using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using NatureHub.Data;
using NatureHub.Models;

namespace NatureHub.Controllers
{
    [Authorize] // Restrict access to authenticated users
    public class DiscussionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DiscussionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var discussions = await _context.Discussions
                    .Where(d => d.ApplicationUserId == user.Id)
                    .Include(d => d.Comments)
                    .OrderByDescending(d => d.CreateDate)
                    .ToListAsync();
                return View(discussions);
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Details(int id)
        {
            var discussion = await _context.Discussions
                .Include(d => d.Comments)
                .Include(d => d.ApplicationUser)
                .FirstOrDefaultAsync(d => d.DiscussionId == id);

            if (discussion == null)
            {
                return NotFound();
            }

            ViewBag.DiscussionId = id;
            return View(discussion);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Discussion discussion, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    discussion.ApplicationUserId = user.Id;

                    // Handle image upload
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        try
                        {
                            // Validate file type and size (5MB max)
                            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                            var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                            if (!allowedExtensions.Contains(fileExtension))
                            {
                                ModelState.AddModelError("ImageFile", "Invalid file type.");
                                return View(discussion);
                            }

                            if (imageFile.Length > 5 * 1024 * 1024) 
                            {
                                ModelState.AddModelError("ImageFile", "File size exceeds the limit of 5MB.");
                                return View(discussion);
                            }

                            // Save the uploaded file
                            string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                            if (!Directory.Exists(uploadsFolder))
                            {
                                Directory.CreateDirectory(uploadsFolder);
                            }
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await imageFile.CopyToAsync(fileStream);
                            }
                            discussion.ImageFilename = uniqueFileName;
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", $"Error uploading file: {ex.Message}");
                            return View(discussion);
                        }
                    }

                    discussion.CreateDate = DateTime.Now;
                    _context.Discussions.Add(discussion);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(discussion);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var discussion = await _context.Discussions.FindAsync(id);

            if (discussion == null)
            {
                return NotFound();
            }

            // Ensure the user owns the discussion before editing
            if (user == null || discussion.ApplicationUserId != user.Id)
            {
                return Forbid();
            }

            return View(discussion);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Discussion discussion)
        {
            if (id != discussion.DiscussionId)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || discussion.ApplicationUserId != user.Id)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(discussion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiscussionExists(discussion.DiscussionId))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(discussion);
        }

        private bool DiscussionExists(int discussionId)
        {
            throw new NotImplementedException(); // Placeholder for existence check
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var discussion = await _context.Discussions.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);

            // Ensure only the owner can delete the discussion
            if (discussion == null || user == null || discussion.ApplicationUserId != user.Id)
            {
                return Forbid();
            }

            _context.Discussions.Remove(discussion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int discussionId, string content)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var discussion = await _context.Discussions.FindAsync(discussionId);
            if (discussion == null)
            {
                return NotFound();
            }

            // Create and add a new comment
            var comment = new Comment
            {
                Content = content,
                DiscussionId = discussionId,
                ApplicationUserId = user.Id,
                CreateDate = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Redirect back to the discussion details page
            return RedirectToAction("Details", new { id = discussionId });
        }
    }
}
