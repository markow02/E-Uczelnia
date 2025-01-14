using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;

namespace University.ViewModels
{
    public class AddGradeViewModel : GradeBaseViewModel, IDataErrorInfo
    {
        private readonly UniversityContext _context;

        public new string Error => string.Empty;

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

        private async void SaveData(object? obj)
        {
            if (!IsValid())
            {
                Response = "Please complete all required fields";
                return;
            }

            var grade = new Grade
            {
                GradeValue = Value,
                StudentId = StudentId,
                SubjectId = SubjectId
            };

            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();

            Response = "Grade Data Saved";
        }

        public AddGradeViewModel(UniversityContext context, IDialogService dialogService)
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
    }
}
