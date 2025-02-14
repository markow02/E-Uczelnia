namespace University.Models
{
    public class Enrollment
    {
        public long EnrollmentId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string CandidateSurname { get; set; } = string.Empty;
        public string CandidateSchool { get; set; } = string.Empty;
    }
}
