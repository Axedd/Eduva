using Microsoft.AspNetCore.Identity;
using SchoolSystem.Models;

namespace SchoolSystem.Interfaces
{
    public interface IUserRegistrationService
    {
        Task<IdentityResult> RegisterStudentAsync(ApplicationUser user, long studentId);
        Task<IdentityResult> RegisterTeacherAsync(ApplicationUser user, long teacherId);
    }
}
