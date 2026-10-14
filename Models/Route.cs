using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RaceDay.API.Models;

[Table("Route")]
[Index("EventId", Name = "UQ_Route_Event", IsUnique = true)]
public partial class Route
{
    [Key]
    [Column("RouteID")]
    public int RouteId { get; set; }

    [Column("EventID")]
    public int EventId { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string RouteName { get; set; } = null!;

    [StringLength(1000)]
    [Unicode(false)]
    public string? RouteDescription { get; set; }

    [Column(TypeName = "decimal(6, 2)")]
    public decimal RouteDistance { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string? MapUrl { get; set; }

    [ForeignKey("EventId")]
    [InverseProperty("Route")]
    public virtual Event Event { get; set; } = null!;
}
