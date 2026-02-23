using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;
using OcelotUI.Domain.Entities;

namespace OcelotUI.Infrastructure.Persistence;

public class FileSnapshotRepository(IOptions<OcelotConfigOptions> options) : ISnapshotRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private const int MaxSnapshots = 50;

    private readonly string _filePath = options.Value.FilePath;

    private string HistoryDir => Path.Combine(Path.GetDirectoryName(_filePath) ?? ".", ".ocelot-history");
    private string IndexPath => Path.Combine(HistoryDir, "snapshots.json");

    public async Task CreateSnapshotAsync(string description, CancellationToken ct = default)
    {
        if (!File.Exists(_filePath))
            return;

        var content = await File.ReadAllTextAsync(_filePath, ct);
        if (string.IsNullOrWhiteSpace(content))
            return;

        Directory.CreateDirectory(HistoryDir);

        var now = DateTime.UtcNow;
        var shortHash = Convert.ToHexString(RandomNumberGenerator.GetBytes(2)).ToLowerInvariant();
        var id = $"{now:yyyy-MM-ddTHHmmss}_{shortHash}";
        var fileName = $"{id}.json";
        var snapshotPath = Path.Combine(HistoryDir, fileName);

        await File.WriteAllTextAsync(snapshotPath, content, ct);

        var routeCount = 0;
        try
        {
            var config = JsonSerializer.Deserialize<OcelotConfiguration>(content, JsonOptions);
            routeCount = config?.Routes.Count ?? 0;
        }
        catch
        {
            // ignore parse errors for metadata
        }

        var snapshot = new ConfigSnapshot
        {
            Id = id,
            Timestamp = now,
            Description = description,
            FileName = fileName,
            RouteCount = routeCount,
            FileSizeBytes = new FileInfo(snapshotPath).Length
        };

        var snapshots = await LoadIndexAsync(ct);
        snapshots.Add(snapshot);

        // Trim oldest if over limit
        while (snapshots.Count > MaxSnapshots)
        {
            var oldest = snapshots[0];
            var oldPath = Path.Combine(HistoryDir, oldest.FileName);
            if (File.Exists(oldPath))
                File.Delete(oldPath);
            snapshots.RemoveAt(0);
        }

        await SaveIndexAsync(snapshots, ct);
    }

    public async Task<List<ConfigSnapshot>> GetAllSnapshotsAsync(CancellationToken ct = default)
    {
        return await LoadIndexAsync(ct);
    }

    public async Task<string?> GetSnapshotContentAsync(string snapshotId, CancellationToken ct = default)
    {
        var snapshots = await LoadIndexAsync(ct);
        var snapshot = snapshots.Find(s => s.Id == snapshotId);
        if (snapshot is null)
            return null;

        var path = Path.Combine(HistoryDir, snapshot.FileName);
        if (!File.Exists(path))
            return null;

        return await File.ReadAllTextAsync(path, ct);
    }

    public async Task<Result> RestoreSnapshotAsync(string snapshotId, string snapshotContent, CancellationToken ct = default)
    {
        // Create backup of current state before overwriting
        await CreateSnapshotAsync("Auto: Before restore", ct);

        var tmpPath = _filePath + ".tmp";
        await File.WriteAllTextAsync(tmpPath, snapshotContent, ct);
        File.Move(tmpPath, _filePath, overwrite: true);

        return Result.Success();
    }

    public async Task<Result> DeleteSnapshotAsync(string snapshotId, CancellationToken ct = default)
    {
        var snapshots = await LoadIndexAsync(ct);
        var snapshot = snapshots.Find(s => s.Id == snapshotId);
        if (snapshot is null)
            return Result.Failure("Snapshot not found.");

        var path = Path.Combine(HistoryDir, snapshot.FileName);
        if (File.Exists(path))
            File.Delete(path);

        snapshots.Remove(snapshot);
        await SaveIndexAsync(snapshots, ct);

        return Result.Success();
    }

    public Task<Result> ClearAllSnapshotsAsync(CancellationToken ct = default)
    {
        if (Directory.Exists(HistoryDir))
            Directory.Delete(HistoryDir, recursive: true);

        return Task.FromResult(Result.Success());
    }

    private async Task<List<ConfigSnapshot>> LoadIndexAsync(CancellationToken ct)
    {
        if (!File.Exists(IndexPath))
            return [];

        var json = await File.ReadAllTextAsync(IndexPath, ct);
        if (string.IsNullOrWhiteSpace(json))
            return [];

        return JsonSerializer.Deserialize<List<ConfigSnapshot>>(json, JsonOptions) ?? [];
    }

    private async Task SaveIndexAsync(List<ConfigSnapshot> snapshots, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(snapshots, JsonOptions);
        var tmpPath = IndexPath + ".tmp";
        await File.WriteAllTextAsync(tmpPath, json, ct);
        File.Move(tmpPath, IndexPath, overwrite: true);
    }
}
