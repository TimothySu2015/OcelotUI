using OcelotUI.Domain.Entities;

namespace OcelotUI.Application.DynamicRoutes.Queries.GetDynamicRouteByIndex;

public class DynamicRouteDetailDto
{
    public int? Index { get; set; }
    public OcelotDynamicRoute DynamicRoute { get; set; } = new();

    public static DynamicRouteDetailDto FromEntity(int index, OcelotDynamicRoute dynamicRoute)
        => new() { Index = index, DynamicRoute = dynamicRoute };
}
