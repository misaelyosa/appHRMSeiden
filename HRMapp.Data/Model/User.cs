using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.Data.Model
{
    public class User
    {
        [Key]
        public int user_id { get; set; }

        [Required]
        [MaxLength(50)]
        public string username { get; set; }

        [Required]
        public string password_hash { get; set; }

        public string? forgot_pass_token { get; set; }

        [Required]
        [MaxLength(20)]
        public string authority { get; set; }
    }
}
