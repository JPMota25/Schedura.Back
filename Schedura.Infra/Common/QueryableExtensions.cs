using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Schedura.Domain.Interfaces.Common;

namespace Schedura.Infra.Common;

public static class QueryableExtensions
{
	public static async Task<PagedResult<T>> ApplyUiFilters<T>(
		this IQueryable<T> query,
		PagedQuery pagedQuery,
		CancellationToken cancellationToken = default)
		where T : class
	{
		var predicate = FilterExpressionBuilder.Build<T>(pagedQuery.Filters, pagedQuery.LogicOperator);
		if (predicate is not null) query = query.Where(predicate);

		var totalCount = await query.CountAsync(cancellationToken);

		query = QuerySortBuilder.Apply(query, pagedQuery.Sort);

		var items = await query
			.Skip(pagedQuery.Page * pagedQuery.PageSize)
			.Take(pagedQuery.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedResult<T>(items.AsReadOnly(), totalCount);
	}

	public static async Task<PagedResult<TResult>> ApplyUiFilters<TEntity, TResult>(
		this IQueryable<TEntity> query,
		PagedQuery pagedQuery,
		Expression<Func<TEntity, TResult>> projection,
		CancellationToken cancellationToken = default)
		where TEntity : class
		where TResult : class
	{
		var predicate = FilterExpressionBuilder.Build<TEntity>(pagedQuery.Filters, pagedQuery.LogicOperator);
		if (predicate is not null) query = query.Where(predicate);

		var totalCount = await query.CountAsync(cancellationToken);

		query = QuerySortBuilder.Apply(query, pagedQuery.Sort);

		var items = await query
			.Skip(pagedQuery.Page * pagedQuery.PageSize)
			.Take(pagedQuery.PageSize)
			.Select(projection)
			.ToListAsync(cancellationToken);

		return new PagedResult<TResult>(items.AsReadOnly(), totalCount);
	}
}

file static class QuerySortBuilder
{
	public static IQueryable<T> Apply<T>(IQueryable<T> query, IReadOnlyList<SortDescriptor> sorts)
	{
		if (sorts.Count == 0)
		{
			var createdAtProp = typeof(T).GetProperty("CreatedAt",
				BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
			if (createdAtProp is not null)
				return ApplyOrderBy(query, createdAtProp.Name, descending: true);
			return query;
		}

		IOrderedQueryable<T>? ordered = null;
		for (var i = 0; i < sorts.Count; i++)
		{
			var sort = sorts[i];
			var prop = typeof(T).GetProperty(sort.Field,
				BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
			if (prop is null) continue;

			if (i == 0)
				ordered = ApplyOrderBy(query, prop.Name, sort.Descending);
			else if (ordered is not null)
				ordered = ApplyThenBy(ordered, prop.Name, sort.Descending);
		}

		return ordered ?? query;
	}

	private static IOrderedQueryable<T> ApplyOrderBy<T>(IQueryable<T> query, string propertyName, bool descending)
	{
		var parameter = Expression.Parameter(typeof(T), "x");
		var property = Expression.Property(parameter, propertyName);
		var lambda = Expression.Lambda(property, parameter);

		var methodName = descending ? "OrderByDescending" : "OrderBy";
		var method = typeof(Queryable)
			.GetMethods()
			.First(m => m.Name == methodName && m.GetParameters().Length == 2)
			.MakeGenericMethod(typeof(T), property.Type);

		return (IOrderedQueryable<T>)method.Invoke(null, [query, lambda])!;
	}

	private static IOrderedQueryable<T> ApplyThenBy<T>(IOrderedQueryable<T> query, string propertyName, bool descending)
	{
		var parameter = Expression.Parameter(typeof(T), "x");
		var property = Expression.Property(parameter, propertyName);
		var lambda = Expression.Lambda(property, parameter);

		var methodName = descending ? "ThenByDescending" : "ThenBy";
		var method = typeof(Queryable)
			.GetMethods()
			.First(m => m.Name == methodName && m.GetParameters().Length == 2)
			.MakeGenericMethod(typeof(T), property.Type);

		return (IOrderedQueryable<T>)method.Invoke(null, [query, lambda])!;
	}
}
