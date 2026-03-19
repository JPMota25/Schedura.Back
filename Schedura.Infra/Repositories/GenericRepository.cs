using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Schedura.Domain.Entities;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Infra.Data;

namespace Schedura.Infra.Repositories;

public class GenericRepository<TEntity, TId>(AppDbContext context) : IGenericRepository<TEntity, TId>
    where TEntity : class
{
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();
    private readonly bool _hasDeletedAtProperty = context.Model.FindEntityType(typeof(TEntity))?.FindProperty("DeletedAt") is not null;

    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity is BaseEntity baseEntity)
        {
            baseEntity.SetDeletedAt();
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsNoTrackingAsync(CancellationToken cancellationToken = default)
    {
        return await QueryActive()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await QueryActive().AnyAsync(predicate, cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        var keyProperty = GetPrimaryKeyProperty();
        var trackedEntity = FindTrackedEntityById(keyProperty, id);
        if (trackedEntity is not null)
        {
            return trackedEntity;
        }

        var predicate = BuildPrimaryKeyPredicate(keyProperty, id);

        return await QueryActive().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsNoTrackingAsync(TId id, CancellationToken cancellationToken = default)
    {
        var keyProperty = GetPrimaryKeyProperty();
        var predicate = BuildPrimaryKeyPredicate(keyProperty, id);

        return await QueryActive().AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    private IQueryable<TEntity> QueryActive()
    {
        if (!_hasDeletedAtProperty)
        {
            return _dbSet;
        }

        var parameter = Expression.Parameter(typeof(TEntity), "entity");
        var deletedAtProperty = Expression.Call(
            typeof(EF),
            nameof(EF.Property),
            [typeof(DateTimeOffset?)],
            parameter,
            Expression.Constant("DeletedAt"));

        var notDeleted = Expression.Equal(
            deletedAtProperty,
            Expression.Constant(null, typeof(DateTimeOffset?)));

        var predicate = Expression.Lambda<Func<TEntity, bool>>(notDeleted, parameter);
        return _dbSet.Where(predicate);
    }

    private IReadOnlyProperty GetPrimaryKeyProperty()
    {
        var entityType = context.Model.FindEntityType(typeof(TEntity))
                         ?? throw new InvalidOperationException($"Entity type '{typeof(TEntity).Name}' was not found in the DbContext model.");

        var primaryKey = entityType.FindPrimaryKey()
                         ?? throw new InvalidOperationException($"Entity type '{typeof(TEntity).Name}' has no primary key configured.");

        if (primaryKey.Properties.Count != 1)
        {
            throw new InvalidOperationException($"Entity type '{typeof(TEntity).Name}' must have a single primary key to use GetByIdAsNoTrackingAsync.");
        }

        return primaryKey.Properties[0];
    }

    private static Expression<Func<TEntity, bool>> BuildPrimaryKeyPredicate(IReadOnlyProperty keyProperty, TId id)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "entity");

        var propertyExpression = Expression.Call(
            typeof(EF),
            nameof(EF.Property),
            [keyProperty.ClrType],
            parameter,
            Expression.Constant(keyProperty.Name));

        var targetType = Nullable.GetUnderlyingType(keyProperty.ClrType) ?? keyProperty.ClrType;
        var convertedId = ConvertId(id, targetType);
        var constantExpression = Expression.Constant(convertedId, keyProperty.ClrType);

        var body = Expression.Equal(propertyExpression, constantExpression);
        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }

    private static object? ConvertId(TId id, Type targetType)
    {
        if (id is null)
        {
            return null;
        }

        if (targetType.IsInstanceOfType(id))
        {
            return id;
        }

        if (targetType == typeof(Guid))
        {
            return Guid.Parse(id.ToString()!);
        }

        if (targetType.IsEnum)
        {
            return Enum.Parse(targetType, id.ToString()!, ignoreCase: true);
        }

        return Convert.ChangeType(id, targetType);
    }

    private TEntity? FindTrackedEntityById(IReadOnlyProperty keyProperty, TId id)
    {
        var keyType = Nullable.GetUnderlyingType(keyProperty.ClrType) ?? keyProperty.ClrType;
        var convertedId = ConvertId(id, keyType);

        foreach (var entry in context.ChangeTracker.Entries<TEntity>())
        {
            if (entry.State == EntityState.Detached)
            {
                continue;
            }

            var currentValue = entry.Property(keyProperty.Name).CurrentValue;
            if (!Equals(currentValue, convertedId))
            {
                continue;
            }

            if (_hasDeletedAtProperty && entry.Entity is BaseEntity baseEntity && baseEntity.DeletedAt is not null)
            {
                return null;
            }

            return entry.Entity;
        }

        return null;
    }
}
