using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RaceDay.API.Models;

[Table("Account")]
[Index("Email", Name = "UQ_Account_Email", IsUnique = true)]
public partial class Account
{
    [Key]
    [Column("AccountID")]
    public int AccountId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string FirstName { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string LastName { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string? PhoneNumber { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Role { get; set; } = null!;

    [StringLength(500)]
    [Unicode(false)]
    public string? ProfileImageUrl { get; set; }

    [InverseProperty("Account")]
    public virtual ICollection<Entry> Entries { get; set; } = new List<Entry>();

    [InverseProperty("Account")]
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
