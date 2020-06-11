using Nemesys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.ViewModels
{
    public class ReportListViewModel
    {
        public int TotalEntries { get; set; }
        public IEnumerable<Report> Reports { get; set; }
    }
}
