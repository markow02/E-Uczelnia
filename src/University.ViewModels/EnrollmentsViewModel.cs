using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;
namespace University.ViewModels
{
    public class EnrollmentsViewModel : ViewModelBase
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

        private ObservableCollection<Enrollment>? _enrollments;
        public ObservableCollection<Enrollment>? Enrollments
        {
            get => _enrollments;
            set
            {
                _enrollments = value;
                OnPropertyChanged(nameof(Enrollments));
            }
        }

        public EnrollmentsViewModel(UniversityContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;

            _context.Database.EnsureCreated();
            _context.Students.Load();
            Enrollments = _context.Enrollments.Local.ToObservableCollection();
        }
    }
}
