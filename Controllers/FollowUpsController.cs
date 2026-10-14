using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcxiomCRM.Controllers;

[Authorize]
public class FollowUpsController : Controller
{
    private readonly ApplicationDbContext _db; private readonly AuditService _audit;
    public FollowUpsController(ApplicationDbContext db,AuditService audit){_db=db;_audit=audit;}
    public async Task<IActionResult> Index()
    {
        var q=_db.FollowUps.Include(x=>x.Customer).AsQueryable();
        if(!User.IsInRole("Admin")&&!User.IsInRole("Manager"))q=q.Where(x=>x.AssignedUserId==User.FindFirstValue(ClaimTypes.NameIdentifier));
        return View(await q.OrderBy(x=>x.FollowUpDate).ToListAsync());
    }
    public async Task<IActionResult> Create(){ViewBag.Customers=await _db.Customers.ToListAsync();return View(new FollowUp());}
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FollowUp item)
    {
        if(item.FollowUpDate.Date<DateTime.Today)ModelState.AddModelError(nameof(item.FollowUpDate),"Follow-up date cannot be earlier than today.");
        item.AssignedUserId=User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(!ModelState.IsValid){ViewBag.Customers=await _db.Customers.ToListAsync();return View(item);}
        _db.FollowUps.Add(item);await _db.SaveChangesAsync();await _audit.WriteAsync("Create","FollowUp",item.Id.ToString(),null,item.Subject);
        return RedirectToAction(nameof(Index));
    }
}
