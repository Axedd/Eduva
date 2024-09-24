using SchoolSystem.Models;

namespace SchoolSystem.Interfaces
{
    public interface IUserService
    {
        Task<string> GenerateUsername();
        Task<List<StudentRoleViewModel>> GetStudentsWithRolesAsync();
        Task<bool> isValidUserStudent(long studentId);
        Task<bool> isValidUserTeacher(long teacherId);
    }
}
