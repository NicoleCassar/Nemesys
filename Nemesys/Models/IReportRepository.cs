using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.Models
{
    public interface IReportRepository
    {
        IEnumerable<Report> GetAllReports();
        Report GetReportsById(int reportId);
        void CreateReport(Report newReport);
        void UpdateReport(Report updatedReport);
        void IncrementUpvote(int reportId);
        void DecrementUpvote(int reportId);
        void UpdateStatus(int reportId, string status);
        void DeleteReport(int reportId);
    }
}
