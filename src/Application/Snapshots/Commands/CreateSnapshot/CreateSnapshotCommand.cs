using MediatR;
using OcelotUI.Application.Common;

namespace OcelotUI.Application.Snapshots.Commands.CreateSnapshot;

public record CreateSnapshotCommand(string Description) : IRequest<Result>;
