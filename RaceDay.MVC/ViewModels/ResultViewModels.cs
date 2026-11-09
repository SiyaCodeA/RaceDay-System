namespace RaceDay.MVC.ViewModels;

public class OrganiserResultViewModel
{
    public int EntryId { get; set; }
    public int AccountId { get; set; }
    public string ParticipantName { get; set; } = "";
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
    public string EntryStatus { get; set; } = "";
    public DateTime EntryDate { get; set; }
    public int? ResultId { get; set; }
    public string? FinishTime { get; set; }
    public int? FinishPosition { get; set; }
}

public class ParticipantResultViewModel
{
    public int EntryId { get; set; }
    public int EventId { get; set; }
    public string EventName { get; set; } = "";
    public DateOnly EventDate { get; set; }
    public string Location { get; set; } = "";
    public string CategoryName { get; set; } = "";
    public string? FinishTime { get; set; }
    public int? FinishPosition { get; set; }
}

public class SaveResultViewModel
{
    public int EntryId { get; set; }
    public int EventId { get; set; }
    public string? FinishTime { get; set; }
    public int? FinishPosition { get; set; }
}