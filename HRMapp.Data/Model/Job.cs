using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.Data.Model
{
    public class Job
    {
        [Key]
        [Required]
        public int job_id { get; set; }

        [Required]
        public string job_name { get; set; }

        //FK
        public int department_id { get; set; }
        public Department Department { get; set; }

    }
}
