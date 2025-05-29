using System.ComponentModel.DataAnnotations;
using VoloLinkApp.Areas.Identity.Data;

namespace VoloLinkApp.Models
{
    public class Event
    {
        public Event()
        {
            // Initialize the Participants collection in the constructor
            Participants = new List<VoloLinkUser>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(200)]
        public string Location { get; set; }

        // Navigation property
        public string? CreatorId { get; set; }
        public VoloLinkUser? Creator { get; set; }

        public bool RequiresVerifiedVolunteer { get; set; }

        // Collection of participants
        public ICollection<VoloLinkUser> Participants { get; set; }
    }
}