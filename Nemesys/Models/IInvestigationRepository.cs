using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.Models
{
    public interface IInvestigationRepository
    {
        public IEnumerable<Investigation> GetAllInvestigations();
        Investigation GetInvestigationsById(int investigationId);
        public void CreateInvestigation(Investigation newInvestigation);
        public void UpdateInvestigation(Investigation updatedInvestigation);
        public Investigation GetInvestigationByReportId(Report report);
        public Investigation GetReportByInvestigationId(int investigationId);
        public void DeleteInvestigation(Investigation investigation);
    }
}
