using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.Models
{
    public class Upvotes
    {
        [Key]
        public int UpvoteId { get; set; }
        public ApplicationUser Reporter { get; set; }
        public Report Report { get; set; }
    }
}
