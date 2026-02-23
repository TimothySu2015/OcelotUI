using MediatR;
using OcelotUI.Application.Common;

namespace OcelotUI.Application.Snapshots.Commands.ClearAllSnapshots;

public record ClearAllSnapshotsCommand : IRequest<Result>;
