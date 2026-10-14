using AcxiomCRM.Data;
using AcxiomCRM.Models;
using System.Security.Claims;

namespace AcxiomCRM.Services;

public class AuditService
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _http;
    public AuditService(ApplicationDbContext db, IHttpContextAccessor http) { _db = db; _http = http; }

    public async Task WriteAsync(string action, string entity, string? recordId = null,
        string? oldValue = null, string? newValue = null, string result = "Success")
    {
        var ctx = _http.HttpContext;
        _db.AuditLogs.Add(new AuditLog {
            UserId = ctx?.User.FindFirstValue(ClaimTypes.NameIdentifier),
            Action = action, EntityName = entity, RecordId = recordId,
            OldValue = oldValue, NewValue = newValue, Result = result,
            IpAddress = ctx?.Connection.RemoteIpAddress?.ToString()
        });
        await _db.SaveChangesAsync();
    }
}
