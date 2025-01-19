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
        private readonly UniversityContext _context;
        private Grade? _grade;

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                if (columnName == "GradeValue" && GradeValue <= 0)
                {
                    return "Grade value must be greater than 0";
                }
                if (columnName == "SelectedSubjectName" && string.IsNullOrEmpty(SelectedSubjectName))
                {
                    return "Subject Name is required";
                }
                if (columnName == "SelectedStudentLastName" && string.IsNullOrEmpty(SelectedStudentLastName))
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
                _gradeValue = value;
                OnPropertyChanged(nameof(GradeValue));
            }
        }

        private string _selectedStudentLastName;
        public string SelectedStudentLastName
        {
            get => _selectedStudentLastName;
            set
            {
                _selectedStudentLastName = value;
                OnPropertyChanged(nameof(SelectedStudentLastName));
            }
        }

        public ObservableCollection<string> StudentLastNames { get; } = new();

        private string _selectedSubjectName;
        public string SelectedSubjectName
        {
            get => _selectedSubjectName;
            set
            {
                _selectedSubjectName = value;
                OnPropertyChanged(nameof(SelectedSubjectName));
            }
        }

        public ObservableCollection<string> SubjectNames { get; } = new();

        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }

        private string _response;
        public string Response
        {
            get => _response;
            set
            {
                _response = value;
                OnPropertyChanged(nameof(Response));
            }
        }

        public ICommand BackCommand { get; }
        public ICommand SaveCommand { get; }

        public EditGradeViewModel(UniversityContext context, IDialogService dialogService, Grade grade)
            : base(context, dialogService)
        {
            _context = context;
            _grade = grade;

            GradeValue = grade.GradeValue;
            SelectedStudentLastName = grade.Student?.LastName ?? string.Empty;
            SelectedSubjectName = grade.Subject?.Name ?? string.Empty;
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

        private bool IsValid()
        {
            return string.IsNullOrEmpty(this["GradeValue"])
                && string.IsNullOrEmpty(this["SelectedStudentLastName"])
                && string.IsNullOrEmpty(this["SelectedSubjectName"]);
        }
    }
}
