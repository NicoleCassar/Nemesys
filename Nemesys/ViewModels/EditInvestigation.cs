using Nemesys.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.ViewModels
{
    public class EditInvestigation
    {
        public int InvestigationId { get; set; }
        public ApplicationUser Investigator { get; set; }
        public Report Report { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string Description { get; set; }
        public DateTime DateOfAction {get; set;}
        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; }
    }
}
