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
        private readonly IAgendaService _agendaService;

        public AddEditAgendaModel(
            IStudentClassService studentClassService,
            IIdValidationService idValidationService,
			ISubjectService subjectService,
            IAgendaService agendaService)
        {
            _studentClassService = studentClassService;
            _idValidationService = idValidationService;
			_subjectService = subjectService;
            _agendaService = agendaService;
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
                    await LoadNewAgenda(studentClassId, id);

                    return Page();
				}
			}

			return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                StudentClassSubjects = await _studentClassService.GetStudentClassSubjectsAsync(NewAgenda.StudentClassId);
                ClassName = await _studentClassService.GetStudentClassName(NewAgenda.StudentClassId) ?? "Invalid Class";
                await LoadNewAgenda(NewAgenda.StudentClassId, NewAgenda.SubjectId);
                return Page();
            }

            // Fetch StudentClassSubject and studentClass based on NewAgenda properties
            StudentClassSubject = await _subjectService.GetStudentClassSubjectById(NewAgenda.StudentClassId, NewAgenda.SubjectId);
            var studentClass = await _studentClassService.GetStudentClassByIdAsync(NewAgenda.StudentClassId);

            if (StudentClassSubject != null)
            {
                // Generate a new Agenda ID
                NewAgenda.AgendaId = await _agendaService.GenerateAgendaIdAsync();

                // Assign values from the StudentClassSubject and studentClass
                NewAgenda.SubjectId = StudentClassSubject.SubjectId;
                NewAgenda.StudentClassId = StudentClassSubject.StudentClassId;
                NewAgenda.StudentClass = studentClass; // Ensure this is set correctly

                NewAgenda.TeacherId = StudentClassSubject.Teacher.TeacherId;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Subject not found.");
                return Page();
            }


            // Add to the agenda
            await _agendaService.AddAgendaAsync(NewAgenda);

            return RedirectToPage("/Admin/Index");
        }


        public async Task LoadNewAgenda(int studentClassId, long subjectId)
        {
            StudentClassSubject = await _subjectService.GetStudentClassSubjectById(studentClassId, subjectId);

            // Fetch the subject using the subject ID
            var studentClass = await _studentClassService.GetStudentClassByIdAsync(studentClassId);
            if (StudentClassSubject != null)
            {
                NewAgenda = new Agenda
                {
                    SubjectId = subjectId,
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
        }


    }
}
