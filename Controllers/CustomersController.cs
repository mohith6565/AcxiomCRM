using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcxiomCRM.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private readonly ApplicationDbContext _db; private readonly AuditService _audit;
    public CustomersController(ApplicationDbContext db, AuditService audit) { _db = db; _audit = audit; }

    public async Task<IActionResult> Index(string? search)
    {
        var q = _db.Customers.AsQueryable();
        if (!User.IsInRole("Admin") && !User.IsInRole("Manager"))
            q = q.Where(c => c.OwnerId == User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(c => c.CustomerName.Contains(search) || c.Email.Contains(search) || (c.Company != null && c.Company.Contains(search)));
        ViewBag.Search = search;
        return View(await q.OrderBy(c => c.CustomerName).ToListAsync());
    }
    public IActionResult Create() => View(new Customer { CustomerCode = "CUS-" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant() });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer customer)
    {
        customer.OwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (await _db.Customers.AnyAsync(c => c.Email == customer.Email || c.Phone == customer.Phone || c.CustomerCode == customer.CustomerCode))
            ModelState.AddModelError("", "Customer code, email, or phone already exists.");
        if (!ModelState.IsValid) return View(customer);
        _db.Customers.Add(customer); await _db.SaveChangesAsync();
        await _audit.WriteAsync("Create", "Customer", customer.Id.ToString(), null, customer.CustomerName);
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Details(int id)
    {
        var c = await _db.Customers.FindAsync(id); if (c == null) return NotFound();
        if (!CanAccess(c.OwnerId)) return Forbid(); return View(c);
    }
    public async Task<IActionResult> Edit(int id)
    {
        var c = await _db.Customers.FindAsync(id); if (c == null) return NotFound();
        if (!CanAccess(c.OwnerId)) return Forbid(); return View(c);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Customer input)
    {
        if (id != input.Id) return BadRequest();
        var c = await _db.Customers.FindAsync(id); if (c == null) return NotFound();
        if (!CanAccess(c.OwnerId)) return Forbid();
        if (await _db.Customers.AnyAsync(x => x.Id != id && (x.Email == input.Email || x.Phone == input.Phone)))
            ModelState.AddModelError("", "Email or phone already belongs to another customer.");
        if (!ModelState.IsValid) return View(input);
        var old = c.CustomerName; c.CustomerName = input.CustomerName; c.Email = input.Email; c.Phone = input.Phone;
        c.Company = input.Company; c.Address = input.Address; c.City = input.City; c.State = input.State;
        c.Status = input.Status; c.ModifiedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(); await _audit.WriteAsync("Update", "Customer", id.ToString(), old, c.CustomerName);
        return RedirectToAction(nameof(Index));
    }
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles="Admin,Manager")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var c = await _db.Customers.FindAsync(id); if (c == null) return NotFound();
        c.Status = "Inactive"; c.ModifiedAtUtc = DateTime.UtcNow; await _db.SaveChangesAsync();
        await _audit.WriteAsync("Deactivate", "Customer", id.ToString(), "Active", "Inactive");
        return RedirectToAction(nameof(Index));
    }
    private bool CanAccess(string? ownerId) => User.IsInRole("Admin") || User.IsInRole("Manager") || ownerId == User.FindFirstValue(ClaimTypes.NameIdentifier);
}
