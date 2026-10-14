using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcxiomCRM.Controllers;

[Authorize]
public class LeadsController : Controller
{
    private readonly ApplicationDbContext _db; private readonly AuditService _audit;
    public LeadsController(ApplicationDbContext db, AuditService audit) { _db=db; _audit=audit; }
    public async Task<IActionResult> Index(string? search)
    {
        var q=_db.Leads.AsQueryable();
        if (!User.IsInRole("Admin") && !User.IsInRole("Manager")) q=q.Where(x=>x.OwnerId==User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (!string.IsNullOrWhiteSpace(search)) q=q.Where(x=>x.Name.Contains(search)||x.Email.Contains(search)||x.Status.Contains(search));
        ViewBag.Search=search; return View(await q.OrderByDescending(x=>x.CreatedAtUtc).ToListAsync());
    }
    public IActionResult Create()=>View(new Lead());
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Lead lead)
    {
        lead.OwnerId=User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(await _db.Leads.AnyAsync(x=>x.Email==lead.Email)) ModelState.AddModelError(nameof(lead.Email),"A lead with this email already exists.");
        if(!ModelState.IsValid)return View(lead);
        _db.Leads.Add(lead); await _db.SaveChangesAsync(); await _audit.WriteAsync("Create","Lead",lead.Id.ToString(),null,lead.Name);
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(int id)
    {
        var x=await _db.Leads.FindAsync(id); if(x==null)return NotFound();
        if(!CanAccess(x.OwnerId))return Forbid(); return View(x);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Lead input)
    {
        if(id!=input.Id)return BadRequest(); var x=await _db.Leads.FindAsync(id); if(x==null)return NotFound();
        if(!CanAccess(x.OwnerId))return Forbid();
        if(input.Status=="Converted" && x.Status=="Converted") ModelState.AddModelError(nameof(input.Status),"Lead is already converted.");
        if(!ModelState.IsValid)return View(input);
        var old=x.Status; x.Name=input.Name; x.Email=input.Email; x.Phone=input.Phone; x.Source=input.Source;
        x.Status=input.Status; x.Priority=input.Priority; x.Notes=input.Notes; await _db.SaveChangesAsync();
        await _audit.WriteAsync("Update","Lead",id.ToString(),old,x.Status); return RedirectToAction(nameof(Index));
    }
    private bool CanAccess(string? owner)=>User.IsInRole("Admin")||User.IsInRole("Manager")||owner==User.FindFirstValue(ClaimTypes.NameIdentifier);
}
