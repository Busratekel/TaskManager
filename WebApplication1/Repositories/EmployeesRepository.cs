using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using Services;

namespace Repositories
{
    public class EmployeesRepository : IEmployeesRepository
    {
        private readonly ApplicationDbContext _context;
        //private readonly ISharePointService _sharePointService;

        public EmployeesRepository(ApplicationDbContext context)
        //ISharePointService sharePointService)
        {
            _context = context;
            //_sharePointService = sharePointService;
        }

        public async Task<List<Employees>> GetEmployeesAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task UpdateEmployeesAsync(List<Employees> employees)
        {
            _context.Employees.UpdateRange(employees);
            await _context.SaveChangesAsync();
        }

        // public async Task SyncEmployeesFromSharePointAsync()
        // {
        //     //var employees = await _sharePointService.GetEmployeesFromSharePointAsync();
        //     foreach (var employee in employees)
        //     {
        //         var existingEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == employee.Email);
        //         if (existingEmployee == null)
        //         {
        //             _context.Employees.Add(employee);
        //         }
        //         else
        //         {
        //             existingEmployee.Name = employee.Name;
        //             existingEmployee.BirthDate = employee.BirthDate;
        //             existingEmployee.HireDate = employee.HireDate;
        //             existingEmployee.BccEmails = employee.BccEmails;
        //         }
        //     }
        //     await _context.SaveChangesAsync();
        // }
    }
}