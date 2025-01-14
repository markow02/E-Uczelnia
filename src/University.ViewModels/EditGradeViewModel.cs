using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;

namespace University.ViewModels
{
    public class EditGradeViewModel : GradeBaseViewModel, IDataErrorInfo
    {
        private readonly UniversityContext _context;
        private Grade? _grade = new Grade();

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                if (columnName == "Value")
                {
                    if (Value <= 0)
                    {
                        return "Grade value must be greater than 0";
                    }
                }
                if (columnName == "StudentId")
                {
                    if (StudentId <= 0)
                    {
                        return "Student ID is required";
                    }
                }
                if (columnName == "SubjectId")
                {
                    if (SubjectId <= 0)
                    {
                        return "Subject ID is required";
                    }
                }
                return string.Empty;
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

        private int _gradeId = 0;
        public int GradeId
        {
            get => _gradeId;
            set
            {
                _gradeId = value;
                OnPropertyChanged(nameof(GradeId));
                LoadGradeData();
            }
        }

        private ICommand? _back;
        public ICommand Back
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

        private void SaveData(object? obj)
        {
            if (!IsValid())
            {
                Response = "Please complete all required fields";
                return;
            }

            if (_grade is null)
            {
                return;
            }

            _grade.GradeValue = Value;
            _grade.StudentId = StudentId;
            _grade.SubjectId = SubjectId;

            _context.Entry(_grade).State = EntityState.Modified;
            _context.SaveChanges();

            Response = "Grade Data Updated";
        }

        public EditGradeViewModel(UniversityContext context, IDialogService dialogService)
            : base(context, dialogService)
        {
            _context = context;
        }

        private bool IsValid()
        {
            string[] properties = { "Value", "StudentId", "SubjectId" };
            foreach (string property in properties)
            {
                if (!string.IsNullOrEmpty(this[property]))
                {
                    return false;
                }
            }
            return true;
        }

        private void LoadGradeData()
        {
            var grades = _context.Grades;
            if (grades is not null)
            {
                _grade = grades.Find(GradeId);
                if (_grade is null)
                {
                    return;
                }
                Value = _grade.GradeValue;
                StudentId = (int)_grade.StudentId; // Explicit cast to int
                SubjectId = (int)_grade.SubjectId; // Explicit cast to int
            }
        }
    }
}
