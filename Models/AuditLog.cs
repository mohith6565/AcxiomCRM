using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models;

public class AuditLog
{
    public long Id { get; set; }
    [StringLength(450)] public string? UserId { get; set; }
    [Required, StringLength(80)] public string Action { get; set; } = "";
    [Required, StringLength(80)] public string EntityName { get; set; } = "";
    public string? RecordId { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    [StringLength(30)] public string Result { get; set; } = "Success";
    [StringLength(64)] public string? IpAddress { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
