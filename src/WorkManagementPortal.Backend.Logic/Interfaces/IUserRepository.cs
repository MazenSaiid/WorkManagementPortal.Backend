using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.Logic.Interfaces
{
    public interface IUserRepository :IGenericRepository<User>
    {
        Task<IEnumerable<User>> GetAllSupervisorsAsync();
        Task<IEnumerable<User>> GetAllTeamLeadersAsync();
        Task<IEnumerable<User>> GetAllEmployeesAsync();
    }
}
