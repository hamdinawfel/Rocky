using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Rocky_DataAccess.Repository
{
    public interface IRepository<T> where T : class
    {
        T Find(int id);

        IEnumerable<T> FindAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null,
            bool isTracking = true);

        T FirstOrDefault(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = null,
            bool isTracking = true);

        void Add(T entity);

        void AddRange(IEnumerable<T> entities);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);

        void SaveChanges();
    }
}
