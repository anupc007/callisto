using CsvHelper;
using Roster.Business;
using Roster.Business.Factory.Repository;
using Roster.Business.Factory.ViewModels;
using Roster.Web.App_Start;
using Roster.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Roster.Web.Controllers
{
    [CustomAuthorize("Company", "Timesheet")]
    public class TimesheetController : Controller
    {
        RosterRepository objRepo;
        public TimesheetController()
        {
            objRepo = new RosterRepository();


        }
        // GET: Timesheet
        [HttpGet]
        public ActionResult Timesheet()
        {
            List<TimesheetModel> model = new List<TimesheetModel>();
            // var Timesheetlst = TimesheetRespository.lstTimesheet("", "", 0);
            return View(model);
        }
        [HttpGet]
        public ActionResult ApprovedTimesheet()
        {
            List<TimesheetModel> model = new List<TimesheetModel>();
            // var Timesheetlst = TimesheetRespository.lstTimesheet("", "", 0);
            return View(model);
        }
        [HttpGet]
        public ActionResult _TimesheetList(string StartDate = "", string EndDate = "")
        {

            var Timesheetlst = TimesheetRespository.lstTimesheet(StartDate, EndDate, 0);
            return PartialView("_TimesheetList", Timesheetlst);
        }
        [HttpGet]
        public ActionResult _ApprovedTimesheetList(string StartDate = "", string EndDate = "")
        {

            var Timesheetlst = TimesheetRespository.LstGetApprovedTimesheet(StartDate, EndDate, 0);
            return PartialView("_ApprovedTimesheetList", Timesheetlst);
        }
        [HttpGet]
        public ActionResult _TimesheetDetailList(long EmployeeId = 0, long RosterId = 0)
        {

            var Timesheetlst = TimesheetRespository.lstTimesheetDtls(EmployeeId, RosterId);
            return PartialView("_TimesheetDetailList", Timesheetlst);
        }

        [HttpGet]
        public ActionResult _ApprovedTimesheetDetailList(long EmployeeId = 0, long RosterId = 0)
        {
            var Timesheetlst = TimesheetRespository.lstTimesheetDtls(EmployeeId, RosterId);
            return PartialView("_ApprovedTimesheetDetailList", Timesheetlst);
        }
        // GET: Timesheet/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Timesheet/Create
        public ActionResult Create()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Approved(long EmpId, long RosterId)
        {
            long id = TimesheetRespository.ApprovedTimesheet(EmpId, RosterId, Convert.ToInt64(UserCache.UserId));
            var Timesheetlst = TimesheetRespository.lstTimesheet("", "", 0);
            return PartialView("_TimesheetList", Timesheetlst);

        }

        // POST: Timesheet/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Timesheet/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Timesheet/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Timesheet/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Timesheet/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
