namespace OcelotUI.Application.DynamicRoutes.Queries.GetAllDynamicRoutes;

public record DynamicRouteListItemDto(
    int Index,
    string? ServiceName,
    string? ServiceNamespace);
