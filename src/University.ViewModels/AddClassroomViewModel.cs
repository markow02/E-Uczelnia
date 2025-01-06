using University.Data;
using University.Interfaces;
using University.Models;

namespace University.ViewModels;

public class AddClassroomViewModel : ClassroomBaseViewModel
{
    public AddClassroomViewModel(UniversityContext context, IDialogService dialogService)
        : base(context, dialogService)
    {
    }
    public string ClassroomNumber
    {
        get => ClassroomName;
        set => ClassroomName = value;
    }

    public override void SaveData(object? obj)
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

        _context.Classrooms.Add(classroom);
        _context.SaveChanges();

        Response = "Classroom Data Saved";
    }


}
