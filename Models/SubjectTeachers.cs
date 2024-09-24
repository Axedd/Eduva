using SchoolSystem.Models;

namespace SchoolSystem.Models
{
	public class SubjectTeachers
	{
		public long SubjectId { get; set; }
		public long TeacherId { get; set; }
		

		public virtual Subject Subject { get; set; } 
		public virtual Teacher Teacher { get; set; } 
	}
}
