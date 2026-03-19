using System.Linq.Expressions;
using System.Reflection;
using Schedura.Domain.Interfaces.Common;

namespace Schedura.Infra.Common;

public static class FilterExpressionBuilder
{
	public static Expression<Func<T, bool>>? Build<T>(
		IReadOnlyList<FilterDescriptor> filters,
		FilterLogicOperator logicOperator) where T : class
	{
		if (filters.Count == 0) return null;

		var parameter = Expression.Parameter(typeof(T), "x");
		var expressions = new List<Expression>();

		foreach (var filter in filters)
		{
			var property = typeof(T).GetProperty(filter.Field,
				BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
			if (property is null) continue;

			var propertyExpr = Expression.Property(parameter, property);
			var filterExpr = BuildFilterExpression(propertyExpr, property.PropertyType, filter.Operator, filter.Value);
			if (filterExpr is not null)
				expressions.Add(filterExpr);
		}

		if (expressions.Count == 0) return null;

		Func<Expression, Expression, Expression> combine = logicOperator == FilterLogicOperator.And
			? (left, right) => Expression.AndAlso(left, right)
			: (left, right) => Expression.OrElse(left, right);
		var body = expressions.Aggregate(combine);

		return Expression.Lambda<Func<T, bool>>(body, parameter);
	}

	private static Expression? BuildFilterExpression(
		MemberExpression propertyExpr, Type propertyType, string op, string? value)
	{
		var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

		if (underlyingType == typeof(string))
			return BuildStringFilter(propertyExpr, op, value);

		return BuildComparableFilter(propertyExpr, underlyingType, propertyType, op, value);
	}

	private static Expression? BuildStringFilter(MemberExpression propertyExpr, string op, string? value)
	{
		var toLower = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
		var propertyLower = Expression.Call(
			Expression.Coalesce(propertyExpr, Expression.Constant(string.Empty)),
			toLower);

		return op switch
		{
			"isEmpty" => Expression.OrElse(
				Expression.Equal(propertyExpr, Expression.Constant(null, typeof(string))),
				Expression.Equal(propertyExpr, Expression.Constant(string.Empty))),
			"isNotEmpty" => Expression.AndAlso(
				Expression.NotEqual(propertyExpr, Expression.Constant(null, typeof(string))),
				Expression.NotEqual(propertyExpr, Expression.Constant(string.Empty))),
			_ when value is null => null,
			"contains" => Expression.Call(propertyLower,
				typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!,
				Expression.Constant(value.ToLower())),
			"equals" => Expression.Equal(propertyLower, Expression.Constant(value.ToLower())),
			"startsWith" => Expression.Call(propertyLower,
				typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)])!,
				Expression.Constant(value.ToLower())),
			"endsWith" => Expression.Call(propertyLower,
				typeof(string).GetMethod(nameof(string.EndsWith), [typeof(string)])!,
				Expression.Constant(value.ToLower())),
			_ => null,
		};
	}

	private static Expression? BuildComparableFilter(
		MemberExpression propertyExpr, Type underlyingType, Type propertyType, string op, string? value)
	{
		if (value is null) return null;

		object? converted;
		try { converted = Convert.ChangeType(value, underlyingType); }
		catch { return null; }

		Expression left = propertyType != underlyingType
			? Expression.Convert(propertyExpr, underlyingType)
			: propertyExpr;
		var right = Expression.Constant(converted, underlyingType);

		return op switch
		{
			"=" or "equals" => Expression.Equal(left, right),
			"!=" => Expression.NotEqual(left, right),
			">" => Expression.GreaterThan(left, right),
			">=" => Expression.GreaterThanOrEqual(left, right),
			"<" => Expression.LessThan(left, right),
			"<=" => Expression.LessThanOrEqual(left, right),
			_ => null,
		};
	}
}
