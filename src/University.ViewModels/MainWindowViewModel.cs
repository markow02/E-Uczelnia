using University.Interfaces;
using University.Data;

namespace University.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly UniversityContext _context;
    private readonly IDialogService _dialogService;
    private readonly IClassroomService _classroomService;
    private readonly IActivityClubService _activityClubService;

    private int _selectedTab;
    public int SelectedTab
    {
        get
        {
            return _selectedTab;
        }
        set
        {
            _selectedTab = value;
            OnPropertyChanged(nameof(SelectedTab));
        }
    }

    private object? _studentsSubView = null;
    public object? StudentsSubView
    {
        get
        {
            return _studentsSubView;
        }
        set
        {
            _studentsSubView = value;
            OnPropertyChanged(nameof(StudentsSubView));
        }
    }

    private object? _classroomsSubView = null;
    public object? ClassroomsSubView
    {
        get
        {
            return _classroomsSubView;
        }
        set
        {
            _classroomsSubView = value;
            OnPropertyChanged(nameof(ClassroomsSubView));
        }
    }

    private object? _subjectsSubView = null;
    public object? SubjectsSubView
    {
        get
        {
            return _subjectsSubView;
        }
        set
        {
            _subjectsSubView = value;
            OnPropertyChanged(nameof(SubjectsSubView));
        }
    }

    private object? _searchSubView = null;
    public object? SearchSubView
    {
        get
        {
            return _searchSubView;
        }
        set
        {
            _searchSubView = value;
            OnPropertyChanged(nameof(SearchSubView));
        }
    }

    private object? _enrollmentsSubView = null;
    public object? EnrollmentsSubView
    {
        get
        {
            return _enrollmentsSubView;
        }
        set
        {
            _enrollmentsSubView = value;
            OnPropertyChanged(nameof(EnrollmentsSubView));
        }
    }

    private object? _gradesSubView = null;
    public object? GradesSubView
    {
        get
        {
            return _gradesSubView;
        }
        set
        {
            _gradesSubView = value;
            OnPropertyChanged(nameof(GradesSubView));
        }
    }

    public void SetEditGradeView(EditGradeViewModel editGradeViewModel)
    {
        GradesSubView = editGradeViewModel;
        OnPropertyChanged(nameof(GradesSubView));
    }

    private object? _activityClubSubView = null;
    public object? ActivityClubSubView
    {
        get
        {
            return _activityClubSubView;
        }
        set
        {
            _activityClubSubView = value;
            OnPropertyChanged(nameof(ActivityClubSubView));
        }
    }


    private static MainWindowViewModel? _instance = null;
    public static MainWindowViewModel? Instance()
    {
        return _instance;
    }

    public MainWindowViewModel(UniversityContext context, IDialogService dialogService, IClassroomService classroomService, IActivityClubService activityClubService)
    {
        _context = context;
        _dialogService = dialogService;
        _classroomService = classroomService;
        _activityClubService = activityClubService;

        if (_instance is null)
        {
            _instance = this;
        }

        StudentsSubView = new StudentsViewModel(_context, _dialogService);
        SubjectsSubView = new SubjectsViewModel(_context, _dialogService);
        ClassroomsSubView = new ClassroomsViewModel(_classroomService, _dialogService);
        SearchSubView = new SearchViewModel(_context, _dialogService);
        GradesSubView = new GradeViewModel(_context, _dialogService);
        EnrollmentsSubView = new EnrollmentViewModel(_context, _dialogService);
        ActivityClubSubView = new ActivityClubViewModel(_activityClubService, _dialogService);
    }
}
