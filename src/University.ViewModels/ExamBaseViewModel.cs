using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;

namespace University.ViewModels
{
    public abstract class ExamBaseViewModel : ViewModelBase, IDataErrorInfo
    {
        #region Fields

        protected readonly UniversityContext _context;
        protected readonly IDialogService _dialogService;
        protected Exam? _exam = new Exam();

        #endregion // Fields

        #region Constructor

        protected ExamBaseViewModel(UniversityContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;

            _context.Database.EnsureCreated();
            _context.Exams.Load();
            Exams = _context.Exams.Local.ToObservableCollection();
        }

        #endregion // Constructor

        #region IDataErrorInfo Members

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                return columnName switch
                {
                    "ClassroomId" when ClassroomId <= 0 => "Classroom ID is required",
                    "SubjectId" when SubjectId <= 0 => "Subject ID is required",
                    _ => string.Empty,
                };
            }
        }

        #endregion // IDataErrorInfo Members

        #region Properties

        private long _examId;
        public long ExamId
        {
            get => _examId;
            set
            {
                _examId = value;
                OnPropertyChanged(nameof(ExamId));
                _ = LoadExamDataAsync();
            }
        }

        private long _classroomId;
        public long ClassroomId
        {
            get => _classroomId;
            set
            {
                _classroomId = value;
                OnPropertyChanged(nameof(ClassroomId));
            }
        }

        private long _subjectId;
        public long SubjectId
        {
            get => _subjectId;
            set
            {
                _subjectId = value;
                OnPropertyChanged(nameof(SubjectId));
            }
        }

        private DateTime _examDate1 = DateTime.Today;
        public DateTime ExamDate1
        {
            get => _examDate1;
            set
            {
                _examDate1 = value;
                OnPropertyChanged(nameof(ExamDate1));
            }
        }

        private DateTime _examDate2 = DateTime.Today;
        public DateTime ExamDate2
        {
            get => _examDate2;
            set
            {
                _examDate2 = value;
                OnPropertyChanged(nameof(ExamDate2));
            }
        }

        private ObservableCollection<Exam>? _exams;
        public ObservableCollection<Exam>? Exams
        {
            get => _exams;
            set
            {
                _exams = value;
                OnPropertyChanged(nameof(Exams));
            }
        }

        private ICommand? _back;
        public ICommand Back => _back ??= new RelayCommand<object>(NavigateBack);

        #endregion // Properties

        #region Protected Methods

        protected bool IsValid()
        {
            var errors = new List<string>
                {
                    this["ClassroomId"],
                    this["SubjectId"]
                };

            return errors.All(string.IsNullOrEmpty);
        }

        protected async Task LoadExamDataAsync()
        {
            if (ExamId <= 0) return;

            _exam = await _context.Exams.FindAsync(ExamId);
            if (_exam is null) return;

            ClassroomId = _exam.ClassroomId;
            SubjectId = _exam.SubjectId;
            ExamDate1 = _exam.ExamDate1;
            ExamDate2 = _exam.ExamDate2;
        }

        private void NavigateBack(object? obj)
        {
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                // Replace with the relevant "Exams" view model if needed.
                // For example: instance.ExamsSubView = new ExamsViewModel(_context, _dialogService);
            }
        }

        #endregion // Protected Methods
    }
}
