using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models;

public class Customer
{
    public int Id { get; set; }
    [Required, StringLength(30)] public string CustomerCode { get; set; } = "";
    [Required, StringLength(120)] public string CustomerName { get; set; } = "";
    [Required, EmailAddress, StringLength(254)] public string Email { get; set; } = "";
    [Required, Phone, StringLength(20)] public string Phone { get; set; } = "";
    [StringLength(150)] public string? Company { get; set; }
    [StringLength(200)] public string? Address { get; set; }
    [StringLength(80)] public string? City { get; set; }
    [StringLength(80)] public string? State { get; set; }
    [Required, StringLength(20)] public string Status { get; set; } = "Active";
    [StringLength(450)] public string? OwnerId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAtUtc { get; set; }
}
