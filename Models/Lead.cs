using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models;

public class Lead
{
    public int Id { get; set; }
    [Required, StringLength(120)] public string Name { get; set; } = "";
    [Required, EmailAddress, StringLength(254)] public string Email { get; set; } = "";
    [Required, Phone, StringLength(20)] public string Phone { get; set; } = "";
    [Required, StringLength(80)] public string Source { get; set; } = "Website";
    [Required, StringLength(20)] public string Status { get; set; } = "New";
    [Range(1, 5)] public int Priority { get; set; } = 3;
    [StringLength(450)] public string? OwnerId { get; set; }
    [StringLength(1000)] public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
