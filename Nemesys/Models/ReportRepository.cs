using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.Models
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<ReportRepository> _logger;

        public ReportRepository(AppDbContext appDbContext, ILogger<ReportRepository> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public IEnumerable<Report> GetAllReports()
        {
            try
            {
                return _appDbContext.Report;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public void CreateReport(Report newReport)
        {
            try
            {
                _appDbContext.Report.Add(newReport);
                _appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public Report GetReportsById(int reportId)
        {
            try
            {
                return _appDbContext.Report.Include(r => r.Reporter).FirstOrDefault(t => t.ReportId == reportId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public void UpdateReport(Report updatedReport)
        {
            try
            {
                var existingReport = _appDbContext.Report.SingleOrDefault(r => r.ReportId == updatedReport.ReportId);
                if (existingReport != null)
                {
                    existingReport.Reporter = updatedReport.Reporter;
                    existingReport.DateOfReport = updatedReport.DateOfReport;
                    existingReport.LocationOfHazard = updatedReport.LocationOfHazard;
                    existingReport.DateOfSpottedHazard = updatedReport.DateOfSpottedHazard;
                    existingReport.TypeOfHazard = updatedReport.TypeOfHazard;
                    existingReport.Description = updatedReport.Description;
                    existingReport.ImageUrl = updatedReport.ImageUrl;
                    existingReport.Upvotes = updatedReport.Upvotes;
                    existingReport.Status = updatedReport.Status;

                    _appDbContext.Entry(existingReport).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _appDbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public void IncrementUpvote(int reportId)
        {
            try
            {
                Report temp = _appDbContext.Report.Include(r => r.Reporter).FirstOrDefault(t => t.ReportId == reportId);
                temp.Upvotes++;
                _appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public void DecrementUpvote(int reportId)
        {
            try
            {
                Report temp = _appDbContext.Report.Include(r => r.Reporter).FirstOrDefault(t => t.ReportId == reportId);
                temp.Upvotes--;
                _appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public void UpdateStatus(int reportId, string status)
        {
            try
            {
                Report temp = _appDbContext.Report.Include(r => r.Reporter).FirstOrDefault(t => t.ReportId == reportId);
                temp.Status = status;
                _appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public void DeleteReport(int reportId)
        {
            try
            {
                Report report = GetReportsById(reportId);

                // Remove any constraints first
                Investigation investigation = _appDbContext.Investigation.Include(i => i.Investigator).FirstOrDefault(t => t.Report.ReportId == report.ReportId);
                _appDbContext.Investigation.Remove(investigation);
                _appDbContext.SaveChanges();

                _appDbContext.Report.Remove(report);
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
