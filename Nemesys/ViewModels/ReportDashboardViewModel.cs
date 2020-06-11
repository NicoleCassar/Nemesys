using Nemesys.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.ViewModels
{
    public class ReportDashboardViewModel
    {
        [DisplayName("Total Reports")]
        public int TotalEntries { get; set; }
        [DisplayName("Registered Users")]
        public int TotalRegisteredUsers { get; set; }
    }
}
