using SchoolSystem.Dtos;
using SchoolSystem.Models;

namespace SchoolSystem.Interfaces
{
    public interface IStudentService
    {
        Task<long> GenerateStudentId();
        Task<int> GetClassIdOfStudentAsync(string? userId = null, long? studentId = null);
        Task<List<StudentGeneralInfoDto>> GetStudentsByClassAsync(int classId);
        Task<List<Student>> GetAllStudents();
        Task<List<Student>> GetAllRegisteredStudents();
        Task<Student> GetStudentById(long studentId);
        Task<StudentGeneralInfoDto> GetStudentGeneralInfoAsync(long studentId);

        Task RegisterStudentAsync(Student student);
        Task UpdateStudentAsync(Student student);

        Task AddStudentAsync(Student newStudent);
    }
}
