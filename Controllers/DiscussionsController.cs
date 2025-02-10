using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NatureHub.Data;
using NatureHub.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NatureHub.Controllers
{
    
    public class DiscussionController : Controller
    {
        // Database context field
        private readonly ApplicationDbContext _context;

        // Constructor initializes the database context
        public DiscussionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Returns the view for creating a new discussion
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Handles the creation of a new discussion with optional image upload
        [HttpPost]
        public async Task<IActionResult> Create(Discussion discussion, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                // Handle image file upload if present
                if (imageFile != null && imageFile.Length > 0)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                    // Create directory if it doesn't exist
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Save the image file
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    discussion.ImageFilename = uniqueFileName;
                }

                // Set creation date and save discussion to database
                discussion.CreateDate = DateTime.Now;
                _context.Discussions.Add(discussion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(discussion);
        }

        // GET: Retrieves and displays all discussions with their comments
        public async Task<IActionResult> Index()
        {
            var discussions = await _context.Discussions
                .Include(d => d.Comments)
                .OrderByDescending(d => d.CreateDate)
                .ToListAsync();
            return View(discussions);
        }
    }
}
