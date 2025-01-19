using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows.Input;
using University.Interfaces;
using University.Models;


namespace University.ViewModels
{
    public abstract class ActivityClubBaseViewModel : ViewModelBase, IDataErrorInfo
    {
        #region Properties And Ctor

        protected readonly IActivityClubService _activityClubService;
        protected readonly IDialogService _dialogService;
        protected ActivityClub? _activityClub = new ActivityClub();

        protected ActivityClubBaseViewModel(
            IActivityClubService activityClubService,
            IDialogService dialogService)
        {
            _activityClubService = activityClubService;
            _dialogService = dialogService;
        }

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                return columnName switch
                {
                    "ActivityClubName" when string.IsNullOrEmpty(ActivityClubName) => "Activity club name is required",
                    _ => string.Empty,
                };
            }
        }

        private int _activityClubId;
        public int ActivityClubId
        {
            get => _activityClubId;
            set
            {
                _activityClubId = value;
                OnPropertyChanged(nameof(ActivityClubId));
                _ = LoadActivityClubDataAsync();
            }
        }

        private string _activityClubName = string.Empty;
        public string ActivityClubName
        {
            get => _activityClubName;
            set
            {
                _activityClubName = value;
                OnPropertyChanged(nameof(ActivityClubName));
            }
        }

        private string _activityClubDescription = string.Empty;
        public string ActivityClubDescription
        {
            get => _activityClubDescription;
            set
            {
                _activityClubDescription = value;
                OnPropertyChanged(nameof(ActivityClubDescription));
            }
        }

        private string _meetingDay = string.Empty;
        public string MeetingDay
        {
            get => _meetingDay;
            set
            {
                _meetingDay = value;
                OnPropertyChanged(nameof(MeetingDay));
            }
        }

        private string _response = string.Empty;
        public string Response
        {
            get => _response;
            set
            {
                _response = value;
                OnPropertyChanged(nameof(Response));
            }
        }

        private ICommand? _back;
        public ICommand Back => _back ??= new RelayCommand<object>(NavigateBack);

        private void NavigateBack(object? obj)
        {
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.ActivityClubSubView = new ActivityClubViewModel(_activityClubService, _dialogService);
            }
        }

        #endregion // Properties And Ctor

        #region Protected Methods

        protected async Task LoadActivityClubDataAsync()
        {
            if (_activityClubService is null)
            {
                return;
            }

            _activityClub = await _activityClubService.GetActivityClubByIdAsync(ActivityClubId);
            if (_activityClub is null)
            {
                return;
            }

            ActivityClubName = _activityClub.ActivityClubName;
            ActivityClubDescription = _activityClub.ActivityClubDescription;
            MeetingDay = _activityClub.MeetingDay;
        }

        #endregion // Protected Methods
    }
}
