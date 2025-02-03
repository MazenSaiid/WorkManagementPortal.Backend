using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.Logic.Interfaces
{
    
        public interface IGenericRepository<T, TKey> where T : class
        {
            Task AddAsync(T entity);
            Task UpdateAsync( T entityToUpdate, T entity);
            Task DeleteAsync(TKey id);
            Task DeleteRangeAsync(IEnumerable<T> values);
            Task<PagedResult<T>> GetAllAsync(int page, int pageSize, Func<IQueryable<T>, IQueryable<T>> include = null);
            Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>> include = null);
            Task<T> GetByIdAsync(TKey id);
            Task<T> GetByIdAsync(TKey id, Func<IQueryable<T>, IQueryable<T>> include = null);
    }
    
}
