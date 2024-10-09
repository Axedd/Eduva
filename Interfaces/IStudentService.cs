using SchoolSystem.Models;

namespace SchoolSystem.Interfaces
{
    public interface IStudentService
    {
        Task<long> GenerateStudentId();
        Task<int> GetClassIdOfStudentAsync();
        Task UpdateStudentAsync(Student student);
        Task<List<Student>> GetStudentsByClassAsync(int classId);
        Task<List<Student>> GetAllStudents();
        Task<List<Student>> GetAllRegisteredStudents();
        Task<Student> GetStudentById(long studentId);
    }
}
