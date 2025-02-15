﻿namespace University.Models
{
    public class Student
    {
        public long StudentId { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PESEL { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; } = null;
        public virtual ICollection<Subject>? Subjects { get; set; } = null;
        public virtual ICollection<Grade>? Grades { get; set; }
        public virtual ICollection<Exam>? Exams { get; set; }
    }
}
