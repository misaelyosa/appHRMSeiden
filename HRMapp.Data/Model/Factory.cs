using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.Data.Model
{
    public class Factory
    {
        [Key]
        [Required]
        public int factory_id { get; set; }

        [Required]
        [MaxLength(30)]
        public string name { get; set; }

        [Required]
        public string address { get; set; }

        public int? personnel_capacity { get; set; }
    }
}
