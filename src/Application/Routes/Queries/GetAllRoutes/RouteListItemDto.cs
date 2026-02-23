namespace OcelotUI.Application.Routes.Queries.GetAllRoutes;

public record RouteListItemDto(
    int Index,
    string? DisplayName,
    string UpstreamPathTemplate,
    List<string> UpstreamHttpMethod,
    string DownstreamPathTemplate,
    string DownstreamScheme,
    string DownstreamHosts,
    string? Key);
