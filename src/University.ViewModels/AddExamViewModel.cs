using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;
using System.Collections.ObjectModel;

namespace University.ViewModels
{
    public class AddExamViewModel : ExamBaseViewModel, IDataErrorInfo
    {
        private new readonly UniversityContext _context;

        public new string Error => string.Empty;

        public new string this[string columnName]
        {
            get
            {
                if (columnName == nameof(SelectedSubjectName) && string.IsNullOrEmpty(SelectedSubjectName))
                {
                    return "Subject Name is required";
                }
                if (columnName == nameof(SelectedClassroomNumber) && string.IsNullOrEmpty(SelectedClassroomNumber))
                {
                    return "Classroom Number is required";
                }
                if (columnName == nameof(ExamDate1) && ExamDate1 == default)
                {
                    return "Exam Date 1 is required";
                }
                if (columnName == nameof(ExamDate2) && ExamDate2 == default)
                {
                    return "Exam Date 2 is required";
                }
                if (columnName == nameof(SelectedExamType) && string.IsNullOrEmpty(SelectedExamType))
                {
                    return "Exam Type is required";
                }
                return string.Empty;
            }
        }

        private string _selectedClassroomNumber;
        public string SelectedClassroomNumber
        {
            get => _selectedClassroomNumber;
            set
            {
                if (_selectedClassroomNumber != value)
                {
                    _selectedClassroomNumber = value;
                    OnPropertyChanged(nameof(SelectedClassroomNumber));
                }
            }
        }

        public ObservableCollection<string> ClassroomNumbers { get; } = new();

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

        private DateTime _examDate1;
        public new DateTime ExamDate1
        {
            get => _examDate1;
            set
            {
                if (_examDate1 != value)
                {
                    _examDate1 = value;
                    OnPropertyChanged(nameof(ExamDate1));
                }
            }
        }

        private DateTime _examDate2;
        public new DateTime ExamDate2
        {
            get => _examDate2;
            set
            {
                if (_examDate2 != value)
                {
                    _examDate2 = value;
                    OnPropertyChanged(nameof(ExamDate2));
                }
            }
        }

        private string _selectedExamType;
        public string SelectedExamType
        {
            get => _selectedExamType;
            set
            {
                if (_selectedExamType != value)
                {
                    _selectedExamType = value;
                    OnPropertyChanged(nameof(SelectedExamType));
                }
            }
        }

        public ObservableCollection<string> ExamTypes { get; } = new() { "Written", "Oral" };

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

        public AddExamViewModel(UniversityContext context, IDialogService dialogService)
            : base(context, dialogService)
        {
            _context = context;

            _response = string.Empty;
            _selectedSubjectName = string.Empty;
            _selectedClassroomNumber = string.Empty;
            _examDate1 = DateTime.Today;
            _examDate2 = DateTime.Today;
            _selectedExamType = string.Empty;

            LoadClassroomNumbers();
            LoadSubjectNames();

            BackCommand = new RelayCommand(NavigateBack);
            SaveCommand = new AsyncRelayCommand(SaveData);
        }

        private void LoadClassroomNumbers()
        {
            ClassroomNumbers.Clear();
            foreach (var number in _context.Classrooms.Select(c => c.ClassroomNumber).Distinct())
            {
                ClassroomNumbers.Add(number);
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
                instance.ExamSubView = new ExamViewModel(_context, _dialogService);
            }
        }

        private async Task SaveData()
        {
            if (!IsValid())
            {
                Response = "Please complete all required fields";
                return;
            }

            var classroom = _context.Classrooms.FirstOrDefault(c => c.ClassroomNumber == SelectedClassroomNumber);
            if (classroom == null)
            {
                Response = "Selected classroom not found";
                return;
            }

            var subject = _context.Subjects.FirstOrDefault(s => s.Name == SelectedSubjectName);
            if (subject == null)
            {
                Response = "Selected subject not found";
                return;
            }

            var exam = new Exam
            {
                ClassroomId = classroom.ClassroomId,
                SubjectId = subject.SubjectId,
                ExamDate1 = ExamDate1,
                ExamDate2 = ExamDate2,
                ExamType = SelectedExamType
            };

            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();

            Response = "Exam Data Saved";
        }

        private new bool IsValid()
        {
            return string.IsNullOrEmpty(this[nameof(SelectedSubjectName)])
                && string.IsNullOrEmpty(this[nameof(SelectedClassroomNumber)])
                && string.IsNullOrEmpty(this[nameof(ExamDate1)])
                && string.IsNullOrEmpty(this[nameof(ExamDate2)])
                && string.IsNullOrEmpty(this[nameof(SelectedExamType)]);
        }
    }
}
