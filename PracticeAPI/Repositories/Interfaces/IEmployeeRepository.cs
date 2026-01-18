using PracticeAPI.Models;

namespace PracticeAPI.Repositories.Interfaces
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<IEnumerable<Employee>> GetAllWithDepartmentAsync();
        Task<Employee?> GetByIdWithDepartmentAsync(int id);
        Task<IEnumerable<Employee>> GetByDepartmentIdAsync(int departmentId);
        Task<IEnumerable<Employee>> GetBySalaryRangeAsync(decimal minSalary, decimal maxSalary);
    }
}
