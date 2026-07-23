namespace FreelanceHub.Web.ViewModels;
public class MyJobListItemViewModel
{
    public int JobId { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Budget { get; set; }
    public DateTime Deadline { get; set; }
    public string Status { get; set; } = "";

    public int ApplicationsCount { get; set; }
}
