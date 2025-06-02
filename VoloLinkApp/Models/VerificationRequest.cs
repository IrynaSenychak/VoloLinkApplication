using VoloLinkApp.Areas.Identity.Data;

public class VerificationRequest
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public VoloLinkUser User { get; set; }
    public DateTime RequestDate { get; set; }
    public string Status { get; set; } 
    public string Message { get; set; } 
    public string? AdminResponse { get; set; }
    public DateTime? ProcessedDate { get; set; }
}