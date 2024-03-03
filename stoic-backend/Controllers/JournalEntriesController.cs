using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            return Ok(journalEntry); 
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetJournalEntry(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var journalEntry = await _context.JournalEntries
                .AsNoTracking() // Using AsNoTracking for read-only operations for better performance
                .FirstOrDefaultAsync(je => je.Id == id && je.UserId == userId);

            if (journalEntry == null)
            {
                return NotFound();
            }

            return Ok(journalEntry);
        }

        [HttpGet]
        [Route("today")]
        public async Task<IActionResult> GetTodaysJournalEntry()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var today = DateTime.Today;

            var journalEntry = await _context.JournalEntries
                .AsNoTracking()
                .FirstOrDefaultAsync(je => je.UserId == userId && je.EntryDate >= today && je.EntryDate < today.AddDays(1));

            if (journalEntry == null)
            {
                return NotFound("No journal entry found for today.");
            }

            return Ok(journalEntry);
        }


        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetJournalEntriesByMonthAndYear(int year, int month, int maxAmount = 10, int skipCount = 0)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Ensure the month and year are valid
            if (month < 1 || month > 12 || year < 1)
            {
                return BadRequest("Invalid month or year.");
            }

            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var journalEntries = await _context.JournalEntries
                .Where(je => je.UserId == userId && je.EntryDate >= startDate && je.EntryDate < endDate)
                .OrderBy(je => je.EntryDate)
                .Skip(skipCount)
                .Take(maxAmount)
                .AsNoTracking()
                .ToListAsync();

            return Ok(journalEntries);
        }


        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateJournalEntry(int id, [FromBody] UpdateJournalEntryDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = _userManager.GetUserId(User);
            var journalEntry = await _context.JournalEntries.FirstOrDefaultAsync(je => je.Id == id && je.UserId == userId);

            if (journalEntry == null)
            {
                return NotFound();
            }

            // Update the properties
            journalEntry.Answer1 = model.Answer1;
            journalEntry.Answer2 = model.Answer2;
            journalEntry.Answer3 = model.Answer3;
            journalEntry.Answer4 = model.Answer4;
            journalEntry.Notes = model.Notes;
            journalEntry.EntryDate = DateTime.Now;

            _context.JournalEntries.Update(journalEntry);
            await _context.SaveChangesAsync();

            return Ok(journalEntry);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteJournalEntry(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var journalEntry = await _context.JournalEntries
                .FirstOrDefaultAsync(je => je.Id == id && je.UserId == userId);

            if (journalEntry == null)
            {
                return NotFound();
            }

            _context.JournalEntries.Remove(journalEntry);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
