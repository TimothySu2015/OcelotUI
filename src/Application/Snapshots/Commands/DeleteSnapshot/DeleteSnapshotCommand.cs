using MediatR;
using OcelotUI.Application.Common;

namespace OcelotUI.Application.Snapshots.Commands.DeleteSnapshot;

public record DeleteSnapshotCommand(string SnapshotId) : IRequest<Result>;
