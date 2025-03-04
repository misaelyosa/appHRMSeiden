using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.Data.Model
{
    public class Department
    {
        [Key]
        [Required]
        public int department_id { get; set; }

        [Required]
        [MaxLength(255)]
        public string name { get; set; }

        //FK
        public int factory_id { get; set; }
        

    }
}
