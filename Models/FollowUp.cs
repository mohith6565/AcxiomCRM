using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models;

public class FollowUp
{
    public int Id { get; set; }
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int? LeadId { get; set; }
    [Required] public DateTime FollowUpDate { get; set; } = DateTime.Today.AddDays(1);
    [Required, StringLength(150)] public string Subject { get; set; } = "";
    [Required, StringLength(30)] public string Type { get; set; } = "Call";
    [Required, StringLength(30)] public string Status { get; set; } = "Planned";
    [StringLength(1000)] public string? Notes { get; set; }
    [StringLength(450)] public string? AssignedUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
