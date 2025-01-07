using System.Threading.Tasks;
using University.Interfaces;
using University.Models;

namespace University.ViewModels;

public class AddClassroomViewModel : ClassroomBaseViewModel
{
    private readonly IClassroomService _classroomService;

    public AddClassroomViewModel(IClassroomService classroomService, IDialogService dialogService)
        : base(classroomService, dialogService)
    {
        _classroomService = classroomService;
    }

    public string ClassroomNumber
    {
        get => ClassroomName;
        set => ClassroomName = value;
    }

    public override async void SaveData(object? obj)
    {
        if (!IsValid())
        {
            Response = "Please complete all required fields";
            return;
        }

        var classroom = new Classroom
        {
            ClassroomNumber = this.ClassroomName,
            Capacity = this.Capacity,
            Floor = this.Floor,
            HasProjector = this.HasProjector,
            IsLab = this.IsLab
        };

        // Zapisujemy dane za pomocą ClassroomService
        await _classroomService.SaveDataAsync(classroom);

        Response = "Classroom Data Saved";
    }
}
