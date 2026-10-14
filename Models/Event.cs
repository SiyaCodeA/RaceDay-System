using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RaceDay.API.Models;

[Table("Event")]
public partial class Event
{
    [Key]
    [Column("EventID")]
    public int EventId { get; set; }

    [Column("AccountID")]
    public int AccountId { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string EventName { get; set; } = null!;

    [StringLength(1000)]
    [Unicode(false)]
    public string? EventDescription { get; set; }

    public DateOnly EventDate { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Location { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string EventType { get; set; } = null!;

    [StringLength(500)]
    [Unicode(false)]
    public string? BannerImageUrl { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("Events")]
    public virtual Account Account { get; set; } = null!;

    [InverseProperty("Event")]
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    [InverseProperty("Event")]
    public virtual ICollection<Entry> Entries { get; set; } = new List<Entry>();

    [InverseProperty("Event")]
    public virtual Route? Route { get; set; }
}
