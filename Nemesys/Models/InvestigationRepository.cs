using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Nemesys.Models
{
    public class InvestigationRepository : IInvestigationRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<InvestigationRepository> _logger;

        public InvestigationRepository(AppDbContext appDbContext, ILogger<InvestigationRepository> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public IEnumerable<Investigation> GetAllInvestigations()
        {
            try
            {
                return _appDbContext.Investigation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public void CreateInvestigation(Investigation newInvestigation)
        {
            try
            {
                _appDbContext.Investigation.Add(newInvestigation);
                _appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public void UpdateInvestigation(Investigation updatedInvestigation)
        {
            try
            {
                var existingInvestigation = _appDbContext.Investigation.SingleOrDefault(inv => inv.InvestigationId == updatedInvestigation.InvestigationId);
                if (existingInvestigation != null)
                {
                    existingInvestigation.Investigator = updatedInvestigation.Investigator;
                    existingInvestigation.Description = updatedInvestigation.Description;
                    existingInvestigation.DateOfAction = updatedInvestigation.DateOfAction;

                    _appDbContext.Entry(existingInvestigation).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _appDbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public Investigation GetInvestigationByReportId(Report report)
        {
            try
            {
                return _appDbContext.Investigation.Include(i => i.Investigator).FirstOrDefault(t => t.Report.ReportId == report.ReportId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public Investigation GetInvestigationsById(int investigationId)
        {
            try
            {
                return _appDbContext.Investigation.Include(i => i.Investigator).FirstOrDefault(t => t.InvestigationId == investigationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public Investigation GetReportByInvestigationId(int investigationId)
        {
            try
            {
                return _appDbContext.Investigation.Include(i => i.Report).FirstOrDefault(t => t.InvestigationId == investigationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public void DeleteInvestigation(Investigation investigation)
        {
            try
            {
                _appDbContext.Investigation.Remove(investigation);
                _appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
