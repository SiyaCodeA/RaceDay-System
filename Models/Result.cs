using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RaceDay.API.Models;

[Table("Result")]
[Index("EntryId", Name = "UQ_Result_Entry", IsUnique = true)]
public partial class Result
{
    [Key]
    [Column("ResultID")]
    public int ResultId { get; set; }

    [Column("EntryID")]
    public int EntryId { get; set; }

    public TimeOnly? FinishTime { get; set; }

    public int? FinishPosition { get; set; }

    [ForeignKey("EntryId")]
    [InverseProperty("Result")]
    public virtual Entry Entry { get; set; } = null!;
}
