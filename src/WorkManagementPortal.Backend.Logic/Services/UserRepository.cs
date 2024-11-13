using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Context;
using WorkManagementPortal.Backend.Infrastructure.Models;
using WorkManagementPortal.Backend.Logic.Interfaces;

namespace WorkManagementPortal.Backend.Logic.Services
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<IEnumerable<User>> GetAllEmployeesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllSupervisorsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllTeamLeadersAsync()
        {
            throw new NotImplementedException();
        }
    }
}
