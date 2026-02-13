using OcelotUI.Domain.Entities;

namespace OcelotUI.Application.Aggregates.Queries.GetAggregateByIndex;

public class AggregateDetailDto
{
    public int? Index { get; set; }
    public OcelotAggregate Aggregate { get; set; } = new();

    public static AggregateDetailDto FromEntity(int index, OcelotAggregate aggregate)
        => new() { Index = index, Aggregate = aggregate };
}
