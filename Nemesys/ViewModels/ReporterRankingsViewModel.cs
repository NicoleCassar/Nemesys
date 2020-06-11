using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nemesys.Models;

namespace Nemesys.ViewModels
{
    public class ReporterRankingsViewModel
    {
        public int Ranking { get; set; }
        public IEnumerable<ApplicationUser> Reporter { get; set; }
    }
}
