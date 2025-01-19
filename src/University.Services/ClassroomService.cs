using System.Threading.Tasks;
using University.Data;
using University.Interfaces;
using University.Models;
using System.Linq;
using System.Collections.Generic;

namespace University.Services
{
    public class ClassroomService : IClassroomService
    {
        private readonly UniversityContext _context;

        public ClassroomService(UniversityContext context)
        {
            _context = context;
        }

        public async Task<List<Classroom>> LoadDataAsync()
        {
            return await Task.FromResult(_context.Classrooms.ToList());
        }

        public async Task<bool> IsValidAsync(Classroom classroom)
        {
            if (string.IsNullOrWhiteSpace(classroom.ClassroomNumber) || classroom.Capacity <= 0)
            {
                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }

        public async Task SaveDataAsync(Classroom classroom)
        {
            if (classroom.ClassroomId == 0)
            {
                _context.Classrooms.Add(classroom);
            }
            else
            {
                var existing = _context.Classrooms.FirstOrDefault(c => c.ClassroomId == classroom.ClassroomId);
                if (existing != null)
                {
                    existing.ClassroomNumber = classroom.ClassroomNumber;
                    existing.Capacity = classroom.Capacity;
                    existing.Floor = classroom.Floor;
                    existing.HasProjector = classroom.HasProjector;
                    existing.IsLab = classroom.IsLab;
                }
            }

            await Task.Run(() => _context.SaveChanges());
        }

        public async Task<Classroom?> GetClassroomByIdAsync(long classroomId)
        {
            return await Task.FromResult(
                _context.Classrooms.FirstOrDefault(c => c.ClassroomId == classroomId)
            );
        }
    }
}
