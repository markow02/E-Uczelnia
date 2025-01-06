using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Models
{
    public class Classroom
    {
        public long ClassroomId { get; set; }
        public string ClassroomNumber { get; set; }
        public int Capacity { get; set; }
        public int Floor { get; set; }
        public bool HasProjector { get; set; }
        public bool IsLab { get; set; }

    }
}
