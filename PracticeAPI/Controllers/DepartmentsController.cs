using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticeAPI.DTOs;
using PracticeAPI.Services.Interfaces;

namespace PracticeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all endpoints
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<DepartmentsController> _logger;

        public DepartmentsController(IDepartmentService departmentService, ILogger<DepartmentsController> logger)
        {
            _departmentService = departmentService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin,Manager,User")] // All authenticated users can view
        [ProducesResponseType(typeof(IEnumerable<DepartmentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAllDepartments()
        {
            _logger.LogInformation("Getting all departments");
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return Ok(departments);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin,Manager,User")] // All authenticated users can view
        [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DepartmentDto>> GetDepartmentById(int id)
        {
            _logger.LogInformation("Getting department with ID: {Id}", id);
            var department = await _departmentService.GetDepartmentByIdAsync(id);

            if (department == null)
            {
                _logger.LogWarning("Department with ID {Id} not found", id);
                return NotFound(new { message = $"Department with ID {id} not found" });
            }

            return Ok(department);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")] // Only SuperAdmin and Admin can create
        [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<DepartmentDto>> CreateDepartment([FromBody] CreateDepartmentDto departmentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating new department: {Name}", departmentDto.Name);
            var createdDepartment = await _departmentService.CreateDepartmentAsync(departmentDto);

            return CreatedAtAction(
                nameof(GetDepartmentById),
                new { id = createdDepartment.Id },
                createdDepartment);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")] // Only SuperAdmin and Admin can update
        [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<DepartmentDto>> UpdateDepartment(int id, [FromBody] UpdateDepartmentDto departmentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating department with ID: {Id}", id);
            var updatedDepartment = await _departmentService.UpdateDepartmentAsync(id, departmentDto);

            if (updatedDepartment == null)
            {
                _logger.LogWarning("Department with ID {Id} not found for update", id);
                return NotFound(new { message = $"Department with ID {id} not found" });
            }

            return Ok(updatedDepartment);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")] // Only SuperAdmin can delete
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            _logger.LogInformation("Deleting department with ID: {Id}", id);
            var deleted = await _departmentService.DeleteDepartmentAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("Department with ID {Id} not found for deletion", id);
                return NotFound(new { message = $"Department with ID {id} not found" });
            }

            return NoContent();
        }
    }
}
