using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using University.Interfaces;

namespace University.ViewModels
{
    public class EditActivityClubViewModel : ActivityClubBaseViewModel
    {
        private readonly IActivityClubService _activityClubService;

        public EditActivityClubViewModel(IActivityClubService activityClubService, IDialogService dialogService)
            : base(activityClubService, dialogService)
        {
            _activityClubService = activityClubService;
            SaveCommand = new RelayCommand(async () => await SaveDataAsync());
        }

        public ICommand SaveCommand { get; }

        private async Task SaveDataAsync()
        {
            var activityClub = await _activityClubService.GetActivityClubByIdAsync(ActivityClubId);
            if (activityClub == null)
            {
                Response = "Error: Activity Club not found";
                return;
            }

            activityClub.ActivityClubName = ActivityClubName;
            activityClub.ActivityClubDescription = ActivityClubDescription;
            activityClub.MeetingDay = MeetingDay;

            if (!await _activityClubService.IsValidAsync(activityClub))
            {
                Response = "Please complete all required fields";
                return;
            }

            await _activityClubService.SaveDataAsync(activityClub);
            Response = "Activity Club Data Updated";
        }
    }
}
