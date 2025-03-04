using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.Data.Model
{
    public class Employee
    {
        [Key]
        [Required]
        public int employee_id { get; set; }

        [Required]
        [MaxLength(255)]
        public string name { get; set; }

        [Required]
        [Column(TypeName ="varchar(6)")]
        public string nip { get; set; }

        [Required]
        [Column(TypeName = "varchar(16)")]
        public string nik { get; set; }

        [Required]
        [Column(TypeName = "varchar(15)")]
        public string phone_number { get; set; }

        [Required]
        [MaxLength(20)]
        public string gender { get; set; }

        [Required]
        public DateOnly birthdate { get; set; }

        [Required]
        [MaxLength(50)]
        public string birthplace { get; set; }

        [Required]
        [MaxLength(255)]
        public string address { get; set; }

        [Required]
        [MaxLength(20)]
        public string marital_status { get; set; }

        [Required]
        [MaxLength(15)]
        public string employee_status { get; set; }
    }
}
