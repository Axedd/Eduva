using SchoolSystem.Models;

namespace SchoolSystem.Interfaces
{
    public interface IClassService
    {
        Task<List<StudentClass>> GetAllClassesAsync();
    }
}
