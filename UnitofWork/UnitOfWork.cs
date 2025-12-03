using Microsoft.EntityFrameworkCore;

namespace UnitofWork
{
    public interface IUnitOfWork<TContext> : IDisposable where TContext : DbContext
    {
        IRepository<T> GetRepository<T>() where T : class, new();
        Task<int> SaveChangesAsync();
        TContext Context { get; }
    }
    public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
    {
        private bool _disposed = false;
        private readonly TContext _context;
        private readonly Dictionary<string, object> _repositories;

        public TContext Context => _context;

        public UnitOfWork(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _repositories = [];
        }

        public IRepository<T> GetRepository<T>() where T : class, new()
        {
            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(Repository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context) ?? throw new InvalidOperationException($"Failed to create an instance of {repositoryType.MakeGenericType(typeof(T))}");
                _repositories.Add(type, repositoryInstance);
            }

            return (IRepository<T>)_repositories[type];

        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }


        // Dispose Pattern
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //Destructor
        ~UnitOfWork() => Dispose(false);
    }
}
