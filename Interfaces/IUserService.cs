using SchoolSystem.Models;
using System.Security.Claims;
using static SchoolSystem.Models.User;

namespace SchoolSystem.Interfaces
{
    public interface IUserService
    {
        Task<string> GenerateUsername();
        Task<List<StudentRoleViewModel>> GetStudentsWithRolesAsync();
        Task<List<TeacherRoleViewModel>> GetTeachersWithRolesAsync();
        Task<string> GenerateTempPassword(string userId);
        Task<bool> isValidUserStudent(long studentId);
        Task<bool> isValidUserTeacher(long teacherId);

        string GetUserRole();
        string GetUserId();
        UserHeaderDto GetTeacherHeaderInfo(string userId = null, long? teacherId = null);
        UserHeaderDto GetStudentHeaderInfo(string userId = null, long? studentId = null);
        UserHeaderDto GetUserHeaderInfo();
    }
}
