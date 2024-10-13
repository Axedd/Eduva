using SchoolSystem.Models;

namespace SchoolSystem.Interfaces
{
    public interface ITeacherService
    {
        Task<Teacher> GetTeacherById(long teacherId);
        Task UpdateTeacherAsync(Teacher teacher);
        Task<long> GetTeacherByUserId(string userId);
        Task<TeacherDto> GetTeacherWithStudentClassesAsync(long teacherId, bool includeSubjects = false);
        Task<List<TeacherDto>> GetTeachersAsync(bool includeSubjects, bool includeStudentClasses);
    }
}
