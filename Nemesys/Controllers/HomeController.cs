using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nemesys.ViewModels;
using Nemesys.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nemesys.Controllers
{
    public class HomeController : Controller
    {
        private readonly IReportRepository _reportRepository;
        private readonly IInvestigationRepository _investigationRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUpvotesRepository _upvotesRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<HomeController> _logger;

        // dropdown menu info
        private readonly List<String> hazardtypes = new List<String>()
        {
            "Unsafe Act",
            "Unsafe Condition",
            "Unsafe Equipment",
            "Unsafe Structure"
        };

        private readonly List<String> statusoptions = new List<String>()
        {
            "Closed",
            "Being Investigated",
            "No Action Required"
        };

        public HomeController(IReportRepository reportRepository, IInvestigationRepository investigationRepository, UserManager<ApplicationUser> userManager, IUpvotesRepository upvotesRepository, IUserRepository userRepository, ILogger<HomeController> logger)
        {
            _reportRepository = reportRepository;
            _investigationRepository = investigationRepository;
            _userManager = userManager;
            _upvotesRepository = upvotesRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpGet]
        [ResponseCache(Duration = 20, Location = ResponseCacheLocation.Client)]
        public IActionResult Index()
        {
            try
            {
                _logger.LogInformation("Information");
                _logger.LogWarning("Warning");
                _logger.LogError("Error");
                _logger.LogCritical("Crtitical");

                ViewBag.Title = "Report Index View";
                var model = new ReportListViewModel();
                model.Reports = _reportRepository.GetAllReports().OrderByDescending(r => r.DateOfReport);
                model.TotalEntries = model.Reports.Count();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error");
            }
        }

        [HttpGet]
        [ResponseCache(Duration = 20, Location = ResponseCacheLocation.Client)]
        public IActionResult ReporterRankings()
        {
            try
            {
                var model = new ReporterRankingsViewModel();
                model.Reporter = _userRepository.GetReportersByNumberOfReports();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult ReportDetails(int id)
        {
            try
            {
                var report = _reportRepository.GetReportsById(id);
                if (report == null)
                    return NotFound();
                else
                    return View(report);
            }
            catch (Exception ex)
            {
                _logger.LogError("Details action: " + ex.Message);
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult InvestigationDetails(int id)
        {
            try
            {
                var report = _reportRepository.GetReportsById(id);
                var investigation = _investigationRepository.GetInvestigationByReportId(report);
                if (investigation == null)
                    return NotFound();
                else
                    return View(investigation);
            }
            catch (Exception ex)
            {
                _logger.LogError("Details action: " + ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upvotes(int id)
        {
            Report report = _reportRepository.GetReportsById(id);
            Upvotes temp = _upvotesRepository.UpvoteExists(report, await _userManager.GetUserAsync(User));
            
            // if upvote doesn't exist already
            if(temp==null)
            {
                Upvotes newUpvote = new Upvotes()
                {
                    Reporter = await _userManager.GetUserAsync(User),
                    Report = report
                };
                _upvotesRepository.AddUpvote(newUpvote);
                _reportRepository.IncrementUpvote(id);
            }
            else // upvote already exists, therefore remove the upvote
            {
                _upvotesRepository.UndoUpvote(report, await _userManager.GetUserAsync(User));
                _reportRepository.DecrementUpvote(id);
            }

            return RedirectToAction("ReportDetails", new {id});
        }

        [HttpGet]
        [Authorize]
        public IActionResult CreateReport()
        {
            ViewBag.typesofhazard = new SelectList(hazardtypes);
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReport([Bind("LocationOfHazard", "DateOfSpottedHazard", "TypeOfHazard", "Description", "ImageToUpload")] EditReport newReport)
        {
            ViewBag.typesofhazard = new SelectList(hazardtypes);
            try
            {
                if (ModelState.IsValid)
                {
                    string fileName = "";
                    if (newReport.ImageToUpload != null)
                    {
                        var extension = "." + newReport.ImageToUpload.FileName.Split('.')[newReport.ImageToUpload.FileName.Split('.').Length - 1];
                        fileName = Guid.NewGuid().ToString() + extension;
                        var path = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\reports\\" + fileName;
                        using (var bits = new FileStream(path, FileMode.Create))
                        {
                            newReport.ImageToUpload.CopyTo(bits);
                        }
                    }
                    else
                    {
                        // Default image that's displayed if no image was uploaded
                        fileName = "default.png";
                    }

                    Report report = new Report()
                    {
                        Reporter = await _userManager.GetUserAsync(User),
                        DateOfReport = DateTime.UtcNow,
                        LocationOfHazard = newReport.LocationOfHazard,
                        DateOfSpottedHazard = newReport.DateOfSpottedHazard,
                        TypeOfHazard = newReport.TypeOfHazard,
                        Description = newReport.Description,
                        ImageUrl = "/images/reports/" + fileName,
                        Upvotes = 0,
                        Status = "Open"
                    };

                    report.Reporter.NumberOfReports++; // increment number of reports

                    _logger.LogInformation("User " + User.Identity.Name + " created a report");

                    _reportRepository.CreateReport(report);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(newReport);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditReport(int id)
        {
            ViewBag.typesofhazard = new SelectList(hazardtypes);
            try
            {
                var existingReport = _reportRepository.GetReportsById(id);
                if (existingReport != null)
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (existingReport.Reporter.IdNum == currentUser.IdNum)
                    {
                        EditReport model = new EditReport()
                        {
                            ReportId = existingReport.ReportId,
                            LocationOfHazard = existingReport.LocationOfHazard,
                            DateOfSpottedHazard = existingReport.DateOfSpottedHazard,
                            TypeOfHazard = existingReport.TypeOfHazard,
                            Description = existingReport.Description,
                            Status = existingReport.Status,
                            ImageUrl = existingReport.ImageUrl
                        };

                        return View(model);
                    }
                    else
                        return Unauthorized();
                }
                else
                    return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error");
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReport(int id, [Bind("ReportId, LocationOfHazard, DateOfSpottedHazard, TypeOfHazard, Description, ImageUrl, ImageToUpload")] EditReport updatedReport)
        {
            try
            {
                //1. Check for incoming data integrity
                if (id != updatedReport.ReportId)
                {
                    return NotFound();
                }

                //2. Check if the user has access to this report
                var existingReport = _reportRepository.GetReportsById(id);
                if (existingReport != null)
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (existingReport.Reporter.IdNum == currentUser.IdNum)
                    {
                        //2. Validate model
                        if (ModelState.IsValid)
                        {
                            if (updatedReport.ImageToUpload != null)
                            {
                                string fileName = "";
                                //At this point you should check size, extension etc...
                                //.....
                                //Then persist using a new name for consistency (e.g. new Guid)
                                var extension = "." + updatedReport.ImageToUpload.FileName.Split('.')[updatedReport.ImageToUpload.FileName.Split('.').Length - 1];
                                fileName = Guid.NewGuid().ToString() + extension;
                                var path = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\reports\\" + fileName;
                                using (var bits = new FileStream(path, FileMode.Create))
                                {
                                    updatedReport.ImageToUpload.CopyTo(bits);
                                }

                                updatedReport.ImageUrl = "/images/reposts/" + fileName;
                            }

                            Report updated_Report = new Report()
                            {
                                ReportId = updatedReport.ReportId,
                                Reporter = await _userManager.GetUserAsync(User),
                                LocationOfHazard = updatedReport.LocationOfHazard,
                                DateOfSpottedHazard = updatedReport.DateOfSpottedHazard,
                                TypeOfHazard = updatedReport.TypeOfHazard,
                                Description = updatedReport.Description,
                                ImageUrl = updatedReport.ImageUrl,
                                Status = updatedReport.Status
                            };

                            _reportRepository.UpdateReport(updated_Report);
                            return RedirectToAction("Index");
                        }
                        else
                            return View(updatedReport);
                    }
                    return Unauthorized();
                }
                else
                    return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Investigator")]
        public IActionResult CreateInvestigation()
        {
            ViewBag.status = new SelectList(statusoptions);
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Investigator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInvestigation(int id, [Bind("Report", "Description", "Status")] EditInvestigation newInvestigation)
        {
            ViewBag.status = new SelectList(statusoptions);
            try
            {
                if (ModelState.IsValid)
                {
                    Report tempReport = _reportRepository.GetReportsById(id);

                    Investigation investigation = new Investigation()
                    {
                        Investigator = await _userManager.GetUserAsync(User),
                        Report = tempReport,
                        Description = newInvestigation.Description,
                        DateOfAction = DateTime.UtcNow
                    };

                    // Updating status of report
                    _reportRepository.UpdateStatus(id, newInvestigation.Status);

                    _investigationRepository.CreateInvestigation(investigation);
                    _logger.LogInformation("User " + User.Identity.Name + " created an investigation");
                    Mailer(investigation.Report.Reporter);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(newInvestigation);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error");
            }
        }

        [Authorize(Roles = "Investigator")]
        [HttpGet]
        public async Task<IActionResult> EditInvestigation(int id)
        {
            ViewBag.status = new SelectList(statusoptions);
            try
            {
                Investigation existingInvestigation = _investigationRepository.GetReportByInvestigationId(id);

                if (existingInvestigation != null)
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (existingInvestigation.Investigator.IdNum == currentUser.IdNum)
                    {
                        EditInvestigation model = new EditInvestigation()
                        {
                            InvestigationId = existingInvestigation.InvestigationId,
                            Investigator = currentUser,
                            Report = existingInvestigation.Report,
                            Description = existingInvestigation.Description,
                            DateOfAction = DateTime.UtcNow,
                            Status = existingInvestigation.Report.Status
                        };

                        return View(model);
                    }
                    else
                        return Unauthorized();
                }
                else
                    return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error");
            }
        }

        [Authorize(Roles = "Investigator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInvestigation(int id, [Bind("InvestigationId", "Report", "Description", "DateOfAction", "Status")] EditInvestigation updatedInvestigation)
        {
            ViewBag.status = new SelectList(statusoptions);
            try
            {
                //1. Check for incoming data integrity
                if (id != updatedInvestigation.InvestigationId)
                {
                    return NotFound();
                }

                //2. Check if the user has access to this investigation
                Investigation existingInvestigation = _investigationRepository.GetReportByInvestigationId(id);

                if (existingInvestigation != null)
                {
                    var currentUser = await _userManager.GetUserAsync(User);

                    if (existingInvestigation.Investigator.IdNum == currentUser.IdNum)
                    {
                        //2. Validate model
                        if (ModelState.IsValid)
                        {
                            Investigation updated_Investigation = new Investigation()
                            {
                                InvestigationId = updatedInvestigation.InvestigationId,
                                Investigator = await _userManager.GetUserAsync(User),
                                Report = existingInvestigation.Report,
                                Description = updatedInvestigation.Description,
                                DateOfAction = DateTime.UtcNow
                            };

                            // Updating status of report
                            _reportRepository.UpdateStatus(existingInvestigation.Report.ReportId, updatedInvestigation.Status);

                            _investigationRepository.UpdateInvestigation(updated_Investigation);
                            return RedirectToAction("Index");
                        }
                        else
                            return View(updatedInvestigation);
                    }
                    return Unauthorized();
                }
                else
                    return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error");
            }
        }

        public IActionResult DeleteReport(int id)
        {
            Report tempReport = _reportRepository.GetReportsById(id);
            tempReport.Reporter.NumberOfReports--;
            _reportRepository.DeleteReport(id);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Investigator")]
        public IActionResult DeleteInvestigation(int id)
        {
            var investigation = _investigationRepository.GetInvestigationsById(id);
            _investigationRepository.DeleteInvestigation(investigation);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Investigator")]
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            try

            {
                ViewBag.Title = "Nemesys Dashboard";
                var model = new ReportDashboardViewModel();
                model.TotalRegisteredUsers = _userManager.Users.Count();
                model.TotalEntries = _reportRepository.GetAllReports().Count();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View("Error");
            }
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Mailer(ApplicationUser user)
        {
            GMailer.GmailUsername = "nemesys2020@gmail.com"; // Set email (username)
            GMailer.GmailPassword = "NemesysMAILER!!"; // Set password

            GMailer mailer = new GMailer(); // create new GMailer object
            mailer.ToEmail = user.Email; // Set target address

            string body = "Dear " + user.Name + " " +user.Surname + ",<br/><br/>Please note that Investigation updates have been made to one of your Reports! <br/><br/><br/>Regards,<br/>Nemesys";
        
            mailer.Subject = "Nemesys: Report Update!"; //Set the subject for the email
            mailer.Body = body;
            mailer.IsHtml = true; 
            mailer.Send(); // Send the email

            return RedirectToAction("Index"); 
        }

        /*
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        */
    }
}
