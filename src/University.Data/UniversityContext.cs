using University.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace University.Data
{
    public class UniversityContext : DbContext
    {
        public UniversityContext()
        {
        }

        public UniversityContext(DbContextOptions<UniversityContext> options) : base(options)
        {
        }

        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Subject> Subjects { get; set; }
        public virtual DbSet<Classroom> Classrooms { get; set; }
        public virtual DbSet<Enrollment> Enrollments { get; set; }
        public virtual DbSet<Grade> Grades { get; set; }
        public virtual DbSet<ActivityClub> ActivityClubs { get; set; }
        public virtual DbSet<Exam> Exams { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseInMemoryDatabase("UniversityDb");
                optionsBuilder.UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subject>().Ignore(s => s.IsSelected);


            modelBuilder.Entity<Student>().HasData(
                new Student { StudentId = 1, Name = "Wieńczysław", LastName = "Nowakowicz", PESEL = "PESEL1", BirthDate = new DateTime(1987, 05, 22) },
                new Student { StudentId = 2, Name = "Stanisław", LastName = "Gagatek", PESEL = "PESEL2", BirthDate = new DateTime(2019, 06, 25) },
                new Student { StudentId = 3, Name = "Eugenia", LastName = "Frankowska", PESEL = "PESEL3", BirthDate = new DateTime(2021, 06, 08) });

            modelBuilder.Entity<Subject>().HasData(
                new Subject { SubjectId = 1, Name = "Matematyka", Semester = "1", Lecturer = "Michalina Warszawa" },
                new Subject { SubjectId = 2, Name = "Biologia", Semester = "2", Lecturer = "Halina Katowice" },
                new Subject { SubjectId = 3, Name = "Chemia", Semester = "3", Lecturer = "Jan Nowak" }
            );

            modelBuilder.Entity<Classroom>().HasData(
                new Classroom { ClassroomId = 1, ClassroomNumber = "A101", Capacity = 30, HasProjector = true, IsLab = false },
                new Classroom { ClassroomId = 2, ClassroomNumber = "B202", Capacity = 50, HasProjector = false, IsLab = true },
                new Classroom { ClassroomId = 3, ClassroomNumber = "C303", Capacity = 45, HasProjector = true, IsLab = true }
            );

            modelBuilder.Entity<Enrollment>().HasData(
                new Enrollment { EnrollmentId = 1, CandidateName = "Jan", CandidateSurname = "Nowak", CandidateSchool = "ZST" },
                new Enrollment { EnrollmentId = 2, CandidateName = "Anna", CandidateSurname = "Kowalska", CandidateSchool = "sikorak" },
                new Enrollment { EnrollmentId = 3, CandidateName = "Michał", CandidateSurname = "Kowalczyk", CandidateSchool = "smolen" }
            );

            modelBuilder.Entity<Grade>().HasData(
                new Grade { GradeId = 1, GradeValue = 5, StudentId = 1, SubjectId = 1, Date = new DateTime(2024, 01, 15) },
                new Grade { GradeId = 2, GradeValue = 4, StudentId = 2, SubjectId = 2, Date = new DateTime(2024, 02, 20) },
                new Grade { GradeId = 3, GradeValue = 3, StudentId = 3, SubjectId = 3, Date = new DateTime(2024, 03, 25) },
                new Grade { GradeId = 4, GradeValue = 2, StudentId = 1, SubjectId = 2, Date = new DateTime(2024, 04, 30) },
                new Grade { GradeId = 5, GradeValue = 1, StudentId = 2, SubjectId = 3, Date = new DateTime(2024, 05, 05) }
            );

            modelBuilder.Entity<ActivityClub>().HasData(
            new ActivityClub { ActivityClubId = 1, ActivityClubName = "Math Club", MeetingDay = "Monday", ActivityClubDescription = "Explore the beauty of numbers and logic." },
            new ActivityClub { ActivityClubId = 2, ActivityClubName = "Biology Club", MeetingDay = "Tuesday", ActivityClubDescription = "Dive into the wonders of life and nature." },
            new ActivityClub { ActivityClubId = 3, ActivityClubName = "Chemistry Club", MeetingDay = "Friday", ActivityClubDescription = "Experiment with reactions and unlock new discoveries." }
            );

            modelBuilder.Entity<Exam>().HasData(
                new Exam { ExamId = 1, ClassroomId = 1, SubjectId = 1, ExamType = "Oral", ExamDate1 = new DateTime(2024, 01, 15), ExamDate2 = new DateTime(2024, 01, 16), },
                new Exam { ExamId = 2, ClassroomId = 2, SubjectId = 2, ExamType = "Written", ExamDate1 = new DateTime(2024, 02, 20), ExamDate2 = new DateTime(2024, 02, 21) },
                new Exam { ExamId = 3, ClassroomId = 3, SubjectId = 3, ExamType = "Written", ExamDate1 = new DateTime(2024, 03, 25), ExamDate2 = new DateTime(2024, 03, 26) }
            );
            // Relacje dla Grade
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Student)               // Relacja 1 do wielu: Grade -> Student
                .WithMany(s => s.Grades)              // Student może mieć wiele ocen
                .HasForeignKey(g => g.StudentId)      // Klucz obcy
                .OnDelete(DeleteBehavior.Cascade);    // Usuwanie kaskadowe

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Subject)               // Relacja 1 do wielu: Grade -> Subject
                .WithMany(s => s.Grades)              // Subject może mieć wiele ocen
                .HasForeignKey(g => g.SubjectId)      // Klucz obcy
                .OnDelete(DeleteBehavior.Cascade);    // Usuwanie kaskadowe

            //Relacje dla Exam
            modelBuilder.Entity<Exam>()
                .HasOne(e => e.Subject)
                .WithMany(s => s.Exams)
                .HasForeignKey(e => e.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Exam>()
               .HasOne(e => e.Classroom)
               .WithMany(s => s.Exams)
               .HasForeignKey(e => e.SubjectId)
               .OnDelete(DeleteBehavior.Cascade);

        }

        public void SaveData(string filePath)
        {
            var data = new
            {
                Students = Students.ToList(),
                Subjects = Subjects.ToList(),
                Classrooms = Classrooms.ToList(),
                Enrollments = Enrollments.ToList(),
                Grades = Grades.ToList(),
                ActivityClubs = ActivityClubs.ToList()
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve
            };

            var jsonData = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filePath, jsonData);
        }

        public void LoadData(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            var jsonData = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<UniversityData>(jsonData, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            });

            if (data != null)
            {
                Students.AddRange(data.Students);
                Subjects.AddRange(data.Subjects);
                Classrooms.AddRange(data.Classrooms);
                Enrollments.AddRange(data.Enrollments);
                Grades.AddRange(data.Grades);
                ActivityClubs.AddRange(data.ActivityClubs);

                SaveChanges();
            }
        }

        public class UniversityData
        {
            public List<Student> Students { get; set; } = new List<Student>();
            public List<Subject> Subjects { get; set; } = new List<Subject>();
            public List<Classroom> Classrooms { get; set; } = new List<Classroom>();
            public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
            public List<Grade> Grades { get; set; } = new List<Grade>();
            public List<ActivityClub> ActivityClubs { get; set; } = new List<ActivityClub>();
        }
    }
}
