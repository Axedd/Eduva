using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SchoolSystem.Pages.Shared
{
    public class BaseService : PageModel
    {
        protected readonly ILogger<BaseService> _logger;

        public BaseService(ILogger<BaseService> logger)
        {
            _logger = logger;
        }

        protected void LogError(Exception ex, string message)
        {
            _logger.LogError(ex, message);
        }

        protected void HandleError(Exception ex, string additionalInfo)
        {
            LogError(ex, additionalInfo);
            throw new Exception(additionalInfo, ex);
        }
    }
}
