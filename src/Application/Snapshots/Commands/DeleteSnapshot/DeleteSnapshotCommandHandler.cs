using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;

namespace OcelotUI.Application.Snapshots.Commands.DeleteSnapshot;

public class DeleteSnapshotCommandHandler(ISnapshotRepository repository)
    : IRequestHandler<DeleteSnapshotCommand, Result>
{
    public async Task<Result> Handle(
        DeleteSnapshotCommand request, CancellationToken cancellationToken)
    {
        return await repository.DeleteSnapshotAsync(request.SnapshotId, cancellationToken);
    }
}
