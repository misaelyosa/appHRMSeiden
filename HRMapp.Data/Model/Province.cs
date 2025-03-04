using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.Data.Model
{
    public class Province
    {
        [Key]
        [Required]
        public int province_id { get; set; }

        [Required]
        [MaxLength(50)]
        public string province_name { get; set; }
    }
}
