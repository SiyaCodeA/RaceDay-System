using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.ViewModels;

public class EventViewModel
{
    public int EventId { get; set; }

    public int AccountId { get; set; }

    [Required]
    [StringLength(150)]
    [Display(Name = "Event name")]
    public string EventName { get; set; } = string.Empty;

    [StringLength(1000)]
    [Display(Name = "Event description")]
    public string? EventDescription { get; set; }

    [Display(Name = "Event date")]
    public DateOnly EventDate { get; set; } =
        DateOnly.FromDateTime(DateTime.Today);

    [Required]
    [StringLength(255)]
    public string Location { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    [Display(Name = "Event type")]
    public string EventType { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Banner image URL")]
    public string? BannerImageUrl { get; set; }
}