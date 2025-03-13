using Microsoft.AspNetCore.Mvc;
using Models;
using Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeesRepository _employeesRepository;

        public EmployeeController(IEmployeesRepository employeeRepository)
        {
            _employeesRepository = employeeRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employees>>> GetEmployees()
        {
            return await _employeesRepository.GetEmployeesAsync();
        }

        // [HttpPost("sync")]
        // public async Task<IActionResult> SyncEmployees()
        // {
        //     await _employeeRepository.SyncEmployeesFromSharePointAsync();
        //     return Ok();
        // }

        // [HttpPut]
        // public async Task<IActionResult> UpdateEmployees(List<Employee> employees)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }

        //     await _employeeRepository.UpdateEmployeesAsync(employees);
        //     return Ok();
        // }
    }
}