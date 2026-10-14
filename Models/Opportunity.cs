using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models;

public class Opportunity
{
    public int Id { get; set; }
    [Required, StringLength(150)] public string OpportunityName { get; set; } = "";
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int? LeadId { get; set; }
    [Range(0.01, 1000000000)] public decimal Amount { get; set; }
    [Range(0, 100)] public int Probability { get; set; }
    [Required, StringLength(30)] public string Stage { get; set; } = "Qualification";
    public DateTime ExpectedCloseDate { get; set; } = DateTime.Today.AddDays(30);
    [StringLength(450)] public string? OwnerId { get; set; }
    [StringLength(1000)] public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
