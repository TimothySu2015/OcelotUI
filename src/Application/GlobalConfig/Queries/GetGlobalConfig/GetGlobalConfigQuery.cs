using MediatR;
using OcelotUI.Application.Common;

namespace OcelotUI.Application.GlobalConfig.Queries.GetGlobalConfig;

public record GetGlobalConfigQuery : IRequest<Result<GlobalConfigDto>>;
