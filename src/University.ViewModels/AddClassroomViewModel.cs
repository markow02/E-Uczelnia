using University.Interfaces;
using University.Models;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;



namespace University.ViewModels;

public class AddClassroomViewModel : ClassroomBaseViewModel
{
    private readonly IClassroomService _classroomService;

    public AddClassroomViewModel(IClassroomService classroomService, IDialogService dialogService)
    : base(classroomService, dialogService)
    {
        _classroomService = classroomService;
        SaveCommand = new RelayCommand(async () => await SaveDataAsync(null));
    }

    public string ClassroomNumber
    {
        get => ClassroomName;
        set => ClassroomName = value;
    }

    public ICommand SaveCommand { get; }

    public async Task SaveDataAsync(object? obj)
    {
        var classroom = new Classroom
        {
            ClassroomNumber = this.ClassroomName,
            Capacity = this.Capacity,
            Floor = this.Floor,
            HasProjector = this.HasProjector,
            IsLab = this.IsLab
        };

        if (!await _classroomService.IsValidAsync(classroom))
        {
            Response = "Please complete all required fields";
            return;
        }

        try
        {
            await _classroomService.SaveDataAsync(classroom);
            Response = "Classroom Data Saved";
        }
        catch
        {
            Response = "Failed to save classroom data";
        }
    }
}
