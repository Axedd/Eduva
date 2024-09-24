using AuthWebApp.Models;

namespace SchoolSystem.Interfaces
{
    public interface IStudentService
    {
        Task<long> GenerateStudentId();

        Task<Student> GetStudentById(long studentId);
    }
}
