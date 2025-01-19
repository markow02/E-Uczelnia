using University.Models;

namespace University.Interfaces
{
    public interface IActivityClubService
    {
        Task<List<ActivityClub>> LoadDataAsync();
        Task<bool> IsValidAsync(ActivityClub activityClub);
        Task SaveDataAsync(ActivityClub activityClub);
        Task<ActivityClub?> GetActivityClubByIdAsync(int activityClubId);
    }
}
