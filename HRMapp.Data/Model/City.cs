using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.Data.Model
{
    public class City
    {
        [Key]
        [Required]
        public int city_id { get; set; }

        [Required]
        [MaxLength(30)]
        public string city_name { get; set; }

    
    }
}
