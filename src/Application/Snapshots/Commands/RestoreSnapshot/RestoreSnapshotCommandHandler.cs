using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;

namespace OcelotUI.Application.Snapshots.Commands.RestoreSnapshot;

public class RestoreSnapshotCommandHandler(ISnapshotRepository repository)
    : IRequestHandler<RestoreSnapshotCommand, Result>
{
    public async Task<Result> Handle(
        RestoreSnapshotCommand request, CancellationToken cancellationToken)
    {
        // Read target content FIRST, before CreateSnapshot could potentially trim it
        var content = await repository.GetSnapshotContentAsync(request.SnapshotId, cancellationToken);
        if (content is null)
            return Result.Failure("Snapshot not found.");

        return await repository.RestoreSnapshotAsync(request.SnapshotId, content, cancellationToken);
    }
}
