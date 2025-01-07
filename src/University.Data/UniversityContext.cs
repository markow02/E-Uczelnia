using University.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

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

        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }

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
                new Student { StudentId = 2, Name = "Stanisław", LastName = "Nowakowicz", PESEL = "PESEL2", BirthDate = new DateTime(2019, 06, 25) },
                new Student { StudentId = 3, Name = "Eugenia", LastName = "Nowakowicz", PESEL = "PESEL3", BirthDate = new DateTime(2021, 06, 08) });

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
        }

        public void SaveData(string filePath)
        {
            var data = new
            {
                Students = Students.ToList(),
                Subjects = Subjects.ToList(),
                Classrooms = Classrooms.ToList()
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
                SaveChanges();
            }
        }

        private class UniversityData
        {
            public List<Student> Students { get; set; }
            public List<Subject> Subjects { get; set; }
            public List<Classroom> Classrooms { get; set; }
        }


    }
}
