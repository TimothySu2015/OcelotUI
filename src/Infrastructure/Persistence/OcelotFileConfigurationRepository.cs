using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using OcelotUI.Application.Interfaces;
using OcelotUI.Domain.Entities;

namespace OcelotUI.Infrastructure.Persistence;

public class OcelotFileConfigurationRepository(IOptions<OcelotConfigOptions> options)
    : IOcelotConfigurationRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private readonly string _filePath = options.Value.FilePath;

    public async Task<OcelotConfiguration> LoadAsync(CancellationToken ct = default)
    {
        if (!File.Exists(_filePath))
            return new OcelotConfiguration();

        var json = await File.ReadAllTextAsync(_filePath, ct);

        if (string.IsNullOrWhiteSpace(json))
            return new OcelotConfiguration();

        return JsonSerializer.Deserialize<OcelotConfiguration>(json, JsonOptions)
               ?? new OcelotConfiguration();
    }

    public async Task SaveAsync(OcelotConfiguration configuration, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(configuration, JsonOptions);
        var tmpPath = _filePath + ".tmp";

        await File.WriteAllTextAsync(tmpPath, json, ct);
        File.Move(tmpPath, _filePath, overwrite: true);
    }
}
