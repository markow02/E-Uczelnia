using University.Models;

namespace University.Interfaces
{
    public interface IClassroomService
    {
        Task<List<Classroom>> LoadDataAsync();
        Task<bool> IsValidAsync(Classroom classroom);
        Task SaveDataAsync(Classroom classroom);
        Task<Classroom?> GetClassroomByIdAsync(long classroomId);
    }
}
