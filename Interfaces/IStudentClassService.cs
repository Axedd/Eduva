using SchoolSystem.Models;

namespace SchoolSystem.Interfaces
{
    public interface IStudentClassService
    {
        Task<List<StudentClass>> GetStudentClassesAsync();
        Task<int> GetStudentClassIdByUserId(string userId);
        Task<List<StudentClassSubjects>> GetStudentClassSubjectsAsync(long studentClassId);
        Task<string?> GetStudentClassName(long studentClassId);
        Task<StudentClass> GetStudentClassByIdAsync(long studentClassId);

	}
}
