namespace OcelotUI.Domain.Entities;

public class ConfigSnapshot
{
    public string Id { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public string Description { get; set; } = "";
    public string FileName { get; set; } = "";
    public int RouteCount { get; set; }
    public long FileSizeBytes { get; set; }
}
