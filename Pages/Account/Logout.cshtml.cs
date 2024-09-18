using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthWebApp.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnPostAsync()
        {
            // Sign out of the default authentication scheme
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            // Optionally, sign out of the external authentication scheme if used
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Redirect to home page or login page after logout
            return RedirectToPage("/Index");
        }
    }
}
