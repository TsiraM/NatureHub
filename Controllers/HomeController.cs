using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NatureHub.Data;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    
    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Retrieves and displays all discussions
    public async Task<IActionResult> Index()
    {
        var discussions = await _context.Discussions
            .Include(d => d.Comments)
            .Include(d => d.ApplicationUser)
            .OrderByDescending(d => d.CreateDate)
            .ToListAsync();
        return View(discussions);
    }

    // Retrieves and displays a specific discussion with its comments
    public async Task<IActionResult> GetDiscussion(int id)
    {
        var discussion = await _context.Discussions
            .Include(d => d.Comments)
                .ThenInclude(c => c.ApplicationUser)
            .Include(d => d.ApplicationUser)
            .FirstOrDefaultAsync(d => d.DiscussionId == id);

        if (discussion == null)
        {
            return NotFound();
        }
        return View(discussion);
    }

    // Displays the profile of a specific user along with their discussions
    [Authorize]
    public async Task<IActionResult> Profile(string id)
    {
        var user = await _context.Users
            .Include(u => u.Discussions)
                .ThenInclude(d => d.Comments)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound();
        }

        if (user.Discussions.Count == 0)
        {
            ViewData["NoDiscussions"] = "This user has no discussions yet.";
        }

        return View(user);
    }

    // Displays the error page
    public IActionResult Error()
    {
        return View();
    }
}
