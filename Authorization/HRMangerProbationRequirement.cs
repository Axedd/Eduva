using Microsoft.AspNetCore.Authorization;

namespace SchoolSystem.Authorization
{
	public class HRMangerProbationRequirement : IAuthorizationRequirement
	{

		public HRMangerProbationRequirement(int probationMonths)
		{
			ProbationMonths = probationMonths;
		}

		public int ProbationMonths { get; }
	}

	public class HRMangerProbationRequirementHandler : AuthorizationHandler<HRMangerProbationRequirement>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HRMangerProbationRequirement requirement)
		{
			if (!context.User.HasClaim(x => x.Type == "EmploymentDate"))
			{
				return Task.CompletedTask;
			}

			if (DateTime.TryParse(context.User.FindFirst(
				x => x.Type == "EmploymentDate")?.Value, out DateTime employmentDate))
			{
				var period = DateTime.Now - employmentDate;
				if (period.Days > 30 * requirement.ProbationMonths)
				{
					context.Succeed(requirement);
				}
			}

			return Task.CompletedTask;
		}
	}
}
