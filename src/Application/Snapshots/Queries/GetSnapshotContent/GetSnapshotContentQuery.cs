using MediatR;
using OcelotUI.Application.Common;

namespace OcelotUI.Application.Snapshots.Queries.GetSnapshotContent;

public record GetSnapshotContentQuery(string SnapshotId) : IRequest<Result<string>>;
