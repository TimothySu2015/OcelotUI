namespace OcelotUI.Application.Aggregates.Queries.GetAllAggregates;

public record AggregateListItemDto(
    int Index,
    string UpstreamPathTemplate,
    List<string> RouteKeys,
    string? Aggregator);
