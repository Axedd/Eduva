namespace SchoolSystem.Interfaces
{
    public interface IValidationService
    {

        Task<bool> IsValidSubjectAsync(string subjectName);
    }
}
