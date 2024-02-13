using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace stoic_backend.Models
{
    public class JournalEntry
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Answer1 { get; set; }
        public string Answer2 { get; set; } 
        public string Answer3 { get; set; } 
        public string Answer4 { get; set; }
        public string Notes { get; set; }
        public DateTime EntryDate { get; set; }
        public IdentityUser User { get; set; }

    }
}
