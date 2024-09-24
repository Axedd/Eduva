using SchoolSystem.Models;

namespace SchoolSystem.Interfaces
{
    public interface IStudentService
    {
        Task<long> GenerateStudentId();

        Task<List<Student>> GetAllStudents();
        Task<List<Student>> GetAllRegisteredStudents();
        Task<Student> GetStudentById(long studentId);
    }
}
