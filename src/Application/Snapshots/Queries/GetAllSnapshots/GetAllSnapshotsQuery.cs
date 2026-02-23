using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Domain.Entities;

namespace OcelotUI.Application.Snapshots.Queries.GetAllSnapshots;

public record GetAllSnapshotsQuery : IRequest<Result<List<ConfigSnapshot>>>;
