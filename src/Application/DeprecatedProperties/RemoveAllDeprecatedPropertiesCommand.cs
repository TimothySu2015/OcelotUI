using MediatR;
using OcelotUI.Application.Common;

namespace OcelotUI.Application.DeprecatedProperties;

public record RemoveAllDeprecatedPropertiesCommand : IRequest<Result<int>>;
