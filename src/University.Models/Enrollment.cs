using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Models
{
    public class Enrollment
    {
        public long EnrollmentId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string CandidateSurname { get; set; } = string.Empty;
    }
}
