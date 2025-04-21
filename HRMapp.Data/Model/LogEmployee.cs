using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.Data.Model
{
    public class LogEmployee
    {
        [Key]
        [Required]
        public int log_id { get; set; }

        [Required]
        public string field_name { get; set; }
        [Required]
        public string old_value { get; set; }
        [Required]
        public string new_value { get; set; }

        [Required]
        public DateTime updated_at { get; set; }

        [Required]
        public DateTime deleted_at { get; set; }

        [Required]
        public string updated_by { get; set; }

        //FK
        public int employee_id { get; set; }
        public Employee Employee { get; set; }
    }
}
