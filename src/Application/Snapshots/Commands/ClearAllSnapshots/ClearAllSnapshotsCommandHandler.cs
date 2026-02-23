using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;

namespace OcelotUI.Application.Snapshots.Commands.ClearAllSnapshots;

public class ClearAllSnapshotsCommandHandler(ISnapshotRepository repository)
    : IRequestHandler<ClearAllSnapshotsCommand, Result>
{
    public async Task<Result> Handle(
        ClearAllSnapshotsCommand request, CancellationToken cancellationToken)
    {
        return await repository.ClearAllSnapshotsAsync(cancellationToken);
    }
}
