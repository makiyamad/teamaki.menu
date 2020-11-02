using System;
using System.Collections.Generic;
using System.Linq;

namespace TeaMaki.Persistence
{
    public class RavenDbRepository<T> : IRepository<T>
    {
        private readonly IRavenDbContext _context;
        public RavenDbRepository(IRavenDbContext context)
        {
            _context = context;
        }

        public T Get(string id)
        {
            try
            {
                using var session = _context.store.OpenSession();
                var element = session.Load<T>(id);
                return element;
            }
            catch (Exception ex)
            {
                throw new RepositoryException(ex.Message, ex.InnerException);
            }
        }

        public IEnumerable<T> GetAll(int pageSize, int pageNumber)
        {
            try
            {
                using var session = _context.store.OpenSession();
                var elements = session.Query<T>()
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize);

                return elements;
            }
            catch (Exception ex)
            {
                throw new RepositoryException(ex.Message, ex.InnerException);
            }
        }

        public void InsertOrUpdate(T element)
        {
            try
            {
                using var session = _context.store.OpenSession();
                session.Store(element);
                session.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new RepositoryException(ex.Message, ex.InnerException);
            }
        }
    }

    public class RepositoryException : Exception
    {
        public RepositoryException(string message, Exception exception) : base(message, exception)
        {

        }
    }
}
