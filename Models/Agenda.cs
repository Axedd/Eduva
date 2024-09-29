namespace SchoolSystem.Models
{
    public class Agenda
    {
        public long AgendaId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string? Info { get; set; }
        public string? Note {  get; set; }
        public string? HomeWork { get; set; }

        public long SubjectId { get; set; }
        public Subject Subject { get; set; }
        public int StudentClassId { get; set; }
        public StudentClass StudentClass { get; set; }
		public long TeacherId { get; set; }
		public Teacher Teacher { get; set; }
	}
}
