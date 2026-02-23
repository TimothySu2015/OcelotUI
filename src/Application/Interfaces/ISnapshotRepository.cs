using OcelotUI.Application.Common;
using OcelotUI.Domain.Entities;

namespace OcelotUI.Application.Interfaces;

public interface ISnapshotRepository
{
    Task CreateSnapshotAsync(string description, CancellationToken ct = default);
    Task<List<ConfigSnapshot>> GetAllSnapshotsAsync(CancellationToken ct = default);
    Task<string?> GetSnapshotContentAsync(string snapshotId, CancellationToken ct = default);
    Task<Result> RestoreSnapshotAsync(string snapshotId, string snapshotContent, CancellationToken ct = default);
    Task<Result> DeleteSnapshotAsync(string snapshotId, CancellationToken ct = default);
    Task<Result> ClearAllSnapshotsAsync(CancellationToken ct = default);
}
