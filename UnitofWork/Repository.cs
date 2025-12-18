using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using UnitofWork.Helper;

namespace UnitofWork
{
    public interface IRepository<T> where T : class
    {
        void ChangeTable(string table);
        ValueTask<EntityEntry<T>> InsertAsync(T entity, CancellationToken cancellationToken = default);
        T Insert(T entity);
        void Update(T entity);
        void Attach(T entity);
        public void UpdateFields(T entity, params Expression<Func<T, object>>[] updatedProperties);
        public string GetPropertyName<TProperty>(Expression<Func<T, TProperty>> propertyExpression);

        IQueryable<T> GetAll(Expression<Func<T, bool>>? predicate = null,
                             Func<IQueryable<T>, IQueryable<T>>? include = null,
                             Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                             bool disableTracking = false, bool IgnoreQueryFilter = false);

        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null,
                                  Func<IQueryable<T>, IQueryable<T>>? include = null,
                                  Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                  bool disableTracking = false, bool IgnoreQueryFilter = false);


        TResult? GetFirstOrDefaultSelector<TResult>(Expression<Func<T, TResult>>? selector = null,
                                           Expression<Func<T, bool>>? predicate = null,
                                           Func<IQueryable<T>, IQueryable<T>>? include = null,
                                           Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                           bool disableTracking = false, bool IgnoreQueryFilter = false);

        Task<TResult?> GetFirstOrDefaultSelectorAsync<TResult>(Expression<Func<T, TResult>>? selector = null,
                                                       Expression<Func<T, bool>>? predicate = null,
                                                       Func<IQueryable<T>, IQueryable<T>>? include = null,
                                                       Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                                       bool disableTracking = false, bool IgnoreQueryFilter = false);

        T? GetFirstOrDefault(Expression<Func<T, bool>>? predicate = null,
                             Func<IQueryable<T>, IQueryable<T>>? include = null,
                             Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                             bool disableTracking = false, bool IgnoreQueryFilter = false);

        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>>? predicate = null,
                                        Func<IQueryable<T>, IQueryable<T>>? include = null,
                                        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                        bool disableTracking = false, bool IgnoreQueryFilter = false);

        Task<PaginatedList<T>> GetToPageListAsync(Expression<Func<T, bool>>? predicate = null,
                                                  Func<IQueryable<T>, IQueryable<T>>? include = null,
                                                  Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                                  bool disableTracking = false, bool IgnoreQueryFilter = false,
                                                  int pageSize = 10, int pageIndex = 1);

        PaginatedList<T> GetToPageList(Expression<Func<T, bool>>? predicate = null,
                                       Func<IQueryable<T>, IQueryable<T>>? include = null,
                                       Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                       bool disableTracking = false, bool IgnoreQueryFilter = false,
                                       int pageSize = 10, int pageIndex = 1);
    }

    public class Repository<T> : IRepository<T> where T : class, new()
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;
        public Repository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public void ChangeTable(string table)
        {
            throw new NotImplementedException();
        }

        public void Update(T entity)
        {
            _ = _dbSet.Update(entity);
        }

        public T? Find(params object?[]? keyValues)
        {
            return _dbSet.Find(keyValues);
        }

        public ValueTask<T?> FindAsync(params object?[]? keyValues)
        {
            return _dbSet.FindAsync(keyValues);
        }

        public T Insert(T entity)
        {
            return _dbSet.Add(entity: entity).Entity;
        }

        public async ValueTask<EntityEntry<T>> InsertAsync(T entity, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AddAsync(entity: entity, cancellationToken);
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>>? predicate = null,
                                    Func<IQueryable<T>, IQueryable<T>>? include = null,
                                    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                    bool disableTracking = false, bool IgnoreQueryFilter = false)
        {
            IQueryable<T> queryable = _dbSet;

            if (disableTracking)
            {
                queryable = queryable.AsNoTracking();
            }

            if (predicate != null)
            {
                queryable = queryable.Where(predicate);
            }

            if (include != null)
            {
                queryable = include(queryable);
            }

            if (orderBy != null)
            {
                queryable = orderBy(queryable);
            }

            if (IgnoreQueryFilter)
            {
                queryable = queryable.IgnoreQueryFilters();
            }

            return queryable;
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null,
                                               Func<IQueryable<T>, IQueryable<T>>? include = null,
                                               Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                               bool disableTracking = false, bool IgnoreQueryFilter = false)
        {
            IQueryable<T> queryable = _dbSet;

            if (disableTracking)
            {
                queryable = queryable.AsNoTracking();
            }

            if (predicate != null)
            {
                queryable = queryable.Where(predicate);
            }

            if (include != null)
            {
                queryable = include(queryable);
            }

            if (orderBy != null)
            {
                queryable = orderBy(queryable);
            }

            if (IgnoreQueryFilter)
            {
                queryable = queryable.IgnoreQueryFilters();
            }

            return await queryable.ToListAsync();
        }

        public virtual TResult? GetFirstOrDefaultSelector<TResult>(Expression<Func<T, TResult>>? selector = null,
                                                                  Expression<Func<T, bool>>? predicate = null,
                                                                  Func<IQueryable<T>, IQueryable<T>>? include = null,
                                                                  Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                                                  bool disableTracking = false, bool IgnoreQueryFilter = false)
        {
            IQueryable<T> queryable = _dbSet;

            if (disableTracking)
            {
                queryable = queryable.AsNoTracking();
            }

            if (predicate != null)
            {
                queryable = queryable.Where(predicate);
            }

            if (include != null)
            {
                queryable = include(queryable);
            }

            if (orderBy != null)
            {
                queryable = orderBy(queryable);
            }

            if (IgnoreQueryFilter)
            {
                queryable = queryable.IgnoreQueryFilters();
            }

            return selector != null ? queryable.Select(selector).FirstOrDefault() : (TResult?)(object?)queryable.FirstOrDefault();
        }

        public virtual async Task<TResult?> GetFirstOrDefaultSelectorAsync<TResult>(Expression<Func<T, TResult>>? selector = null,
                                                                   Expression<Func<T, bool>>? predicate = null,
                                                                   Func<IQueryable<T>, IQueryable<T>>? include = null,
                                                                   Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                                                   bool disableTracking = false,
                                                                   bool IgnoreQueryFilter = false)
        {
            IQueryable<T> queryable = _dbSet;

            if (disableTracking)
            {
                queryable = queryable.AsNoTracking();
            }

            if (predicate != null)
            {
                queryable = queryable.Where(predicate);
            }

            if (include != null)
            {
                queryable = include(queryable);
            }

            if (orderBy != null)
            {
                queryable = orderBy(queryable);
            }

            if (IgnoreQueryFilter)
            {
                queryable = queryable.IgnoreQueryFilters();
            }

            return selector != null
                ? await queryable.Select(selector).FirstOrDefaultAsync()
                : (TResult?)(object?)await queryable.FirstOrDefaultAsync();
        }

        public T? GetFirstOrDefault(Expression<Func<T, bool>>? predicate = null,
                                            Func<IQueryable<T>, IQueryable<T>>? include = null,
                                            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                            bool disableTracking = false, bool IgnoreQueryFilter = false)
        {
            IQueryable<T> queryable = _dbSet;

            if (disableTracking)
            {
                queryable = queryable.AsNoTracking();
            }

            if (predicate != null)
            {
                queryable = queryable.Where(predicate);
            }

            if (include != null)
            {
                queryable = include(queryable);
            }

            if (orderBy != null)
            {
                queryable = orderBy(queryable);
            }

            if (IgnoreQueryFilter)
            {
                queryable = queryable.IgnoreQueryFilters();
            }

            return queryable.FirstOrDefault();
        }

        public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>>? predicate = null,
                                                              Func<IQueryable<T>, IQueryable<T>>? include = null,
                                                              Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                                              bool disableTracking = false, bool IgnoreQueryFilter = false)
        {
            IQueryable<T> queryable = _dbSet;

            if (disableTracking)
            {
                queryable = queryable.AsNoTracking();
            }

            if (predicate != null)
            {
                queryable = queryable.Where(predicate);
            }

            if (include != null)
            {
                queryable = include(queryable);
            }

            if (orderBy != null)
            {
                queryable = orderBy(queryable);
            }

            if (IgnoreQueryFilter)
            {
                queryable = queryable.IgnoreQueryFilters();
            }

            return await queryable.FirstOrDefaultAsync();
        }

        public Task<PaginatedList<T>> GetToPageListAsync(Expression<Func<T, bool>>? predicate = null,
                                                         Func<IQueryable<T>, IQueryable<T>>? include = null,
                                                         Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                                         bool disableTracking = false, bool IgnoreQueryFilter = false,
                                                         int pageSize = 10, int pageIndex = 1)
        {
            IQueryable<T> queryable = _dbSet;

            if (disableTracking)
            {
                queryable = queryable.AsNoTracking();
            }

            if (predicate != null)
            {
                queryable = queryable.Where(predicate);
            }

            if (include != null)
            {
                queryable = include(queryable);
            }

            if (orderBy != null)
            {
                queryable = orderBy(queryable);
            }

            if (IgnoreQueryFilter)
            {
                queryable = queryable.IgnoreQueryFilters();
            }

            return queryable.ToPageListAsync(pageIndex, pageSize);
        }

        public PaginatedList<T> GetToPageList(Expression<Func<T, bool>>? predicate = null,
                                              Func<IQueryable<T>, IQueryable<T>>? include = null,
                                              Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                              bool disableTracking = false, bool IgnoreQueryFilter = false,
                                              int pageSize = 10, int pageIndex = 1)
        {
            IQueryable<T> queryable = _dbSet;

            if (disableTracking)
            {
                queryable = queryable.AsNoTracking();
            }

            if (predicate != null)
            {
                queryable = queryable.Where(predicate);
            }

            if (include != null)
            {
                queryable = include(queryable);
            }

            if (orderBy != null)
            {
                queryable = orderBy(queryable);
            }

            if (IgnoreQueryFilter)
            {
                queryable = queryable.IgnoreQueryFilters();
            }

            return queryable.ToPageList(pageIndex, pageSize);
        }

        public void Attach(T entity)
        {
            _ = _dbSet.Attach(entity);
        }

        public void UpdateFields(T entity, params Expression<Func<T, object>>[] updatedProperties)
        {
            // Attach thực thể với Id
            _ = _dbSet.Attach(entity);

            // Đánh dấu các trường cần cập nhật
            foreach (Expression<Func<T, object>> property in updatedProperties)
            {
                string propertyName = ((MemberExpression)property.Body).Member.Name;
                _context.Entry(entity).Property(propertyName).IsModified = true;
            }
        }

        public string GetPropertyName<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            return propertyExpression.Body is MemberExpression memberExpression
                ? memberExpression.Member.Name
                : throw new ArgumentException("Expression must be a member expression", nameof(propertyExpression));
        }
    }
}
