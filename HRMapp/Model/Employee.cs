using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.Model
{
    public class Employee
    {
        [Key]
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Column(TypeName ="varchar(6)")]
        public string NIP { get; set; }

        [Required]
        [Column(TypeName = "varchar(16)")]
        public string NIK { get; set; }

        [Required]
        [Column(TypeName = "varchar(15)")]
        public string PhoneNumber { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public DateOnly Birthdate { get; set; }

        [Required]
        public string Birthplace { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string MaritalStatus { get; set; }

        [Required]
        public string EmployeeStatus { get; set; }
    }
}
