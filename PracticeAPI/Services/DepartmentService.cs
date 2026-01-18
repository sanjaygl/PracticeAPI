using PracticeAPI.DTOs;
using PracticeAPI.Models;
using PracticeAPI.Repositories.Interfaces;
using PracticeAPI.Services.Interfaces;

namespace PracticeAPI.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync()
        {
            var departments = await _departmentRepository.GetAllAsync();
            return departments.Select(d => MapToDto(d));
        }

        public async Task<DepartmentDto?> GetDepartmentByIdAsync(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            return department == null ? null : MapToDto(department);
        }

        public async Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto departmentDto)
        {
            var department = new Department
            {
                Name = departmentDto.Name
            };

            var createdDepartment = await _departmentRepository.AddAsync(department);
            return MapToDto(createdDepartment);
        }

        public async Task<DepartmentDto?> UpdateDepartmentAsync(int id, UpdateDepartmentDto departmentDto)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
                return null;

            department.Name = departmentDto.Name;
            await _departmentRepository.UpdateAsync(department);

            return MapToDto(department);
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
                return false;

            var hasEmployees = await _departmentRepository.HasEmployeesAsync(id);
            if (hasEmployees)
            {
                throw new InvalidOperationException($"Cannot delete department with ID {id} because it has associated employees.");
            }

            await _departmentRepository.DeleteAsync(department);
            return true;
        }

        private static DepartmentDto MapToDto(Department department)
        {
            return new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name
            };
        }
    }
}
