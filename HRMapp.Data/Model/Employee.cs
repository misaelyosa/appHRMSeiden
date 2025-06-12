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

        [MaxLength(60)]
        public string? email { get; set; }

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

        [MaxLength(20)]
        public string? marital_status { get; set; }

        [Required]
        [MaxLength(15)]
        public string employee_status { get; set; }

        public DateOnly? graduation_date { get; set; }

        [Required]
        public DateOnly hire_date { get; set; }

        public int yearly_cuti_left { get; set; }

        [MaxLength(255)]
        public string? skill { get; set; }

        [MaxLength(30)]
        public string? pic_path { get; set; }

        // Foreign Keys
        public int department_id { get; set; }
        public Department Department { get; set; }

        public int education_id { get; set; }
        public Education Education { get; set; }

        public int city_id { get; set; }
        public City City { get; set; }

        public int religion_id { get; set; }
        public Religion Religion { get; set; }

        public int job_id { get; set; }
        public Job Job { get; set; }

        public int factory_id { get; set; }
        public Factory Factory { get; set; }

        public ICollection<Course> Courses { get; set; }
        public ICollection<Contract> Contracts { get; set; }
        public ICollection<LogEmployee> LogEmployees { get; set; }
        public ICollection<Cuti> Cuti { get; set; }
    }
}
