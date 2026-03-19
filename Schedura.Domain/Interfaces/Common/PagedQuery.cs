namespace Schedura.Domain.Interfaces.Common;

public enum FilterLogicOperator { And, Or }

public record SortDescriptor(string Field, bool Descending);

public record FilterDescriptor(string Field, string Operator, string? Value);

public record PagedQuery(
	int Page, int PageSize,
	IReadOnlyList<SortDescriptor> Sort,
	IReadOnlyList<FilterDescriptor> Filters,
	FilterLogicOperator LogicOperator = FilterLogicOperator.And);

public record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount);
