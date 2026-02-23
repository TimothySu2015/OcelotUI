using MediatR;
using OcelotUI.Application.Common;

namespace OcelotUI.Application.Snapshots.Commands.RestoreSnapshot;

public record RestoreSnapshotCommand(string SnapshotId) : IRequest<Result>;
