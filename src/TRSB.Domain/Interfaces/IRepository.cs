// src/TRSB.Domain/Interfaces/IRepository.cs
using System.Linq.Expressions;

namespace TRSB.Domain.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(Guid id);
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task AddAsync(TEntity entity);
    Task<int> SaveChangesAsync();
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
    void Update(TEntity entity);
}
