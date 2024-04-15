namespace Domain;

public class Inquiry
{
    public Guid InquiryId { get; set; }
    public string? Description { get; set; }
    public DateTime SubmissionTime { get; set; }
    public DateTime ResolutionDeadline { get; set; }
    public bool IsResolved { get; set; }
}