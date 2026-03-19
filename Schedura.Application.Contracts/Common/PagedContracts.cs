namespace Schedura.Application.Contracts.Common;

public enum FilterLogicOperator { And, Or }

public record SortItem(string Field, string Sort);

public record FilterItem(string Field, string Operator, string? Value, int? Id = null);

public record PagedRequest(
	int Page, int PageSize,
	IReadOnlyList<SortItem> Sort,
	IReadOnlyList<FilterItem> Filters,
	FilterLogicOperator LogicOperator = FilterLogicOperator.And);
