using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.Models
{
    public interface IUpvotesRepository
    {
        IEnumerable<Upvotes> GetAllUpvotes();
        Upvotes GetUpvotesByReportId(Report report);
        Upvotes GetUpvotesByReporterId(ApplicationUser reporter);
        void UndoUpvote(Report report, ApplicationUser reporter);
        void AddUpvote(Upvotes upvote);
        Upvotes UpvoteExists(Report report, ApplicationUser reporter);
    }
}
