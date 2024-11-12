using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Context;
using WorkManagementPortal.Backend.Infrastructure.Models;
using WorkManagementPortal.Backend.Logic.Interfaces;

namespace WorkManagementPortal.Backend.Logic.Services
{

    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entityToDelete = await _context.Set<T>().FindAsync(id);
            if (entityToDelete is not null)
            {
                _context.Set<T>().Remove(entityToDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync() =>
           await _context.Set<T>().AsNoTracking().ToListAsync();


        public async Task<T> GetByIdAsync(int id) =>
            await _context.Set<T>().FindAsync(id);


        public async Task UpdateAsync(int id, T entity)
        {
            var entityToUpdate = await _context.Set<T>().FindAsync(id);
            if (entityToUpdate is not null)
            {
                _context.Set<T>().Update(entity);
                await _context.SaveChangesAsync();

            }
        }
    }

}
