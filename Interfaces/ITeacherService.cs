using SchoolSystem.Models;

namespace SchoolSystem.Interfaces
{
    public interface ITeacherService
    {
        Task<Teacher> GetTeacherById(long teacherId);
    }
}
