namespace OcelotUI.Infrastructure.Persistence;

public class OcelotConfigOptions
{
    public const string SectionName = "OcelotConfig";
    public string FilePath { get; set; } = "ocelot.json";
}
