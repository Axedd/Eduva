using AuthWebApp.Models;

namespace SchoolSystem.Interfaces
{
    public interface IStudentClassService
    {
        Task<List<StudentClass>> GetStudentClassesAsync();
    }
}
