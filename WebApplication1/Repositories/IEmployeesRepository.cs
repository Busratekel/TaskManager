using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace Repositories
{
    public interface IEmployeesRepository{
        Task<List<Employees>> GetEmployeesAsync();
    }
    // {
    //     Task<List<Employee>> GetEmployeesAsync();
    //     Task UpdateEmployeesAsync(List<Employee> employees);
    //     Task SyncEmployeesFromSharePointAsync();
    // }
} 