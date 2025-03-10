using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.Data.Model
{
    public class Session
    {
        [Key]
        [Required]
        public int session_id { get; set; }

        [Required]
        [MaxLength(50)]
        public string username { get; set; }

        [Required]
        [MaxLength(50)]
        public string password { get; set; }

        public string? forgot_pass_token { get; set; }

        [Required]
        [MaxLength(20)]
        public string authority { get; set; }

        public DateTime last_login { get; set; }
    }
}
