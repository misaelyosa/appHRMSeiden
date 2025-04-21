using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.Data.Model
{
    public class Course
    {
        [Key]
        [Required]
        public int course_id { get; set; }

        [Required]
        public string course_name { get; set; }

        [Required]
        public DateOnly course_graduation_date { get; set; }

        //FK
        public int employee_id { get; set; }
        public Employee Employee { get; set; }
    }
}
