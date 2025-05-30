﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.Data.Model
{
    public enum Sessionstatus
    {
        Active,
        Inactive
    };

    public class Session
    {
        [Key]
        public int session_id { get; set; }

        [Required]
        public string user_token { get; set; }

        public DateTime last_login { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public Sessionstatus status { get; set; }

        //FK
        public int user_id { get; set; }
        public User User { get; set; }
    }
}
