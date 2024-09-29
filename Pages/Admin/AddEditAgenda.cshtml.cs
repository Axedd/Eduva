using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolSystem.Interfaces;
using SchoolSystem.Models;
using SchoolSystem.Services;

namespace SchoolSystem.Pages.Admin
{
    public class AddEditAgendaModel : PageModel
    {
        private readonly IStudentClassService _studentClassService;
        private readonly IIdValidationService _idValidationService;
		private readonly ISubjectService _subjectService;

        public AddEditAgendaModel(
            IStudentClassService studentClassService,
            IIdValidationService idValidationService,
			ISubjectService subjectService)
        {
            _studentClassService = studentClassService;
            _idValidationService = idValidationService;
			_subjectService = subjectService;
        }

        public List<StudentClassSubjects> StudentClassSubjects { get; set; }
        public string ClassName { get; set; }

        [BindProperty]
        public Agenda NewAgenda { get; set; }
		public StudentClassSubjects StudentClassSubject { get; set; }

        public async Task<IActionResult> OnGetAsync(int studentClassId, long? subjectId)
        {
            StudentClassSubjects = await _studentClassService.GetStudentClassSubjectsAsync(studentClassId);
			
			ClassName = await _studentClassService.GetStudentClassName(studentClassId) ?? "Invalid Class";

			

			if (subjectId != null)
			{
				long id = subjectId.Value; // Unwrap the nullable
				
				if (await _idValidationService.IsValidSubjectId(id))
				{
					StudentClassSubject = await _subjectService.GetStudentClassSubjectById(studentClassId, id);

					// Fetch the subject using the subject ID
					var studentClass = await _studentClassService.GetStudentClassByIdAsync(studentClassId);
					if (StudentClassSubject != null)
					{
						NewAgenda = new Agenda
						{
							SubjectId = id,
							Subject = StudentClassSubject.Subject, 
							StudentClassId = studentClassId,
							StudentClass = studentClass,
							Teacher = StudentClassSubject.Teacher,
						};
					}
					else
					{
						// Handle case where subject is not found (optional)
						// e.g., set an error message or log it
					}
					return Page();
				}
			}

			return Page();
        }
    }
}
