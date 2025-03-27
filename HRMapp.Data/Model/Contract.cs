using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.Data.Model
{
    public class Contract
    {
        [Key]
        [Required]
        public int contract_id { get; set; }

        [Required]
        [MaxLength(6)]
        public string contract_nip { get; set; }

        [Required]
        public DateOnly contract_date { get; set; }

        [Required]
        public DateOnly hire_date { get; set; }

        [Required]
        public DateOnly end_date { get; set; }

        [Required]
        public int contract_duration { get; set; }

        [Required]
        public int gaji_pokok { get; set; } = 2751000;

        public DateTime updated_at { get; set; }
        public DateTime created_at { get; set; }

        public string? author { get; set; }

        //FK
        public int employee_id { get; set; }
        public Employee Employee { get; set; }
    }
}
