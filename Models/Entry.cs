using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RaceDay.API.Models;

[Table("Entry")]
[Index("AccountId", "EventId", Name = "UQ_Entry_Account_Event", IsUnique = true)]
public partial class Entry
{
    [Key]
    [Column("EntryID")]
    public int EntryId { get; set; }

    [Column("AccountID")]
    public int AccountId { get; set; }

    [Column("EventID")]
    public int EventId { get; set; }

    [Column("CategoryID")]
    public int CategoryId { get; set; }

    public DateTime EntryDate { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string EntryStatus { get; set; } = null!;

    [ForeignKey("AccountId")]
    [InverseProperty("Entries")]
    public virtual Account Account { get; set; } = null!;

    [ForeignKey("EventId, CategoryId")]
    [InverseProperty("Entries")]
    public virtual Category Category { get; set; } = null!;

    [ForeignKey("EventId")]
    [InverseProperty("Entries")]
    public virtual Event Event { get; set; } = null!;

    [InverseProperty("Entry")]
    public virtual Result? Result { get; set; }
}
