using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows.Input;
using University.Interfaces;
using University.Models;

namespace University.ViewModels;

public abstract class ClassroomBaseViewModel : ViewModelBase, IDataErrorInfo
{
    #region Properties And Ctor

    protected readonly IClassroomService _classroomService;
    protected readonly IDialogService _dialogService;
    protected Classroom? _classroom = new Classroom();

    protected ClassroomBaseViewModel(
        IClassroomService classroomService,
        IDialogService dialogService)
    {
        _classroomService = classroomService;
        _dialogService = dialogService;
    }

    public string Error => string.Empty;

    public string this[string columnName]
    {
        get
        {
            return columnName switch
            {
                "ClassroomName" when string.IsNullOrEmpty(ClassroomName) => "Classroom name is required",
                "Capacity" when Capacity <= 0 => "Capacity must be greater than 0",
                "Floor" when Floor <= 0 => "Floor must be greater than 0",
                _ => string.Empty,
            };
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
            _ = LoadRoomDataAsync();
        }
    }

    private string _classroomName = string.Empty;
    public string ClassroomName
    {
        get => _classroomName;
        set
        {
            _classroomName = value;
            OnPropertyChanged(nameof(ClassroomName));
        }
    }

    private int _capacity;
    public int Capacity
    {
        get => _capacity;
        set
        {
            _capacity = value;
            OnPropertyChanged(nameof(Capacity));
        }
    }

    private int _floor;
    public int Floor
    {
        get => _floor;
        set
        {
            _floor = value;
            OnPropertyChanged(nameof(Floor));
        }
    }

    private bool _hasProjector;
    public bool HasProjector
    {
        get => _hasProjector;
        set
        {
            _hasProjector = value;
            OnPropertyChanged(nameof(HasProjector));
        }
    }

    private bool _isLab;
    public bool IsLab
    {
        get => _isLab;
        set
        {
            _isLab = value;
            OnPropertyChanged(nameof(IsLab));
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
    public ICommand Back => _back ??= new RelayCommand<object>(NavigateBack);

    private void NavigateBack(object? obj)
    {
        var instance = MainWindowViewModel.Instance();
        if (instance is not null)
        {
            instance.ClassroomsSubView = new ClassroomsViewModel(_classroomService, _dialogService);
        }
    }


    #endregion // Properties And Ctor

    #region Protected Methods

    protected async Task LoadRoomDataAsync()
    {
        if (_classroomService is null)
        {
            return;
        }

        _classroom = await _classroomService.GetClassroomByIdAsync(ClassroomId);
        if (_classroom is null)
        {
            return;
        }

        ClassroomName = _classroom.ClassroomNumber; // Fixed property name
        Capacity = _classroom.Capacity;
        Floor = _classroom.Floor;
        HasProjector = _classroom.HasProjector;
        IsLab = _classroom.IsLab;
    }

    #endregion // Protected Methods
}
