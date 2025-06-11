using CSharpFunctionalExtensions;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Numerics;
namespace IdentityService.Models;

public class Identity : Entity<Guid>
{
    public string Login { get; set; }
    public string Password { get; set; } 
    public string Email { get; set; }
    public string? Phone { get; set; }
    public string Role { get; set; }

    public Identity(string login, string password, string email, string? phone, string role)
    {
        Id = Guid.NewGuid();
        Login = login;
        Password = password;
        Email = email;
        Phone = phone;
        Role = role;
    }
    public Identity(Guid id) {
        Id = id;
        Login = "";
        Password = "";
        Email = "";
        Phone = "";
        Role = "";
    }

    public void ChangePassword(string newPassword)
    {
        Password = newPassword;
    }
}