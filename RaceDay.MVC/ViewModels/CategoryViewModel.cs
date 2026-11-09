using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RaceDay.MVC.ViewModels;

public class CategoryViewModel : IValidatableObject
{
    public int CategoryId { get; set; }

    [Required]
    public int EventId { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Category name")]
    public string CategoryName { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Description")]
    public string? CategoryDescription { get; set; }

    [Required]
    [StringLength(20)]
    [Display(Name = "Category type")]
    public string CategoryType { get; set; } = "Age";

    [Display(Name = "Minimum age")]
    public int? MinAge { get; set; }

    [Display(Name = "Maximum age")]
    public int? MaxAge { get; set; }

    [Display(Name = "Distance (km)")]
    public decimal? DistanceKm { get; set; }

    [JsonIgnore]
    public string? EventName { get; set; }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (CategoryType == "Age")
        {
            if (!MinAge.HasValue || !MaxAge.HasValue)
            {
                yield return new ValidationResult(
                    "Both minimum and maximum ages are required.",
                    new[] { nameof(MinAge), nameof(MaxAge) });
            }
            else if (MinAge < 0 || MaxAge < MinAge)
            {
                yield return new ValidationResult(
                    "Enter valid ages. Maximum age must be at least the minimum age.",
                    new[] { nameof(MinAge), nameof(MaxAge) });
            }

            if (DistanceKm.HasValue)
            {
                yield return new ValidationResult(
                    "Age categories cannot have a distance.",
                    new[] { nameof(DistanceKm) });
            }
        }
        else if (CategoryType == "Distance")
        {
            if (!DistanceKm.HasValue || DistanceKm <= 0)
            {
                yield return new ValidationResult(
                    "Enter a distance greater than zero.",
                    new[] { nameof(DistanceKm) });
            }

            if (MinAge.HasValue || MaxAge.HasValue)
            {
                yield return new ValidationResult(
                    "Distance categories cannot have age limits.",
                    new[] { nameof(MinAge), nameof(MaxAge) });
            }
        }
        else
        {
            yield return new ValidationResult(
                "Select either Age or Distance.",
                new[] { nameof(CategoryType) });
        }
    }
}