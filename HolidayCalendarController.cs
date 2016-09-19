using CsvHelper;
using Roster.Business.Factory.Repository;
using Roster.Business.Factory.ViewModels;
using Roster.Web.App_Start;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
namespace Roster.Web.Controllers
{
    [CustomAuthorize("Administration", "Company")]
    public class HolidayCalendarController : Controller
    {
        // GET: HolidayCalendar
        public ActionResult Index()
        {

            return View();
        }
        [HttpGet]
        public ActionResult _HolidayCalendarList(string StartDate = "", string EndDate = "")
        {
            var result = HolidayCalendarRepository.LstHolidayCalendar(StartDate, EndDate);
            return PartialView("_HolidayCalendarList", result);
        }

        // GET: HolidayCalendar/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HolidayCalendar/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HolidayCalendar/Create
        [HttpPost]
        public ActionResult Create(HolidayCalenderModel model)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    HolidayCalendarRepository objholiday = new HolidayCalendarRepository();
                    objholiday.AddUpdateHoliday(model);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("error", "Invalid details");
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: HolidayCalendar/Edit/5
        public ActionResult Edit(int id)
        {
            HolidayCalenderModel model = HolidayCalendarRepository.GetHolidayById(id);
            return View(model);
        }

        // POST: HolidayCalendar/Edit/5
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

        // GET: HolidayCalendar/Delete/5
        [HttpGet]
        public ActionResult Delete(int id)
        {
            HolidayCalendarRepository.DeleteHolidayById(id);
            return RedirectToAction("Index");
        }
    }
}
