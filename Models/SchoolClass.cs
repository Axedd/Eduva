namespace SchoolSystem.Models
{
	public class SchoolClass
	{
		public int ClassId { get; set; } // This is the primary key

		public string ClassName { get; set; }

		// Navigation property
		public List<Student> Students { get; set; }
	}
}
