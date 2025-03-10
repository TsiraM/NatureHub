using Microsoft.AspNetCore.Mvc;
using NatureHub.Data;
using Microsoft.EntityFrameworkCore;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var discussions = await _context.Discussions
            .Include(d => d.Comments)
            .Include(d => d.ApplicationUser)
            .OrderByDescending(d => d.CreateDate)
            .ToListAsync();
        return View(discussions);
    }

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
        return View(user);
    }

    public IActionResult Error()
    {
        return View();
    }
}