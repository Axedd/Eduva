using SchoolSystem.Models;

namespace SchoolSystem.Interfaces
{
    public interface ITeacherService
    {
        Task<Teacher> GetTeacherById(long teacherId);
        Task UpdateTeacherAsync(Teacher teacher);
        Task<long> GetTeacherByUserId(string userId);
        Task<TeacherDto> GetTeachersWithStudentClassesAsync(long teacherId);
        Task<List<TeacherDto>> GetAllTeachersWithSubjectsAsync();
    }
}
