using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using University.Interfaces;
using University.Models;

namespace University.ViewModels
{
    public class ActivityClubViewModel : ViewModelBase
    {
        private readonly IActivityClubService _activityClubService;
        private readonly IDialogService _dialogService;

        private bool? _dialogResult = null;
        public bool? DialogResult
        {
            get => _dialogResult;
            set
            {
                _dialogResult = value;
                OnPropertyChanged(nameof(DialogResult));
            }
        }

        private ObservableCollection<ActivityClub>? _activityClubs;
        public ObservableCollection<ActivityClub>? ActivityClubs
        {
            get => _activityClubs;
            private set
            {
                _activityClubs = value;
                OnPropertyChanged(nameof(ActivityClubs));
            }
        }

        private ICommand? _add;
        public ICommand Add => _add ??= new RelayCommand(AddNewActivityClub);

        private void AddNewActivityClub()
        {
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.ActivityClubSubView = new AddActivityClubViewModel(_activityClubService, _dialogService);
            }
        }

        private ICommand? _edit;
        public ICommand Edit => _edit ??= new RelayCommand<object>(EditActivityClub);

        private void EditActivityClub(object? obj)
        {
            if (obj is int activityClubId)
            {
                var editActivityClubViewModel = new EditActivityClubViewModel(_activityClubService, _dialogService)
                {
                    ActivityClubId = activityClubId
                };

                var instance = MainWindowViewModel.Instance();
                if (instance is not null)
                {
                    instance.ActivityClubSubView = editActivityClubViewModel;
                }
            }
        }

        private ICommand? _remove;
        public ICommand Remove => _remove ??= new RelayCommand<object>(RemoveActivityClub);

        private async void RemoveActivityClub(object? obj)
        {
            if (obj is int activityClubId)
            {
                var activityClub = ActivityClubs?.FirstOrDefault(c => c.ActivityClubId == activityClubId);
                if (activityClub is not null)
                {
                    DialogResult = _dialogService.Show(
                        $"Are you sure you want to remove the activity club '{activityClub.ActivityClubName}'?"
                    );

                    if (DialogResult == true)
                    {
                        ActivityClubs?.Remove(activityClub);
                        await _activityClubService.SaveDataAsync(activityClub);
                    }
                }
            }
        }

        public ActivityClubViewModel(IActivityClubService activityClubService, IDialogService dialogService)
        {
            _activityClubService = activityClubService;
            _dialogService = dialogService;

            LoadActivityClubs();
        }

        private async void LoadActivityClubs()
        {
            ActivityClubs = new ObservableCollection<ActivityClub>(await _activityClubService.LoadDataAsync());
        }
    }
}
