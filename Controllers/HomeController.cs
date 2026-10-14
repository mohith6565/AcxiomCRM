using AcxiomCRM.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    public HomeController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        if (!(User.Identity?.IsAuthenticated ?? false)) return View("Landing");
        ViewBag.Customers = await _db.Customers.CountAsync();
        ViewBag.OpenLeads = await _db.Leads.CountAsync(x => x.Status != "Converted" && x.Status != "Lost");
        ViewBag.OpenOpportunities = await _db.Opportunities.CountAsync(x => x.Stage != "Won" && x.Stage != "Lost");
        ViewBag.PipelineValue = await _db.Opportunities.Where(x => x.Stage != "Lost").SumAsync(x => (decimal?)x.Amount) ?? 0;
        ViewBag.StageData = await _db.Opportunities.GroupBy(x => x.Stage)
            .Select(g => new { Stage = g.Key, Count = g.Count() }).ToListAsync();
        return View();
    }
    [AllowAnonymous] public IActionResult AccessDenied() => View();
}
