namespace SchoolSystem.Interfaces
{
    public interface IUserService
    {
        Task<string> GenerateUsername();
        Task<bool> isValidUserStudent(long studentId);
        Task<bool> isValidUserTeacher(long teacherId);
    }
}
