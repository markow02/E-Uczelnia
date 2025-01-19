using System.Threading.Tasks;
using University.Data;
using University.Interfaces;
using University.Models;
using System.Linq;
using System.Collections.Generic;

namespace University.Services
{
    public class ActivityClubService : IActivityClubService
    {
        private readonly UniversityContext _context;

        public ActivityClubService(UniversityContext context)
        {
            _context = context;
        }

        public async Task<List<ActivityClub>> LoadDataAsync()
        {
            return await Task.FromResult(_context.ActivityClubs.ToList());
        }

        public async Task<bool> IsValidAsync(ActivityClub activityClub)
        {
            // Add validation logic here
            if (string.IsNullOrEmpty(activityClub.ActivityClubName))
            {
                return false;
            }

            if (string.IsNullOrEmpty(activityClub.MeetingDay))
            {
                return false;
            }

            if (string.IsNullOrEmpty(activityClub.ActivityClubDescription))
            {
                return false;
            }

            return true;
        }

        public async Task SaveDataAsync(ActivityClub activityClub)
        {
            if (activityClub.ActivityClubId == 0)
            {
                _context.ActivityClubs.Add(activityClub);
            }
            else
            {
                _context.ActivityClubs.Update(activityClub);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<ActivityClub?> GetActivityClubByIdAsync(int activityClubId)
        {
            return await _context.ActivityClubs.FindAsync(activityClubId);
        }
    }
}
