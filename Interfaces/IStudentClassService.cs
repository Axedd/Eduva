using SchoolSystem.Models;
using static SchoolSystem.Models.Student;

namespace SchoolSystem.Interfaces
{
    public interface IStudentClassService
    {
        Task<List<StudentClass>> GetStudentClassesAsync();
        Task<int> GetStudentClassIdByUserId(string userId);
        Task<List<StudentClassSubjects>> GetStudentClassSubjectsAsync(long studentClassId);
        Task<string?> GetStudentClassName(long studentClassId);
        Task<StudentClass> GetStudentClassByIdAsync(long studentClassId);
        Task<List<StudentDto>> GetAllStudentsFromClassIdAsync(long studentClassId);

	}
}
