using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;
using OcelotUI.Domain.Entities;

namespace OcelotUI.Application.Snapshots.Queries.GetAllSnapshots;

public class GetAllSnapshotsQueryHandler(ISnapshotRepository repository)
    : IRequestHandler<GetAllSnapshotsQuery, Result<List<ConfigSnapshot>>>
{
    public async Task<Result<List<ConfigSnapshot>>> Handle(
        GetAllSnapshotsQuery request, CancellationToken cancellationToken)
    {
        var snapshots = await repository.GetAllSnapshotsAsync(cancellationToken);
        return Result.Success(snapshots);
    }
}
