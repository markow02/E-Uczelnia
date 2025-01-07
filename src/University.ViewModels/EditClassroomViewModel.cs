using System.Threading.Tasks;
using University.Interfaces;

namespace University.ViewModels;

public class EditClassroomViewModel : ClassroomBaseViewModel
{
    private readonly IClassroomService _classroomService;

    public EditClassroomViewModel(IClassroomService classroomService, IDialogService dialogService)
        : base(classroomService, dialogService)
    {
        _classroomService = classroomService;
    }

    public override async void SaveData(object? obj)
    {
        if (!IsValid())
        {
            Response = "Please complete all required fields";
            return;
        }

        var classroom = await _classroomService.GetClassroomByIdAsync(ClassroomId);
        if (classroom != null)
        {
            classroom.ClassroomNumber = this.ClassroomName;
            classroom.Capacity = this.Capacity;
            classroom.Floor = this.Floor;
            classroom.HasProjector = this.HasProjector;
            classroom.IsLab = this.IsLab;

            await _classroomService.SaveDataAsync(classroom);
            Response = "Classroom Data Updated";
        }
        else
        {
            Response = "Error: Classroom not found";
        }
    }
}
