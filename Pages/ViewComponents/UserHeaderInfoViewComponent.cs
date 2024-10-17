using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Interfaces;
using SchoolSystem.Models;
using System.Security.Claims;
using static SchoolSystem.Models.User;

namespace SchoolSystem.Pages.ViewComponents
{
    public class UserHeaderInfoViewComponent : ViewComponent
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserHeaderInfoViewComponent> _logger;
        public UserHeaderInfoViewComponent(IUserService userService, ILogger<UserHeaderInfoViewComponent> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync(long id, string? type)
        {
            UserHeaderDto user = null;

            if (id == 0)
            {
                user = _userService.GetUserHeaderInfo();
            }
            else if (type == "Student")
            {
                user = _userService.GetStudentHeaderInfo(userId: null, studentId: id);
            }
            else if (type == "Teacher")
            {
                user = _userService.GetTeacherHeaderInfo(userId: null, teacherId: id);
            }

            if (user == null || string.IsNullOrEmpty(user.FirstName))
            {
                // Handle case where no user data is found
                _logger.LogWarning($"No valid user found for ID {id}.");
                user = new UserHeaderDto();  // Default value
            }

            return View("Default", user);
        }

    }
}
