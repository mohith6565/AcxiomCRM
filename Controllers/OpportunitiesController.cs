using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcxiomCRM.Controllers;

[Authorize]
public class OpportunitiesController : Controller
{
    private readonly ApplicationDbContext _db; private readonly AuditService _audit;
    public OpportunitiesController(ApplicationDbContext db, AuditService audit){_db=db;_audit=audit;}
    public async Task<IActionResult> Index()
    {
        var q=_db.Opportunities.Include(x=>x.Customer).AsQueryable();
        if(!User.IsInRole("Admin")&&!User.IsInRole("Manager"))q=q.Where(x=>x.OwnerId==User.FindFirstValue(ClaimTypes.NameIdentifier));
        return View(await q.OrderByDescending(x=>x.CreatedAtUtc).ToListAsync());
    }
    public async Task<IActionResult> Create(){ViewBag.Customers=await _db.Customers.OrderBy(x=>x.CustomerName).ToListAsync();return View(new Opportunity());}
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Opportunity item)
    {
        if(item.ExpectedCloseDate.Date<DateTime.Today)ModelState.AddModelError(nameof(item.ExpectedCloseDate),"Expected close date cannot be in the past.");
        if(item.Amount<=0)ModelState.AddModelError(nameof(item.Amount),"Opportunity amount must be greater than zero.");
        item.OwnerId=User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(!ModelState.IsValid){ViewBag.Customers=await _db.Customers.ToListAsync();return View(item);}
        _db.Opportunities.Add(item);await _db.SaveChangesAsync();await _audit.WriteAsync("Create","Opportunity",item.Id.ToString(),null,item.OpportunityName);
        return RedirectToAction(nameof(Index));
    }
}
