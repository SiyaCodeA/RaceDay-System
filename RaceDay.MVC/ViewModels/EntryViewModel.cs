
namespace RaceDay.MVC.ViewModels;

public class EntryViewModel
{
    public int EntryId { get; set; }

    public int AccountId { get; set; }

    public int EventId { get; set; }

    public int CategoryId { get; set; }

    public DateTime EntryDate { get; set; }

    public string EntryStatus { get; set; } = string.Empty;

    public string EventName { get; set; } = string.Empty;

    public DateOnly EventDate { get; set; }

    public string Location { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public string CategoryType { get; set; } = string.Empty;
}
