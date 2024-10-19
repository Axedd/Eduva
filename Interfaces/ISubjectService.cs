using SchoolSystem.Models;

namespace SchoolSystem.Interfaces
{
	public interface ISubjectService
	{
		Task<int> GenerateSubjectIdAsync();
		Task AssignSubjectToClassAsync(long subjectId, long teacherId, int studentClassId);
		Task<Subject> GetSubjectByIdAsync(long subjectId);
		Task<StudentClassSubjects> GetStudentClassSubjectById(int studentClassId, long subjectId);
		Task<List<SubjectTeacherDto>> GetSubjectTeachersAsync(long subjectId);
		Task<List<Subject>> GetSubjectWithTeachersAsync();

    }
}
