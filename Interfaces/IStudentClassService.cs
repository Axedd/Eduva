using SchoolSystem.Models;
using static SchoolSystem.Models.Student;

namespace SchoolSystem.Interfaces
{
    public interface IStudentClassService
    {
        Task<List<StudentClass>> GetStudentClassesAsync();
        Task<int> GetStudentClassIdAsync(string userId = null, long? studentId = null);
        Task<List<StudentClassSubjects>> GetStudentClassSubjectsAsync(long studentClassId);
        Task<string?> GetStudentClassName(long studentClassId);
        Task<StudentClass> GetStudentClassByIdAsync(long studentClassId);
        Task<List<StudentDto>> GetAllStudentsFromClassIdAsync(long studentClassId);

	}
}
