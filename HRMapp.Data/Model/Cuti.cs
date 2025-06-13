using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.Data.Model
{
    public class Cuti
    {
        [Key]
        [Required]
        public int cuti_id { get; set; }
        [Required]
        public DateOnly cuti_start_date { get; set; }
        public DateOnly? cuti_end_date { get; set; }
        [Required]
        public int cuti_day_count { get; set; }
        public string? reason { get; set; }

        public string? created_by { get; set; }
        public DateTime created_at { get; set; }
        public string? updated_by { get; set; }
        public DateTime updated_at { get; set; }
        //FK
        public int employee_id { get; set; }
        public Employee Employee { get; set; }
    }
}
