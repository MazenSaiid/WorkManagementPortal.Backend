using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Context;
using WorkManagementPortal.Backend.Infrastructure.Models;
using WorkManagementPortal.Backend.Logic.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WorkManagementPortal.Backend.Logic.Services
{

    public class GenericRepository<T, TKey> : IGenericRepository<T, TKey> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly IPaginationHelper<T> _paginationHelper;
        public GenericRepository(IPaginationHelper<T> paginationHelper, ApplicationDbContext context)
        {
            _context = context;
            _paginationHelper = paginationHelper;
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TKey id)
        {
            var entityToDelete = await _context.Set<T>().FindAsync(id);
            if (entityToDelete is not null)
            {
                _context.Set<T>().Remove(entityToDelete);
                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteRangeAsync(IEnumerable<T> values)
        {
          _context.Set<T>().RemoveRange(values);
          await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<T>> GetAllAsync(int page, int pageSize, Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            // Start with the base query
            var query = _context.Set<T>().AsNoTracking();

            // Apply the include
            if (include != null)
            {
                query = include(query);
            }

            // Get the paginated result
            var paginatedResult = await _paginationHelper.GetPagedResult(query, page, pageSize);

            return paginatedResult;
        }

        // Get All without Pagination and optional Include
        public async Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            // Start with the base query
            var query = _context.Set<T>().AsNoTracking();

            // Apply the include (e.g., for eager loading) if it's provided
            if (include != null)
            {
                query = include(query);
            }

            // Execute the query and return the results
            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(TKey id) =>
         await _context.Set<T>().FindAsync(id);

        public async Task<T> GetByIdAsync(TKey id, Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            IQueryable<T> query = _context.Set<T>();

            // Apply eager loading includes if provided
            if (include != null)
            {
                query = include(query);
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<TKey>(e, "Id").Equals(id));
        }


        public async Task UpdateAsync(T entityToUpdate, T updatedEntity)
        {
            if (entityToUpdate == null || updatedEntity == null)
            {
                throw new ArgumentNullException("Both entities must be provided for the update.");
            }

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                // Skip the primary key or any non-updatable fields (optional)
                if (property.Name == "Id") // Adjust as needed for your primary key field
                    continue;

                var currentValue = property.GetValue(entityToUpdate);
                var newValue = property.GetValue(updatedEntity);

                // If the new value is not null and different from the current value, update it
                if (newValue != null && !newValue.Equals(currentValue))
                {
                    property.SetValue(entityToUpdate, newValue);
                }
            }

            // Mark the entity as updated and save the changes to the database
            _context.Set<T>().Update(entityToUpdate);
            await _context.SaveChangesAsync();
        }

    }

}
