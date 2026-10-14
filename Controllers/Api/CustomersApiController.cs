using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AcxiomCRM.Controllers.Api;

[ApiController, Route("api/customers"), Authorize]
public class CustomersApiController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public CustomersApiController(ApplicationDbContext db)=>_db=db;
    public record CustomerDto(int Id,string CustomerCode,string CustomerName,string Email,string Phone,string? Company,string Status);
    public record CreateCustomerDto([Required, StringLength(30)]string CustomerCode,[Required, StringLength(120)]string CustomerName,
        [Required,EmailAddress]string Email,[Required,Phone]string Phone,[StringLength(150)]string? Company);
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> Get()
    {
        var q=_db.Customers.AsQueryable();
        if(!User.IsInRole("Admin")&&!User.IsInRole("Manager"))q=q.Where(x=>x.OwnerId==User.FindFirstValue(ClaimTypes.NameIdentifier));
        return await q.Select(x=>new CustomerDto(x.Id,x.CustomerCode,x.CustomerName,x.Email,x.Phone,x.Company,x.Status)).ToListAsync();
    }
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerDto>> GetById(int id)
    {
        var x=await _db.Customers.FindAsync(id);if(x==null)return NotFound();
        if(!User.IsInRole("Admin")&&!User.IsInRole("Manager")&&x.OwnerId!=User.FindFirstValue(ClaimTypes.NameIdentifier))return Forbid();
        return new CustomerDto(x.Id,x.CustomerCode,x.CustomerName,x.Email,x.Phone,x.Company,x.Status);
    }
    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create(CreateCustomerDto dto)
    {
        if(await _db.Customers.AnyAsync(x=>x.CustomerCode==dto.CustomerCode||x.Email==dto.Email||x.Phone==dto.Phone))
            return Conflict(new {message="Customer code, email, or phone already exists."});
        var x=new Customer{CustomerCode=dto.CustomerCode,CustomerName=dto.CustomerName,Email=dto.Email,Phone=dto.Phone,
            Company=dto.Company,OwnerId=User.FindFirstValue(ClaimTypes.NameIdentifier)};
        _db.Customers.Add(x);await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById),new{id=x.Id},new CustomerDto(x.Id,x.CustomerCode,x.CustomerName,x.Email,x.Phone,x.Company,x.Status));
    }
}
