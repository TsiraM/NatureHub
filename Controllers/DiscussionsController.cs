using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using NatureHub.Data;
using NatureHub.Models;

namespace NatureHub.Controllers
{
    [Authorize]
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
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
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
            var discussion = await _context.Discussions.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);

            if (discussion == null || user == null || discussion.ApplicationUserId != user.Id)
            {
                return Forbid();
            }

            return View(discussion);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var discussion = await _context.Discussions.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);

            if (discussion == null || user == null || discussion.ApplicationUserId != user.Id)
            {
                return Forbid();
            }

            _context.Discussions.Remove(discussion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}