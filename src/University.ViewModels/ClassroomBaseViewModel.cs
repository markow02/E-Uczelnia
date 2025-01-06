using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;

namespace University.ViewModels;

public abstract class ClassroomBaseViewModel : ViewModelBase, IDataErrorInfo
{
    #region Properties And Ctor

    protected readonly UniversityContext _context;
    protected readonly IDialogService _dialogService;
    protected Classroom? _classroom = new Classroom();

    protected ClassroomBaseViewModel(
        UniversityContext context,
        IDialogService dialogService)
    {
        _context = context;
        _dialogService = dialogService;
    }

    public string Error
    {
        get { return string.Empty; }
    }

    public string this[string columnName]
    {
        get
        {
            if (columnName == "ClassroomName")
            {
                if (string.IsNullOrEmpty(ClassroomName))
                {
                    return "Classroom name is required";
                }
            }
            if (columnName == "Capacity")
            {
                if (Capacity <= 0)
                {
                    return "Capacity must be greater than 0";
                }
            }
            if (columnName == "Floor")
            {
                if (Floor <= 0)
                {
                    return "Floor must be greater than 0";
                }
            }
            return string.Empty;
        }
    }

    private long _classroomId = 0;
    public long ClassroomId
    {
        get
        {
            return _classroomId;
        }
        set
        {
            _classroomId = value;
            OnPropertyChanged(nameof(ClassroomId));
            LoadRoomData();
        }
    }

    private string _classroomName = string.Empty;
    public string ClassroomName
    {
        get
        {
            return _classroomName;
        }
        set
        {
            _classroomName = value;
            OnPropertyChanged(nameof(ClassroomName));
        }
    }

    private int _capacity = 0;
    public int Capacity
    {
        get
        {
            return _capacity;
        }
        set
        {
            _capacity = value;
            OnPropertyChanged(nameof(Capacity));
        }
    }

    private int _floor = 0;
    public int Floor
    {
        get
        {
            return _floor;
        }
        set
        {
            _floor = value;
            OnPropertyChanged(nameof(Floor));
        }
    }

    private bool _hasProjector = false;
    public bool HasProjector
    {
        get
        {
            return _hasProjector;
        }
        set
        {
            _hasProjector = value;
            OnPropertyChanged(nameof(HasProjector));
        }
    }

    private bool _isLab = false;
    public bool IsLab
    {
        get
        {
            return _isLab;
        }
        set
        {
            _isLab = value;
            OnPropertyChanged(nameof(IsLab));
        }
    }

    private string _response = string.Empty;
    public string Response
    {
        get
        {
            return _response;
        }
        set
        {
            _response = value;
            OnPropertyChanged(nameof(Response));
        }
    }

    private ICommand? _back = null;
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
            instance.ClassroomsSubView = new ClassroomsViewModel(_context, _dialogService);
        }
    }

    private ICommand? _save = null;
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

    #endregion // Properties And Ctor

    #region Public Methods  

    public virtual void SaveData(object? obj)
    {
        // TODO: Implement SaveData in derived classes
    }

    #endregion // Public Methods

    #region Protected Methods

    protected bool IsValid()
    {
        var errors = new List<string>
    {
        this["ClassroomName"],
        this["Capacity"],
        this["Floor"]
    };

        return errors.All(string.IsNullOrEmpty);
    }


    protected void LoadRoomData()
    {
        if (_context?.Classrooms is null)
        {
            return;
        }
        _classroom = _context.Classrooms.Find(ClassroomId);
        if (_classroom is null)
        {
            return;
        }
        this.ClassroomName = _classroom.ClassroomNumber; // Fixed property name
        this.Capacity = _classroom.Capacity;
        this.Floor = _classroom.Floor;
        this.HasProjector = _classroom.HasProjector;
        this.IsLab = _classroom.IsLab;
    }

    #endregion // Protected Methods
}
