using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;
using Microsoft.EntityFrameworkCore;
namespace University.ViewModels
{
    public class ExamViewModel : ViewModelBase
    {
        private readonly UniversityContext _context;
        private readonly IDialogService _dialogService;

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

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand RemoveCommand { get; }

        public ExamViewModel(UniversityContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;

            _context.Database.EnsureCreated();
            _context.Exams.Load();
            Exams = _context.Exams.Local.ToObservableCollection();

            AddCommand = new RelayCommand(AddNewExam);
            EditCommand = new RelayCommand<object>(EditExam);
            RemoveCommand = new AsyncRelayCommand<object>(RemoveExam);
        }

        private void AddNewExam()
        {
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.ExamSubView = new AddExamViewModel(_context, _dialogService);
            }
        }

        private void EditExam(object? obj)
        {
            if (obj is Exam exam)
            {
                var instance = MainWindowViewModel.Instance();
                instance?.SetEditExamView(new EditExamViewModel(_context, _dialogService, exam));
            }
        }

        private async Task RemoveExam(object? obj)
        {
            if (obj is Exam exam)
            {
                var confirmed = _dialogService.Show("Are you sure you want to remove this exam?");
                if (confirmed == true)
                {
                    Exams?.Remove(exam);
                    _context.Exams.Remove(exam);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
