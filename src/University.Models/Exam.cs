using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Models
{
    public class Exam
    {
        public long ExamId { get; set; }
        public long ClassroomId { get; set; }
        public long SubjectId { get; set; }
        public DateTime ExamDate1 { get; set; }
        public DateTime ExamDate2 { get; set; }
        public string ExamType { get; set; } = string.Empty;

        // Navigation properties
        public virtual Subject Subject { get; set; } = null!;
        public virtual Classroom Classroom { get; set; } = null!;
    }
}
