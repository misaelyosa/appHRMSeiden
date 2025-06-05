using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.Data.Model
{
    public class HolidayDTO
    {
        public string holiday_date { get; set; } = string.Empty;
        public string holiday_name { get; set; } = string.Empty;
        public bool is_national_holiday { get; set; }
    }
}
