
using Microsoft.AspNetCore.Identity;
using RaceDay.API.Models;
using System.Security.Principal;

// Create a temporary Account object
var account = new Account
{
    FirstName = "Test",
    LastName = "Organiser",
    Email = "testorganiser@example.com",
    Role = "Organiser"
};

// Generate a password hash using ASP.NET Core Identity
var passwordHasher = new PasswordHasher<Account>();

string password = "RaceDayOrganiser123!";
string passwordHash = passwordHasher.HashPassword(account, password);

Console.WriteLine("Email: " + account.Email);
Console.WriteLine("Role: " + account.Role);
Console.WriteLine("Password: " + password);
Console.WriteLine("PasswordHash: " + passwordHash);
