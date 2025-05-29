using VoloLinkApp.Areas.Identity.Data;

public class VerificationRequest
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public VoloLinkUser User { get; set; }
    public DateTime RequestDate { get; set; }
    public string Status { get; set; } // "Pending", "Approved", "Rejected"
    public string Message { get; set; } // User's message explaining why they should be verified
    public string? AdminResponse { get; set; }
    public DateTime? ProcessedDate { get; set; }
}