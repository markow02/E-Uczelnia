using Microsoft.EntityFrameworkCore;
using University.Data;
using University.Interfaces;

namespace University.ViewModels;

public class EditClassroomViewModel : ClassroomBaseViewModel
{
    public EditClassroomViewModel(UniversityContext context, IDialogService dialogService)
        : base(context, dialogService)
    {
    }

    private string _classroomNumber = string.Empty;
    public string ClassroomNumber
    {
        get => _classroomNumber;
        set
        {
            _classroomNumber = value;
            OnPropertyChanged(nameof(ClassroomNumber));
        }
    }

    public override void SaveData(object? obj)
    {
        if (!IsValid())
        {
            Response = "Please complete all required fields";
            return;
        }

        if (_classroom is null)
        {
            return;
        }

        _classroom.ClassroomNumber = ClassroomNumber;
        _classroom.Capacity = Capacity;
        _classroom.Floor = Floor;
        _classroom.HasProjector = HasProjector;
        _classroom.IsLab = IsLab;

        _context.Entry(_classroom).State = EntityState.Modified;
        _context.SaveChanges();

        Response = "Classroom data updated successfully";
    }
}
