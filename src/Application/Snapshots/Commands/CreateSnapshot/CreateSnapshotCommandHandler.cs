using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;

namespace OcelotUI.Application.Snapshots.Commands.CreateSnapshot;

public class CreateSnapshotCommandHandler(ISnapshotRepository repository)
    : IRequestHandler<CreateSnapshotCommand, Result>
{
    public async Task<Result> Handle(
        CreateSnapshotCommand request, CancellationToken cancellationToken)
    {
        await repository.CreateSnapshotAsync(request.Description, cancellationToken);
        return Result.Success();
    }
}
