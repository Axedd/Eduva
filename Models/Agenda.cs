using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolSystem.Models
{
    public class Agenda
    {
        public long AgendaId { get; set; }
        [Required(ErrorMessage = "Start Date and Time is required.")]
        [DataType(DataType.DateTime)]
        public DateTime StartDateTime { get; set; }
        [Required(ErrorMessage = "End Date and Time is required.")]
        [DataType(DataType.DateTime)]
        public DateTime EndDateTime { get; set; }
        public string? Info { get; set; }
        public string? Note {  get; set; }
        public string? HomeWork { get; set; }

        public long SubjectId { get; set; }
        public Subject? Subject { get; set; }
        [NotMapped]
        public SubjectAgendaDto? SubjectDto { get; set; }
        public int StudentClassId { get; set; }
        public StudentClass? StudentClass { get; set; }
        [NotMapped]
        public StudentClassDto? StudentClassDto { get; set; }
		public long TeacherId { get; set; }
		public Teacher? Teacher { get; set; }

    }
}
