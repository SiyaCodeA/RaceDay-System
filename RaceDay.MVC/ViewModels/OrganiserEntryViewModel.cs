namespace RaceDay.MVC.ViewModels;

public class OrganiserEntryViewModel
{
    public int EntryId { get; set; }
    public int AccountId { get; set; }
    public string ParticipantName { get; set; } = "";
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
    public DateTime EntryDate { get; set; }
    public string EntryStatus { get; set; } = "";
}