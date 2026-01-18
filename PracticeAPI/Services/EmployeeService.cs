using PracticeAPI.DTOs;
using PracticeAPI.Models;
using PracticeAPI.Repositories.Interfaces;
using PracticeAPI.Services.Interfaces;

namespace PracticeAPI.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public EmployeeService(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(int? departmentId = null, decimal? minSalary = null, decimal? maxSalary = null)
        {
            IEnumerable<Employee> employees;

            if (departmentId.HasValue)
            {
                employees = await _employeeRepository.GetByDepartmentIdAsync(departmentId.Value);
            }
            else if (minSalary.HasValue && maxSalary.HasValue)
            {
                employees = await _employeeRepository.GetBySalaryRangeAsync(minSalary.Value, maxSalary.Value);
            }
            else
            {
                employees = await _employeeRepository.GetAllWithDepartmentAsync();
            }

            return employees.Select(e => MapToDto(e));
        }

        public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdWithDepartmentAsync(id);
            return employee == null ? null : MapToDto(employee);
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto employeeDto)
        {
            if (employeeDto.DepartmentId.HasValue)
            {
                var departmentExists = await _departmentRepository.ExistsAsync(employeeDto.DepartmentId.Value);
                if (!departmentExists)
                {
                    throw new InvalidOperationException($"Department with ID {employeeDto.DepartmentId} does not exist.");
                }
            }

            var employee = new Employee
            {
                Name = employeeDto.Name,
                Gender = employeeDto.Gender,
                Salary = employeeDto.Salary,
                DepartmentId = employeeDto.DepartmentId
            };

            var createdEmployee = await _employeeRepository.AddAsync(employee);
            var employeeWithDept = await _employeeRepository.GetByIdWithDepartmentAsync(createdEmployee.Id);
            return MapToDto(employeeWithDept!);
        }

        public async Task<EmployeeDto?> UpdateEmployeeAsync(int id, UpdateEmployeeDto employeeDto)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return null;

            if (employeeDto.DepartmentId.HasValue)
            {
                var departmentExists = await _departmentRepository.ExistsAsync(employeeDto.DepartmentId.Value);
                if (!departmentExists)
                {
                    throw new InvalidOperationException($"Department with ID {employeeDto.DepartmentId} does not exist.");
                }
            }

            employee.Name = employeeDto.Name;
            employee.Gender = employeeDto.Gender;
            employee.Salary = employeeDto.Salary;
            employee.DepartmentId = employeeDto.DepartmentId;

            await _employeeRepository.UpdateAsync(employee);

            var updatedEmployee = await _employeeRepository.GetByIdWithDepartmentAsync(id);
            return MapToDto(updatedEmployee!);
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return false;

            await _employeeRepository.DeleteAsync(employee);
            return true;
        }

        private static EmployeeDto MapToDto(Employee employee)
        {
            return new EmployeeDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Gender = employee.Gender,
                Salary = employee.Salary,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.Name
            };
        }
    }
}
