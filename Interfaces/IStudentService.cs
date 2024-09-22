namespace SchoolSystem.Interfaces
{
    public interface IStudentService
    {
        Task<long> GenerateStudentId();
    }
}
