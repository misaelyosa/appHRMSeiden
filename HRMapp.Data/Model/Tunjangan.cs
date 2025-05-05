using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.Data.Model
{
    public class Tunjangan
    {
        [Key]
        [Required]
        public int tunjangan_id { get; set; }

        public string? tunjangan_name { get; set; }

        public int? amount { get; set; }

        //FK
        public int contract_id { get; set; }
        public Contract Contract { get; set; }
    }
}
