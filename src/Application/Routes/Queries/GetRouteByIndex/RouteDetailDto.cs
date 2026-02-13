using OcelotUI.Domain.Entities;

namespace OcelotUI.Application.Routes.Queries.GetRouteByIndex;

public class RouteDetailDto
{
    public int? Index { get; set; }
    public OcelotRoute Route { get; set; } = new();

    public static RouteDetailDto FromEntity(int index, OcelotRoute route)
        => new() { Index = index, Route = route };
}
