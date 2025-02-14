using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;

namespace University.ViewModels
{
    public abstract class EnrollmentBaseViewModel : ViewModelBase, IDataErrorInfo
    {
        #region Properties And Ctor

        protected readonly UniversityContext _context;
        protected readonly IDialogService _dialogService;
        protected Enrollment? _enrollment = new Enrollment();

        protected EnrollmentBaseViewModel(UniversityContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;

            _context.Database.EnsureCreated();
            _context.Enrollments.Load();
            Enrollments = _context.Enrollments.Local.ToObservableCollection();
        }

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                return columnName switch
                {
                    "CandidateName" when string.IsNullOrEmpty(CandidateName) => "Candidate name is required",
                    "CandidateSurname" when string.IsNullOrEmpty(CandidateSurname) => "Candidate surname is required",
                    "CandidateSchool" when string.IsNullOrEmpty(CandidateSchool) => "Candidate school is required",
                    _ => string.Empty,
                };
            }
        }

        private long _enrollmentId;
        public long EnrollmentId
        {
            get => _enrollmentId;
            set
            {
                _enrollmentId = value;
                OnPropertyChanged(nameof(EnrollmentId));
                _ = LoadEnrollmentDataAsync();
            }
        }

        private string _candidateName = string.Empty;
        public string CandidateName
        {
            get => _candidateName;
            set
            {
                _candidateName = value;
                OnPropertyChanged(nameof(CandidateName));
            }
        }

        private string _candidateSurname = string.Empty;
        public string CandidateSurname
        {
            get => _candidateSurname;
            set
            {
                _candidateSurname = value;
                OnPropertyChanged(nameof(CandidateSurname));
            }
        }

        private string _candidateSchool = string.Empty;
        public string CandidateSchool
        {
            get => _candidateSchool;
            set
            {
                _candidateSchool = value;
                OnPropertyChanged(nameof(CandidateSchool));
            }
        }

        private ObservableCollection<Enrollment>? _enrollments;
        public ObservableCollection<Enrollment>? Enrollments
        {
            get => _enrollments;
            set
            {
                _enrollments = value;
                OnPropertyChanged(nameof(Enrollments));
            }
        }

        private ICommand? _back;
        public ICommand Back => _back ??= new RelayCommand<object>(NavigateBack);

        private void NavigateBack(object? obj)
        {
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.EnrollmentsSubView = new EnrollmentViewModel(_context, _dialogService);
            }
        }

        #endregion // Properties And Ctor

        #region Protected Methods

        protected bool IsValid()
        {
            var errors = new List<string>
                {
                    this["CandidateName"],
                    this["CandidateSurname"],
                    this["CandidateSchool"]
                };

            return errors.All(string.IsNullOrEmpty);
        }

        protected async Task LoadEnrollmentDataAsync()
        {
            if (_context is null)
            {
                return;
            }

            _enrollment = await _context.Enrollments.FindAsync(EnrollmentId);
            if (_enrollment is null)
            {
                return;
            }

            CandidateName = _enrollment.CandidateName;
            CandidateSurname = _enrollment.CandidateSurname;
            CandidateSchool = _enrollment.CandidateSchool;
        }

        #endregion // Protected Methods
    }
}
