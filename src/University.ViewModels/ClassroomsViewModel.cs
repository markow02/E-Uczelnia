using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using University.Interfaces;
using University.Models;

namespace University.ViewModels
{
    public class ClassroomsViewModel : ViewModelBase
    {
        private readonly IClassroomService _classroomService;
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

        private ObservableCollection<Classroom>? _classrooms;
        public ObservableCollection<Classroom>? Classrooms
        {
            get => _classrooms;
            private set
            {
                _classrooms = value;
                OnPropertyChanged(nameof(Classrooms));
            }
        }

        private ICommand? _add;
        public ICommand Add => _add ??= new RelayCommand(AddNewClassroom);

        private void AddNewClassroom()
        {
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.ClassroomsSubView = new AddClassroomViewModel(_classroomService, _dialogService);
            }
        }

        private ICommand? _edit;
        public ICommand Edit => _edit ??= new RelayCommand<object>(EditClassroom);

        private void EditClassroom(object? obj)
        {
            if (obj is long classroomId)
            {
                var editClassroomViewModel = new EditClassroomViewModel(_classroomService, _dialogService)
                {
                    ClassroomId = classroomId
                };

                var instance = MainWindowViewModel.Instance();
                if (instance is not null)
                {
                    instance.ClassroomsSubView = editClassroomViewModel;
                }
            }
        }

        private ICommand? _remove;
        public ICommand Remove => _remove ??= new RelayCommand<object>(RemoveClassroom);

        private async void RemoveClassroom(object? obj)
        {
            if (obj is long classroomId)
            {
                var classroom = Classrooms?.FirstOrDefault(c => c.ClassroomId == classroomId);
                if (classroom is not null)
                {
                    DialogResult = _dialogService.Show(
                        $"Are you sure you want to remove the classroom '{classroom.ClassroomNumber}' located on floor {classroom.Floor}?"
                    );

                    if (DialogResult == true)
                    {
                        Classrooms?.Remove(classroom);
                        await _classroomService.SaveDataAsync(classroom);
                    }
                }
            }
        }

        public ClassroomsViewModel(IClassroomService classroomService, IDialogService dialogService)
        {
            _classroomService = classroomService;
            _dialogService = dialogService;

            LoadClassrooms();
        }

        private async void LoadClassrooms()
        {
            Classrooms = new ObservableCollection<Classroom>(await _classroomService.LoadDataAsync());
        }
    }
}
