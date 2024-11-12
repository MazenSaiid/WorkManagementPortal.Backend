using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.Logic.Interfaces
{
    
        public interface IGenericRepository<T> where T : class
        {
            Task AddAsync(T entity);
            Task UpdateAsync(int id, T entity);
            Task DeleteAsync(int id);
            Task<IEnumerable<T>> GetAllAsync();
            Task<T> GetByIdAsync(int id);
        }
    
}
