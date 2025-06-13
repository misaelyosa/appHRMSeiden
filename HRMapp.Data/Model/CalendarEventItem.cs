using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.Data.Model
{
    public class CalendarEventItem
    {
        public string Title { get; set; } = string.Empty;
        public bool IsHoliday { get; set; }
    }
}
