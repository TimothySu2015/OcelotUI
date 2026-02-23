using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.Interfaces;

namespace OcelotUI.Application.Snapshots.Queries.GetSnapshotContent;

public class GetSnapshotContentQueryHandler(ISnapshotRepository repository)
    : IRequestHandler<GetSnapshotContentQuery, Result<string>>
{
    public async Task<Result<string>> Handle(
        GetSnapshotContentQuery request, CancellationToken cancellationToken)
    {
        var content = await repository.GetSnapshotContentAsync(request.SnapshotId, cancellationToken);

        return content is not null
            ? Result.Success(content)
            : Result.Failure<string>("Snapshot not found.");
    }
}
