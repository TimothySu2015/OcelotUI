namespace OcelotUI.Application.Routes.Queries.GetAllRoutes;

public record RouteListItemDto(
    int Index,
    string UpstreamPathTemplate,
    List<string> UpstreamHttpMethod,
    string DownstreamPathTemplate,
    string DownstreamScheme,
    string DownstreamHosts,
    string? Key);
