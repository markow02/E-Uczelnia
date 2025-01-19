using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using University.Interfaces;
using University.Models;

namespace University.ViewModels
{
    public class AddActivityClubViewModel : ActivityClubBaseViewModel
    {
        private readonly IActivityClubService _activityClubService;

        public AddActivityClubViewModel(IActivityClubService activityClubService, IDialogService dialogService)
            : base(activityClubService, dialogService)
        {
            _activityClubService = activityClubService;
            SaveCommand = new RelayCommand(async () => await SaveDataAsync());
        }

        public ICommand SaveCommand { get; }

        private async Task SaveDataAsync()
        {
            var activityClub = new ActivityClub
            {
                ActivityClubName = ActivityClubName,
                ActivityClubDescription = ActivityClubDescription,
                MeetingDay = MeetingDay
            };

            if (!await _activityClubService.IsValidAsync(activityClub))
            {
                Response = "Please complete all required fields";
                return;
            }

            await _activityClubService.SaveDataAsync(activityClub);
            Response = "Activity Club Data Saved";
        }
    }
}
