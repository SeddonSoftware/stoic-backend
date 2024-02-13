using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using stoic_backend.Data;
using stoic_backend.Models;

namespace stoic_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class JournalEntriesController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public JournalEntriesController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost(Name = "Create Journal Entry")]
        public async Task<IActionResult> Create(JournalEntry journalEntry)
        {
            // Set the UserId to the current user's ID
            journalEntry.UserId = _userManager.GetUserId(User);

            _context.Add(journalEntry);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
