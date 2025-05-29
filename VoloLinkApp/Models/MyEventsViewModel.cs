namespace VoloLinkApp.Models
{
    public class MyEventsViewModel
    {
        public IEnumerable<Event> CreatedEvents { get; set; } = new List<Event>();
        public IEnumerable<Event> ParticipatingEvents { get; set; } = new List<Event>();
    }
}