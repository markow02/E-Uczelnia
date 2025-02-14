using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;
using System.Collections.ObjectModel;

namespace University.ViewModels
{
    public class AddGradeViewModel : GradeBaseViewModel, IDataErrorInfo
    {
        private readonly new UniversityContext _context;

        public new string Error => string.Empty;

        public new string this[string columnName]
        {
            get
            {
                if (columnName == "GradeValue")
                {
                    if (GradeValue <= 0)
                    {
                        return "Grade value must be greater than 0";
                    }
                }
                if (columnName == "SubjectName")
                {
                    if (string.IsNullOrEmpty(SubjectName))
                    {
                        return "Subject Name is required";
                    }
                }
                return string.Empty;
            }
        }

        private int _gradeValue;
        public int GradeValue
        {
            get => _gradeValue;
            set
            {
                _gradeValue = value;
                OnPropertyChanged(nameof(GradeValue));
            }
        }

        public List<int> GradeValues { get; } = new List<int> { 1, 2, 3, 4, 5, 6 };

        private string _selectedStudentLastName = string.Empty;
        public string SelectedStudentLastName
        {
            get => _selectedStudentLastName;
            set
            {
                _selectedStudentLastName = value;
                OnPropertyChanged(nameof(SelectedStudentLastName));
            }
        }

        private ObservableCollection<string> _studentLastNames = new();
        public ObservableCollection<string> StudentLastNames
        {
            get => _studentLastNames;
            set
            {
                _studentLastNames = value;
                OnPropertyChanged(nameof(StudentLastNames));
            }
        }

        private string _selectedSubjectName = string.Empty;
        public string SelectedSubjectName
        {
            get => _selectedSubjectName;
            set
            {
                _selectedSubjectName = value;
                OnPropertyChanged(nameof(SelectedSubjectName));
            }
        }

        private ObservableCollection<string> _subjectNames = new();
        public ObservableCollection<string> SubjectNames
        {
            get => _subjectNames;
            set
            {
                _subjectNames = value;
                OnPropertyChanged(nameof(SubjectNames));
            }
        }

        private string _subjectName = string.Empty;
        public string SubjectName
        {
            get => _subjectName;
            set
            {
                _subjectName = value;
                OnPropertyChanged(nameof(SubjectName));
            }
        }

        private DateTime _date = DateTime.Today;
        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
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
                instance.GradesSubView = new GradeViewModel(_context, _dialogService);
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

            var student = _context.Students.FirstOrDefault(s => s.LastName == SelectedStudentLastName);
            if (student == null)
            {
                Response = "Selected student not found";
                return;
            }

            var subject = _context.Subjects.FirstOrDefault(s => s.Name == SelectedSubjectName);
            if (subject == null)
            {
                Response = "Selected subject not found";
                return;
            }

            var grade = new Grade
            {
                GradeValue = GradeValue,
                StudentId = student.StudentId,
                SubjectId = subject.SubjectId,
                Date = Date
            };

            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();

            Response = "Grade Data Saved";
        }

        public AddGradeViewModel(UniversityContext context, IDialogService dialogService)
            : base(context, dialogService)
        {
            _context = context;
            LoadStudentLastNames();
            LoadSubjectNames();
        }

        private void LoadStudentLastNames()
        {
            StudentLastNames = new ObservableCollection<string>(_context.Students.Select(s => s.LastName).Distinct().ToList());
        }

        private void LoadSubjectNames()
        {
            SubjectNames = new ObservableCollection<string>(_context.Subjects.Select(s => s.Name).Distinct().ToList());
        }

        private new bool IsValid()
        {
            string[] properties = { "GradeValue", "SelectedStudentLastName", "SelectedSubjectName" };
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
