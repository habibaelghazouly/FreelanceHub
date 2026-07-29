namespace FreelanceHub.Application.DTOs.Results;

public class CreateJobPageResult
{
    public List<SelectableItemResult> Categories { get; set; } = [];
    public List<SelectableItemResult> Tags { get; set; } = [];
    public List<SelectableItemResult> Skills { get; set; } = [];
}

public class SelectableItemResult
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}