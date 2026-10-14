using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RaceDay.API.Models;

[Table("Category")]
[Index("EventId", "CategoryId", Name = "UQ_Category_Event_CategoryID", IsUnique = true)]
[Index("EventId", "CategoryName", Name = "UQ_Category_Event_Name", IsUnique = true)]
public partial class Category
{
    [Key]
    [Column("CategoryID")]
    public int CategoryId { get; set; }

    [Column("EventID")]
    public int EventId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string CategoryName { get; set; } = null!;

    [StringLength(500)]
    [Unicode(false)]
    public string? CategoryDescription { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string CategoryType { get; set; } = null!;

    public int? MinAge { get; set; }

    public int? MaxAge { get; set; }

    [Column(TypeName = "decimal(6, 2)")]
    public decimal? DistanceKm { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<Entry> Entries { get; set; } = new List<Entry>();

    [ForeignKey("EventId")]
    [InverseProperty("Categories")]
    public virtual Event Event { get; set; } = null!;
}
