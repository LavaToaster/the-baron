using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheBaron.Data;
using TheBaron.Entities;

namespace TheBaron.Http.Controllers;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet(Name = "GetUsers")]
    public async Task<List<User>> Index()
    {
        return await _context.Users.ToListAsync();
    }
}