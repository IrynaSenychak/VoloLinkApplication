using System.ComponentModel.DataAnnotations;
using VoloLinkApp.Areas.Identity.Data;

namespace VoloLinkApp.Models
{
    public class Event
    {
        public Event()
        {
            Participants = new List<VoloLinkUser>();
            AttendedParticipants = new List<VoloLinkUser>();
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
        public string? CreatorId { get; set; }
        public VoloLinkUser? Creator { get; set; }
        public bool RequiresVerifiedVolunteer { get; set; }
        public ICollection<VoloLinkUser> Participants { get; set; }

        public ICollection<VoloLinkUser> AttendedParticipants { get; set; }
        public bool IsCompleted { get; set; }
    }
}