using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;


namespace University.ViewModels
{
    public class AddEnrollmentViewModel : EnrollmentBaseViewModel, IDataErrorInfo
    {
        private new readonly UniversityContext _context;

        public new string Error => string.Empty;

        public new string this[string columnName]
        {
            get
            {
                if (columnName == "CandidateName")
                {
                    if (string.IsNullOrEmpty(CandidateName))
                    {
                        return "Candidate Name is Required";
                    }
                }
                if (columnName == "CandidateSurname")
                {
                    if (string.IsNullOrEmpty(CandidateSurname))
                    {
                        return "Candidate Surname is Required";
                    }
                }
                if (columnName == "CandidateSchool")
                {
                    if (string.IsNullOrEmpty(CandidateSchool))
                    {
                        return "Candidate School is Required";
                    }
                }
                return string.Empty;
            }
        }

        private string _candidateName = string.Empty;
        public new string CandidateName
        {
            get => _candidateName;
            set
            {
                _candidateName = value;
                OnPropertyChanged(nameof(CandidateName));
            }
        }

        private string _candidateSurname = string.Empty;
        public new string CandidateSurname
        {
            get => _candidateSurname;
            set
            {
                _candidateSurname = value;
                OnPropertyChanged(nameof(CandidateSurname));
            }
        }
        private string _candidateSchool = string.Empty;
        public new string CandidateSchool
        {
            get => _candidateSchool;
            set
            {
                _candidateSurname = value;
                OnPropertyChanged(nameof(CandidateSurname));
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
        public new ICommand Back
        {
            get
            {
                if (_back is null)
                {
                    _back = new RelayCommand<object>(NavigateBack);
                }
                return _back;
            }
        }

        private void NavigateBack(object? obj)
        {
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.EnrollmentsSubView = new EnrollmentViewModel(_context, _dialogService);
            }
        }

        private ICommand? _save;
        public ICommand Save
        {
            get
            {
                if (_save is null)
                {
                    _save = new RelayCommand<object>(SaveData);
                }
                return _save;
            }
        }

        private async void SaveData(object? obj)
        {
            if (!IsValid())
            {
                Response = "Please complete all required fields";
                return;
            }

            var enrollment = new Enrollment
            {
                CandidateName = CandidateName,
                CandidateSurname = CandidateSurname,
                CandidateSchool = CandidateSchool
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            Response = "Enrollment Data Saved";
        }

        public AddEnrollmentViewModel(UniversityContext context, IDialogService dialogService)
            : base(context, dialogService)
        {
            _context = context;
        }

        private new bool IsValid()
        {
            string[] properties = { "CandidateName", "CandidateSurname", "CandidateSchool" };
            foreach (string property in properties)
            {
                if (!string.IsNullOrEmpty(this[property]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
