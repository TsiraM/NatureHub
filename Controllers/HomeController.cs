using Microsoft.AspNetCore.Mvc;
using NatureHub.Data;
using NatureHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;


public class HomeController : Controller
{
    // Database context for data operations
    private readonly ApplicationDbContext _context;

    // Constructor initializes database context
    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    //Displays a list of discussions ordered by CreateDate in descending order on the home page
    public async Task<IActionResult> Index()
    {
        var discussions = await _context.Discussions
            .Include(d => d.Comments)
            .OrderByDescending(d => d.CreateDate)
            .ToListAsync();
        return View(discussions);
    }

    // GET: Retrieves and displays a specific discussion
    public async Task<IActionResult> GetDiscussion(int id)
    {
        var discussion = await _context.Discussions
            .Include(d => d.Comments)
            .FirstOrDefaultAsync(d => d.DiscussionId == id);

        if (discussion == null)
        {
            return NotFound();
        }

        return View(discussion);
    }

    // GET: Returns the discussion creation view
    public IActionResult Create()
    {
        return View("~/Views/Discussion/Create.cshtml");
    }

    // POST: Handles new discussion creation with image upload
    [HttpPost]
    public async Task<IActionResult> Create(Discussion discussion, IFormFile imageFile)
    {
        if (ModelState.IsValid)
        {
            // Handle image upload if present
            if (imageFile != null && imageFile.Length > 0)
            {
                var uniqueFileName = DateTime.Now.Ticks + Path.GetExtension(imageFile.FileName);
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", uniqueFileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                discussion.ImageFilename = uniqueFileName;
            }

            // Set creation date and save to database
            discussion.CreateDate = DateTime.Now;
            _context.Discussions.Add(discussion);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        return View("~/Views/Discussion/Create.cshtml", discussion);
    }
}