namespace University.Models
{
    public class Grade
    {
        public long GradeId { get; set; }
        public long StudentId { get; set; }
        public long SubjectId { get; set; }
        public double GradeValue { get; set; }
        public DateTime Date { get; set; }

        // Navigation properties
        public virtual Student Student { get; set; } = null!;
        public virtual Subject Subject { get; set; } = null!;

    }
}
