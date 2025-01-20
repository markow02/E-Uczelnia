using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;
using System.Collections.ObjectModel;

namespace University.ViewModels
{
    public class EditGradeViewModel : GradeBaseViewModel, IDataErrorInfo
    {
        private new readonly UniversityContext _context;
        private new readonly Grade? _grade;


        public new string Error => string.Empty;

        public new string this[string columnName]
        {
            get
            {
                if (columnName == nameof(GradeValue) && GradeValue <= 0)
                {
                    return "Grade value must be greater than 0";
                }
                if (columnName == nameof(SelectedSubjectName) && string.IsNullOrEmpty(SelectedSubjectName))
                {
                    return "Subject Name is required";
                }
                if (columnName == nameof(SelectedStudentLastName) && string.IsNullOrEmpty(SelectedStudentLastName))
                {
                    return "Student Last Name is required";
                }
                return string.Empty;
            }
        }

        private double _gradeValue;
        public double GradeValue
        {
            get => _gradeValue;
            set
            {
                if (_gradeValue != value)
                {
                    _gradeValue = value;
                    OnPropertyChanged(nameof(GradeValue));
                }
            }
        }

        private string _selectedStudentLastName;
        public string SelectedStudentLastName
        {
            get => _selectedStudentLastName;
            set
            {
                if (_selectedStudentLastName != value)
                {
                    _selectedStudentLastName = value;
                    OnPropertyChanged(nameof(SelectedStudentLastName));
                }
            }
        }

        public ObservableCollection<string> StudentLastNames { get; } = new();

        private string _selectedSubjectName;
        public string SelectedSubjectName
        {
            get => _selectedSubjectName;
            set
            {
                if (_selectedSubjectName != value)
                {
                    _selectedSubjectName = value;
                    OnPropertyChanged(nameof(SelectedSubjectName));
                }
            }
        }

        public ObservableCollection<string> SubjectNames { get; } = new();

        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set
            {
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged(nameof(Date));
                }
            }
        }

        private string _response;
        public string Response
        {
            get => _response;
            set
            {
                if (_response != value)
                {
                    _response = value;
                    OnPropertyChanged(nameof(Response));
                }
            }
        }

        public ICommand BackCommand { get; }
        public ICommand SaveCommand { get; }


        public EditGradeViewModel(UniversityContext context, IDialogService dialogService, Grade grade)
    : base(context, dialogService)
        {
            _context = context;
            _grade = grade ?? throw new ArgumentNullException(nameof(grade));

            _response = string.Empty;
            _selectedSubjectName = grade.Subject?.Name ?? string.Empty;
            _selectedStudentLastName = grade.Student?.LastName ?? string.Empty;

            GradeValue = grade.GradeValue;
            Date = grade.Date;

            LoadStudentLastNames();
            LoadSubjectNames();

            BackCommand = new RelayCommand(NavigateBack);
            SaveCommand = new AsyncRelayCommand(SaveData);
        }


        private void LoadStudentLastNames()
        {
            StudentLastNames.Clear();
            foreach (var lastName in _context.Students.Select(s => s.LastName).Distinct())
            {
                StudentLastNames.Add(lastName);
            }
        }

        private void LoadSubjectNames()
        {
            SubjectNames.Clear();
            foreach (var name in _context.Subjects.Select(s => s.Name).Distinct())
            {
                SubjectNames.Add(name);
            }
        }

        private void NavigateBack()
        {
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.GradesSubView = new GradeViewModel(_context, _dialogService);
            }
        }

        private async Task SaveData()
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

            if (_grade is null) return;

            _grade.GradeValue = GradeValue;
            _grade.StudentId = student.StudentId;
            _grade.SubjectId = subject.SubjectId;
            _grade.Date = Date;

            _context.Entry(_grade).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            Response = "Grade Data Updated";
        }

        private new bool IsValid()
        {
            return string.IsNullOrEmpty(this[nameof(GradeValue)])
                && string.IsNullOrEmpty(this[nameof(SelectedStudentLastName)])
                && string.IsNullOrEmpty(this[nameof(SelectedSubjectName)]);
        }
    }
}
