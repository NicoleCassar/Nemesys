using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nemesys.Models
{
    public class Investigation
    {
       [Key]
        public int InvestigationId { get; set; }
        public Report Report { get; set; }
        public ApplicationUser Investigator { get; set; }
        public string Description { get; set; }
        public DateTime DateOfAction { get; set; }
    }
}
