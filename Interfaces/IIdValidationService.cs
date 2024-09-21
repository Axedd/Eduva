namespace SchoolSystem.Interfaces
{
    public interface IIdValidationService
    {
        Task<bool> IsValidStudentIdAsync(int studentId);
        Task<bool> IsValidStudentClassIdAsync(int studentClassId);

    }
}
