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

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateJournalEntry([FromBody] CreateJournalEntryDto model)
        {
            var userId = _userManager.GetUserId(User); // Get UserId from the currently authenticated user

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(); 
            }

            var journalEntry = new JournalEntry
            {
                Answer1 = model.Answer1,
                Answer2 = model.Answer2,
                Answer3 = model.Answer3,
                Answer4 = model.Answer4,
                Notes = model.Notes,
                EntryDate = DateTime.Now,
                UserId = userId
            };

            _context.JournalEntries.Add(journalEntry);
            await _context.SaveChangesAsync();

            return Ok(); 
        }
    }
}
