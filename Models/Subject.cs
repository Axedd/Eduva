﻿using SchoolSystem.Models;

namespace AuthWebApp.Models
{
    public class Subject
    {
        public long SubjectId { get; set; }
        public string SubjectName { get; set; }

        public ICollection<SubjectTeachers> SubjectTeachers { get; set; } = new List<SubjectTeachers>();
        public ICollection<StudentClassSubjects> StudentClassSubjects { get; set; } = new List<StudentClassSubjects>();
    }
}
