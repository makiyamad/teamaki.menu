using System.Collections.Generic;

namespace TeaMaki.Persistence
{
    public interface IRepository<T>
        {
            public T Get(string id);
            public IEnumerable<T> GetAll(int pageSize, int pageNumber);
            public void InsertOrUpdate(T element);
        }
}
