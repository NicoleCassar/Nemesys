using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Nemesys.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nemesys.Models
{
    public class Report
    {
        public int ReportId { get; set; }
        public ApplicationUser Reporter { get; set; }
        public DateTime DateOfReport { get; set; }
        public string LocationOfHazard { get; set; }
        public DateTime DateOfSpottedHazard { get; set; }
        public string TypeOfHazard { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int Upvotes { get; set; }
        public string Status { get; set; }
    }
}
