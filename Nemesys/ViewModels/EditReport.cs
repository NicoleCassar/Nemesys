using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Nemesys.Models;

namespace Nemesys.ViewModels
{
    public class EditReport
    {
        public int ReportId { get; set; }
        public DateTime DateOfReport { get; set; }
        [Required(ErrorMessage = "Location of Spotted Hazard is required")]
        public string LocationOfHazard { get; set; }
        [Required(ErrorMessage = "Date & Time of Spotted Hazard is required")]
        public DateTime DateOfSpottedHazard { get; set; }
        [Required(ErrorMessage = "Type of Hazard is required")]
        public string TypeOfHazard { get; set; }
        [Required(ErrorMessage = "Hazard Description is required")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public IFormFile ImageToUpload { get; set; } // used when submitting a form
        public int Upvotes { get; set; }
        public string Status { get; set; }
    }
}
