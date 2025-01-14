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

        private bool? _dialogResult = null;
        public bool? DialogResult
        {
            get => _dialogResult;
            set
            {
                _dialogResult = value;
                OnPropertyChanged(nameof(DialogResult));
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

        private ICommand? _add;
        public ICommand Add => _add ??= new RelayCommand(AddNewGrade);

        private void AddNewGrade()
        {
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.GradesSubView = new AddGradeViewModel(_context, _dialogService);
            }
        }

        private ICommand? _edit;
        public ICommand Edit => _edit ??= new RelayCommand<object>(EditGrade);

        private void EditGrade(object? obj)
        {
            if (obj is int gradeId)
            {
                var editGradeViewModel = new EditGradeViewModel(_context, _dialogService)
                {
                    GradeId = gradeId
                };

                var instance = MainWindowViewModel.Instance();
                if (instance is not null)
                {
                    instance.GradesSubView = editGradeViewModel;
                }
            }
        }

        private ICommand? _remove;
        public ICommand Remove => _remove ??= new RelayCommand<object>(RemoveGrade);

        private async void RemoveGrade(object? obj)
        {
            if (obj is int gradeId)
            {
                var grade = Grades?.FirstOrDefault(g => g.GradeId == gradeId);
                if (grade is not null)
                {
                    DialogResult = _dialogService.Show(
                        $"Are you sure you want to remove the grade for '{grade.Student.Name} in {grade.Subject.Name}'?"
                    );

                    if (DialogResult == true)
                    {
                        Grades?.Remove(grade);
                        _context.Grades.Remove(grade);
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }

        public GradeViewModel(UniversityContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;

            _context.Database.EnsureCreated();
            _context.Grades.Load();
            Grades = _context.Grades.Local.ToObservableCollection();
        }
    }
}
