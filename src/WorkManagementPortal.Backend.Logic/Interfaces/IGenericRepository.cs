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
            Task<IEnumerable<T>> GetAllAsync();
            Task<T> GetByIdAsync(TKey id);
        }
    
}
