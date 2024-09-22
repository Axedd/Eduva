namespace SchoolSystem.Interfaces
{
    public interface IIdValidationService
    {
        Task<bool> IsValidStudentIdAsync(long studentId);
        Task<bool> IsValidTeacherIdAsync(long teacherId);
        Task<bool> IsValidStudentClassIdAsync(int studentClassId);

    }
}
