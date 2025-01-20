using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using University.Interfaces;


namespace University.ViewModels;

public class EditClassroomViewModel : ClassroomBaseViewModel
{
    private new readonly IClassroomService _classroomService;

    public EditClassroomViewModel(IClassroomService classroomService, IDialogService dialogService)
        : base(classroomService, dialogService)
    {
        _classroomService = classroomService;
        SaveCommand = new RelayCommand(async () => await SaveDataAsync());
    }

    public ICommand SaveCommand { get; }

    private async Task SaveDataAsync()
    {
        var classroom = await _classroomService.GetClassroomByIdAsync(ClassroomId);
        if (classroom == null)
        {
            Response = "Error: Classroom not found";
            return;
        }

        classroom.ClassroomNumber = ClassroomName;
        classroom.Capacity = Capacity;
        classroom.Floor = Floor;
        classroom.HasProjector = HasProjector;
        classroom.IsLab = IsLab;

        if (!await _classroomService.IsValidAsync(classroom))
        {
            Response = "Please complete all required fields";
            return;
        }

        await _classroomService.SaveDataAsync(classroom);
        Response = "Classroom Data Updated";
    }
}
