using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using University.Data;
using University.Interfaces;

namespace University.ViewModels
{
    public class EnrollmentViewModel : EnrollmentBaseViewModel
    {
        private new readonly IDialogService _dialogService;

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

        private ICommand? _add;
        public ICommand Add => _add ??= new RelayCommand(AddNewEnrollment);

        private void AddNewEnrollment()
        {
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.EnrollmentsSubView = new AddEnrollmentViewModel(_context, _dialogService);
            }
        }

        private ICommand? _edit;
        public ICommand Edit => _edit ??= new RelayCommand<object>(EditEnrollment);

        private void EditEnrollment(object? obj)
        {
            if (obj is long enrollmentId)
            {
                var editEnrollmentViewModel = new EditEnrollmentViewModel(_context, _dialogService)
                {
                    EnrollmentId = enrollmentId
                };

                var instance = MainWindowViewModel.Instance();
                if (instance is not null)
                {
                    instance.EnrollmentsSubView = editEnrollmentViewModel;
                }
            }
        }

        private ICommand? _remove;
        public ICommand Remove => _remove ??= new RelayCommand<object>(RemoveEnrollment);

        private async void RemoveEnrollment(object? obj)
        {
            if (obj is long enrollmentId)
            {
                var enrollment = Enrollments?.FirstOrDefault(e => e.EnrollmentId == enrollmentId);
                if (enrollment is not null)
                {
                    DialogResult = _dialogService.Show(
                        $"Are you sure you want to remove the enrollment for '{enrollment.CandidateName} {enrollment.CandidateSurname} {enrollment.CandidateSchool}'?"
                    );

                    if (DialogResult == true)
                    {
                        Enrollments?.Remove(enrollment);
                        _context.Enrollments.Remove(enrollment);
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }

        public EnrollmentViewModel(UniversityContext context, IDialogService dialogService)
            : base(context, dialogService)
        {
            _dialogService = dialogService;
        }
    }
}
