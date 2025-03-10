#nullable enable
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NatureHub.Models;
using System.ComponentModel.DataAnnotations;

namespace NatureHub.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _environment;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
        }

        public string? Username { get; set; }
        public string? ProfileImage { get; set; }

        [TempData]
        public string? StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required]
            [Display(Name = "Name")]
            public string Name { get; set; } = string.Empty;

            [Display(Name = "Location")]
            public string Location { get; set; } = string.Empty;

            [Display(Name = "Profile Picture")]
            public IFormFile? ImageFile { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            Username = userName;
            ProfileImage = user.ImageFilename;

            Input = new InputModel
            {
                Name = user.Name,
                Location = user.Location ?? string.Empty
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            user.Name = Input.Name;
            user.Location = Input.Location;

            if (Input.ImageFile != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Input.ImageFile.FileName);
                var imagePath = Path.Combine(_environment.WebRootPath, "images", fileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await Input.ImageFile.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(user.ImageFilename))
                {
                    var oldImagePath = Path.Combine(_environment.WebRootPath, "images", user.ImageFilename);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                user.ImageFilename = fileName;
            }

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}