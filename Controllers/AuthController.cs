
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RaceDayContext _context;
    private readonly PasswordHasher<Account> _passwordHasher;

    public AuthController(RaceDayContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<Account>();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        bool emailExists = await _context.Accounts
            .AnyAsync(a => a.Email == request.Email);

        if (emailExists)
            return Conflict(new { message = "Email already registered." });

        var account = new Account
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Role = "Participant"
        };

        account.PasswordHash = _passwordHasher.HashPassword(
            account, request.Password);

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Registration successful.",
            accountId = account.AccountId,
            role = account.Role
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Email == request.Email);

        if (account == null)
            return Unauthorized(new { message = "Invalid email or password." });

        var result = _passwordHasher.VerifyHashedPassword(
            account, account.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
            return Unauthorized(new { message = "Invalid email or password." });

        // Store the authenticated user's details in the server-side session.
        HttpContext.Session.Clear();
        HttpContext.Session.SetInt32("AccountId", account.AccountId);
        HttpContext.Session.SetString("Role", account.Role);

        return Ok(new
        {
            message = "Login successful.",
            accountId = account.AccountId,
            firstName = account.FirstName,
            role = account.Role
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return Ok(new { message = "Logout successful." });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            accountId = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
            role = User.FindFirst(
                System.Security.Claims.ClaimTypes.Role)?.Value
        });
    }

    [Authorize(Roles = "Organiser")]
    [HttpGet("organiser-check")]
    public IActionResult OrganiserCheck()
    {
        return Ok(new { message = "Organiser access granted." });
    }
}
