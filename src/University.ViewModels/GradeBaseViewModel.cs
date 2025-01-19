using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;

namespace University.ViewModels
{
    public abstract class GradeBaseViewModel : ViewModelBase, IDataErrorInfo
    {
        #region Properties And Ctor

        protected readonly UniversityContext _context;
        protected readonly IDialogService _dialogService;
        protected Grade? _grade = new Grade();

        protected GradeBaseViewModel(UniversityContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;

            _context.Database.EnsureCreated();
            _context.Grades.Load();
            Grades = _context.Grades.Local.ToObservableCollection();
        }

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                return columnName switch
                {
                    "Value" when Value <= 0 => "Grade value must be greater than 0",
                    "StudentId" when StudentId <= 0 => "Student ID is required",
                    "SubjectId" when SubjectId <= 0 => "Subject ID is required",
                    _ => string.Empty,
                };
            }
        }

        private int _gradeId;
        public int GradeId
        {
            get => _gradeId;
            set
            {
                _gradeId = value;
                OnPropertyChanged(nameof(GradeId));
                _ = LoadGradeDataAsync();
            }
        }

        private double _value;
        public double Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        private int _studentId;
        public int StudentId
        {
            get => _studentId;
            set
            {
                _studentId = value;
                OnPropertyChanged(nameof(StudentId));
            }
        }

        private int _subjectId;
        public int SubjectId
        {
            get => _subjectId;
            set
            {
                _subjectId = value;
                OnPropertyChanged(nameof(SubjectId));
            }
        }

        private ObservableCollection<Grade>? _grades;
        public ObservableCollection<Grade>? Grades
        {
            get => _grades;
            set
            {
                _grades = value;
                OnPropertyChanged(nameof(Grades));
            }
        }

        private ICommand? _back;
        public ICommand Back => _back ??= new RelayCommand<object>(NavigateBack);

        private void NavigateBack(object? obj)
        {
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.GradesSubView = new GradeViewModel(_context, _dialogService);
            }
        }


        #endregion // Properties And Ctor


        #region Protected Methods

        protected bool IsValid()
        {
            var errors = new List<string>
            {
                this["Value"],
                this["StudentId"],
                this["SubjectId"]
            };

            return errors.All(string.IsNullOrEmpty);
        }

        protected async Task LoadGradeDataAsync()
        {
            if (_context is null)
            {
                return;
            }

            _grade = await _context.Grades.FindAsync(GradeId);
            if (_grade is null)
            {
                return;
            }

            Value = _grade.GradeValue;
            StudentId = (int)_grade.StudentId;
            SubjectId = (int)_grade.SubjectId;
        }

        #endregion // Protected Methods
    }
}
