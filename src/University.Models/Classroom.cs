namespace University.Models
{
    public class Classroom
    {
        public long ClassroomId { get; set; }
        public string ClassroomNumber { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int Floor { get; set; }
        public bool HasProjector { get; set; }
        public bool IsLab { get; set; }
    }
}
