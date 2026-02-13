using MediatR;
using OcelotUI.Application.Common;
using OcelotUI.Application.GlobalConfig.Queries.GetGlobalConfig;

namespace OcelotUI.Application.GlobalConfig.Commands.UpdateGlobalConfig;

public record UpdateGlobalConfigCommand(GlobalConfigDto Config) : IRequest<Result>;
