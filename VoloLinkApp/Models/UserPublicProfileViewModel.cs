using VoloLinkApp.Models;

public class UserPublicProfileViewModel
{
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsVerifiedVolunteer { get; set; }
    public int CreatedEvents { get; set; }
    public int ParticipatingEvents { get; set; }

    public int AttendedEvents { get; set; }
   

    public List<Event> UpcomingParticipatingEvents { get; set; }
    public List<Event> CreatedEventsList { get; set; }
    public List<Event> AttendedEventsList { get; set; }
}