using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticeAPI.DTOs;
using PracticeAPI.Services.Interfaces;

namespace PracticeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all endpoints
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin,Manager,User")] // All authenticated users can view
        [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAllEmployees(
            [FromQuery] int? departmentId = null,
            [FromQuery] decimal? minSalary = null,
            [FromQuery] decimal? maxSalary = null)
        {
            _logger.LogInformation("Getting all employees with filters - DepartmentId: {DepartmentId}, MinSalary: {MinSalary}, MaxSalary: {MaxSalary}",
                departmentId, minSalary, maxSalary);

            var employees = await _employeeService.GetAllEmployeesAsync(departmentId, minSalary, maxSalary);
            return Ok(employees);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin,Manager,User")] // All authenticated users can view
        [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeById(int id)
        {
            _logger.LogInformation("Getting employee with ID: {Id}", id);
            var employee = await _employeeService.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                _logger.LogWarning("Employee with ID {Id} not found", id);
                return NotFound(new { message = $"Employee with ID {id} not found" });
            }

            return Ok(employee);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin,Manager")] // SuperAdmin, Admin, and Manager can create
        [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee([FromBody] CreateEmployeeDto employeeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating new employee: {Name}", employeeDto.Name);
            var createdEmployee = await _employeeService.CreateEmployeeAsync(employeeDto);

            return CreatedAtAction(
                nameof(GetEmployeeById),
                new { id = createdEmployee.Id },
                createdEmployee);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin,Manager")] // SuperAdmin, Admin, and Manager can update
        [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<EmployeeDto>> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto employeeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating employee with ID: {Id}", id);
            var updatedEmployee = await _employeeService.UpdateEmployeeAsync(id, employeeDto);

            if (updatedEmployee == null)
            {
                _logger.LogWarning("Employee with ID {Id} not found for update", id);
                return NotFound(new { message = $"Employee with ID {id} not found" });
            }

            return Ok(updatedEmployee);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")] // Only SuperAdmin and Admin can delete
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            _logger.LogInformation("Deleting employee with ID: {Id}", id);
            var deleted = await _employeeService.DeleteEmployeeAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("Employee with ID {Id} not found for deletion", id);
                return NotFound(new { message = $"Employee with ID {id} not found" });
            }

            return NoContent();
        }
    }
}
