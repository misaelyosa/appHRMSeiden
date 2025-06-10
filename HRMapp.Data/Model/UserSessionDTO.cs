using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.Data.Model
{
    public class UserSessionDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Authority { get; set; }
        public int SessionId { get; set; }
        public DateTime LastLogin { get; set; }
        public string Status { get; set; }
        
    }
}
