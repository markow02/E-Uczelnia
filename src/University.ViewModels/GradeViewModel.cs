using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;
using Microsoft.EntityFrameworkCore;

namespace University.ViewModels
{
    public class GradeViewModel : ViewModelBase
    {
        private readonly UniversityContext _context;
        private readonly IDialogService _dialogService;

        private bool? _dialogResult;
        public bool? DialogResult
        {
            get => _dialogResult;
            set
            {
                _dialogResult = value;
                OnPropertyChanged(nameof(DialogResult));
            }
        }

        private ObservableCollection<Grade> _grades;
        public ObservableCollection<Grade> Grades
        {
            get => _grades;
            set
            {
                _grades = value;
                OnPropertyChanged(nameof(Grades));
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand RemoveCommand { get; }

        public GradeViewModel(UniversityContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;

            _grades = new ObservableCollection<Grade>();
            _context.Database.EnsureCreated();
            _context.Grades.Load();
            Grades = _context.Grades.Local.ToObservableCollection();

            AddCommand = new RelayCommand(AddNewGrade);
            EditCommand = new RelayCommand<object>(EditGrade);
            RemoveCommand = new AsyncRelayCommand<object>(RemoveGrade);
        }


        private void AddNewGrade()
        {
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.GradesSubView = new AddGradeViewModel(_context, _dialogService);
            }

        }

        private void EditGrade(object? obj)
        {
            if (obj is Grade grade)
            {
                var instance = MainWindowViewModel.Instance();
                instance?.SetEditGradeView(new EditGradeViewModel(_context, _dialogService, grade));
            }
        }

        private async Task RemoveGrade(object? obj)
        {
            if (obj is Grade grade)
            {
                var confirmed = _dialogService.Show($"Are you sure you want to remove this grade?");
                if (confirmed == true)
                {
                    Grades.Remove(grade);
                    _context.Grades.Remove(grade);
                    await _context.SaveChangesAsync();
                }
            }
        }

    }
}
