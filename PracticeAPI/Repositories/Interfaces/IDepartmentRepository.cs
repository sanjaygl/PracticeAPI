using PracticeAPI.Models;

namespace PracticeAPI.Repositories.Interfaces
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Task<bool> HasEmployeesAsync(int departmentId);
    }
}
