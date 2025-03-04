using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.Data.Model
{
    public class Religion
    {
        [Key]
        [Required]
        public int religion_id { get; set; }

        [Required]
        public string religion_name { get; set; }
    }
}
