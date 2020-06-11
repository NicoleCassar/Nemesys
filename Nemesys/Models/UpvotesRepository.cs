using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.Models
{
    public class UpvotesRepository : IUpvotesRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<UpvotesRepository> _logger;

        public UpvotesRepository(AppDbContext appDbContext, ILogger<UpvotesRepository> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }
        public IEnumerable<Upvotes> GetAllUpvotes()
        {
            try
            {
                return _appDbContext.Upvotes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public Upvotes GetUpvotesByReporterId(ApplicationUser reporter)
        {
            try
            {
                return _appDbContext.Upvotes.Include(u => u.Reporter).FirstOrDefault(v => v.Reporter.Id == reporter.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public Upvotes GetUpvotesByReportId(Report report)
        {
            try
            {
                return _appDbContext.Upvotes.Include(u => u.Reporter).FirstOrDefault(v => v.Report.ReportId == report.ReportId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public void UndoUpvote(Report report, ApplicationUser reporter)
        {
            try
            {
                //Upvotes tempId = _appDbContext.Upvotes.Include(u => u.UpvotesId).FirstOrDefault(v => v.Report.ReportId == report.ReportId && v.Reporter.Id == reporter.Id);
                var tempIds = _appDbContext.Upvotes.Include(u => u.Reporter);
                var tempId = tempIds.FirstOrDefault(v => v.Report.ReportId == report.ReportId && v.Reporter.Id == reporter.Id);
                _appDbContext.Upvotes.Remove(tempId);
                _appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public void AddUpvote(Upvotes newUpvote)
        {
            try
            {
                _appDbContext.Upvotes.Add(newUpvote);
                _appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public Upvotes UpvoteExists(Report report, ApplicationUser reporter)
        {
            try
            {
                var temp = _appDbContext.Upvotes.Include(u => u.Reporter);
                return temp.FirstOrDefault(v => v.Report.ReportId == report.ReportId && v.Reporter.Id == reporter.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        
    }
}
