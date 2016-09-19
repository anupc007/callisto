using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Roster.Business.Factory;
using Roster.Business.Factory.Repository;
using Roster.Business.Factory.ViewModels;
using CsvHelper;
using Roster.Web.App_Start;
using Roster.Business;


namespace Roster.Web.Controllers
{

    public class HomeController : Controller
    {
        #region Dashboard
        /// <summary>
        /// HomeController constractor
        /// </summary>
        RosterRepository rosterRepo;
        public HomeController()
        {
            rosterRepo = new RosterRepository();
        }
        /// <summary>
        /// Dashboard module 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.ManagerTeamData = EmployeeRepository.GetManagerTeam(true);
            ViewBag.ManagerWeeklyTeamData = EmployeeRepository.GetManagerTeam(false);

            ViewBag.DailyRosterSnapshot = EmployeeRepository.GetDailyRosterSnapshot(true, Convert.ToInt32(UserCache.UserId), Convert.ToInt64(UserCache.CompanyId));
            ViewBag.WeeklyRosterSnapshot = EmployeeRepository.GetDailyRosterSnapshot(false, Convert.ToInt32(UserCache.UserId), Convert.ToInt64(UserCache.CompanyId));
            
            //if (UserCache.UserParmission.ToLower() == "company")
            //{
            //    ViewBag.CoverPage = LoginRepository.GetCompanyCoverPage(Convert.ToInt32(UserCache.CompanyId));

            //}
            //else
                ViewBag.CoverPage = LoginRepository.GetUserDashbaordCoverPage(Convert.ToInt32(UserCache.UserId));

            ViewBag.DashboardSummary = EmployeeRepository.GetDashboardSummriesTime(true);
            ViewBag.WeeklyDashboardSummary = EmployeeRepository.GetDashboardSummriesTime(false);
            
            return View();
        }
        /// <summary>
        /// Dashboard module 
        /// </summary>
        /// <returns></returns>
        public ActionResult WeeklyDashboard()
        {
            ViewBag.ManagerTeamData = EmployeeRepository.GetManagerTeam(false);
            ViewBag.DailyRosterSnapshot = EmployeeRepository.GetDailyRosterSnapshot(false, Convert.ToInt32(UserCache.UserId), Convert.ToInt64(UserCache.CompanyId));
            ViewBag.CoverPage = LoginRepository.GetCompanyCoverPage(Convert.ToInt32(UserCache.CompanyId));
            ViewBag.DashboardSummary = EmployeeRepository.GetDashboardSummriesTime(false);
            return View();
        }
        /// <summary>
        /// using for upload company cover image
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadCoverImage()
        {
            string ext = string.Empty;
            String UUID = Guid.NewGuid().ToString();
            string path = string.Empty;
            var attachedFile = System.Web.HttpContext.Current.Request.Files["ProfilePic"];
            var FileP = System.Web.HttpContext.Current.Request.Form[0];
            var companyName = System.Web.HttpContext.Current.Request.Form[1];
            if (attachedFile != null && attachedFile.ContentLength > 0)
            {
                ext = Path.GetExtension(attachedFile.FileName);
                path = System.IO.Path.Combine(Server.MapPath("~/Upload/"), UUID + ext);
                attachedFile.SaveAs(path);
                path = UUID + ext;
            }
            else
            {
                path = FileP.Split('/')[3];
            }

            DashboardCover objCover = new DashboardCover();
            objCover.PorfilePicture = path;
            objCover.CompanyId = Convert.ToInt64(UserCache.CompanyId);
            if (!string.IsNullOrEmpty(companyName))
            {
                objCover.CompanyName = companyName;
                UserCache.UserName = companyName;
            }
            var result = LoginRepository.UpLoadCoverImage(objCover);
            if (result)
            {

                return Json(path, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

    }
}