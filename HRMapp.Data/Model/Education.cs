using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.Data.Model
{
    public class Education
    {
        [Key]
        [Required]
        public int education_id { get; set; }

        [Required]
        [MaxLength(10)]
        public string education_type { get; set; }

        [MaxLength(30)]
        public string? major { get; set; }
    }
}
