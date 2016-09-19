using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Roster.Business.Factory.ViewModels;
using System.Text;
using Roster.Web.App_Start;


namespace Roster.Web.Controllers
{
    [CustomAuthorize("Company", "Event")]
    public class EventTypesController : Controller
    {
        // GET: EventTypes
        [HttpGet]
        public ActionResult CreateEventType()
        {
            return View();
        }
        [HttpGet]
        public ActionResult _CreateEventType()
        {
            List<CheckBoxList> checkBoxList = new List<CheckBoxList>();
            checkBoxList.Add(new CheckBoxList() { Key = "#d96666", Value = 1 });
            checkBoxList.Add(new CheckBoxList() { Key = "#e67399", Value = 2 });
            checkBoxList.Add(new CheckBoxList() { Key = "#b373b3", Value = 3 });
            checkBoxList.Add(new CheckBoxList() { Key = "#8c66d9", Value = 4 });
            checkBoxList.Add(new CheckBoxList() { Key = "#668cb3", Value = 5 });
            checkBoxList.Add(new CheckBoxList() { Key = "#668cd9", Value = 6 });
            checkBoxList.Add(new CheckBoxList() { Key = "#59bfb3", Value = 7 });
            checkBoxList.Add(new CheckBoxList() { Key = "#65ad89", Value = 8 });
            checkBoxList.Add(new CheckBoxList() { Key = "#4cb052", Value = 9 });
            checkBoxList.Add(new CheckBoxList() { Key = "#8cbf40", Value = 10 });
            checkBoxList.Add(new CheckBoxList() { Key = "#bfbf4d", Value = 11 });
            checkBoxList.Add(new CheckBoxList() { Key = "#e0c240", Value = 12 });
            checkBoxList.Add(new CheckBoxList() { Key = "#f2a640", Value = 13 });
            checkBoxList.Add(new CheckBoxList() { Key = "#e6804d", Value = 14 });
            checkBoxList.Add(new CheckBoxList() { Key = "#be9494", Value = 15 });
            checkBoxList.Add(new CheckBoxList() { Key = "#a992a9", Value = 16 });
            checkBoxList.Add(new CheckBoxList() { Key = "#8997a5", Value = 17 });
            checkBoxList.Add(new CheckBoxList() { Key = "#94a2be", Value = 18 });
            checkBoxList.Add(new CheckBoxList() { Key = "#85aaa5", Value = 19 });
            checkBoxList.Add(new CheckBoxList() { Key = "#a7a77d", Value = 20 });
            checkBoxList.Add(new CheckBoxList() { Key = "#c4a883", Value = 21 });
            checkBoxList.Add(new CheckBoxList() { Key = "#c7561e", Value = 22 });
            checkBoxList.Add(new CheckBoxList() { Key = "#b5515d", Value = 23 });
            checkBoxList.Add(new CheckBoxList() { Key = "#c244ab", Value = 24 });
            checkBoxList.Add(new CheckBoxList() { Key = "#603f99", Value = 25 });
            checkBoxList.Add(new CheckBoxList() { Key = "#536ca6", Value = 26 });
            checkBoxList.Add(new CheckBoxList() { Key = "#3640ad", Value = 27 });
            checkBoxList.Add(new CheckBoxList() { Key = "#3c995b", Value = 28 });
            checkBoxList.Add(new CheckBoxList() { Key = "#5ca632", Value = 29 });
            checkBoxList.Add(new CheckBoxList() { Key = "#7ec225", Value = 30 });
            checkBoxList.Add(new CheckBoxList() { Key = "#a7b828", Value = 31 });
            checkBoxList.Add(new CheckBoxList() { Key = "#cf9911", Value = 32 });
            checkBoxList.Add(new CheckBoxList() { Key = "#d47f1e", Value = 33 });
            checkBoxList.Add(new CheckBoxList() { Key = "#b56414", Value = 34 });
            checkBoxList.Add(new CheckBoxList() { Key = "#914d14", Value = 35 });
            checkBoxList.Add(new CheckBoxList() { Key = "#ab2671", Value = 36 });
            checkBoxList.Add(new CheckBoxList() { Key = "#9643a5", Value = 37 });
            checkBoxList.Add(new CheckBoxList() { Key = "#4585a3", Value = 38 });
            checkBoxList.Add(new CheckBoxList() { Key = "#737373", Value = 39 });
            checkBoxList.Add(new CheckBoxList() { Key = "#41a587", Value = 40 });
            checkBoxList.Add(new CheckBoxList() { Key = "#d1bc36", Value = 41 });
            checkBoxList.Add(new CheckBoxList() { Key = "#ad2d2d", Value = 42 });
            checkBoxList.Add(new CheckBoxList() { Key = "#e37c23", Value = 43 });
            checkBoxList.Add(new CheckBoxList() { Key = "#08766d", Value = 44 });
            checkBoxList.Add(new CheckBoxList() { Key = "#78002e", Value = 45 });
            checkBoxList.Add(new CheckBoxList() { Key = "#e6be19", Value = 46 });
            checkBoxList.Add(new CheckBoxList() { Key = "#73b532", Value = 47 });
            checkBoxList.Add(new CheckBoxList() { Key = "#d53f20", Value = 48 });
            checkBoxList.Add(new CheckBoxList() { Key = "#00adef", Value = 49 });
            checkBoxList.Add(new CheckBoxList() { Key = "#000000", Value = 50 });
            checkBoxList.Add(new CheckBoxList() { Key = "#000000", Value = 51 });
            checkBoxList.Add(new CheckBoxList() { Key = "#000000", Value = 52 });
            checkBoxList.Add(new CheckBoxList() { Key = "#000000", Value = 53 });
            checkBoxList.Add(new CheckBoxList() { Key = "#000000", Value = 54 });
            checkBoxList.Add(new CheckBoxList() { Key = "#000000", Value = 55 });
            checkBoxList.Add(new CheckBoxList() { Key = "#000000", Value = 56 });
            checkBoxList.Add(new CheckBoxList() { Key = "#660066", Value = 56 });
            ViewBag.EventTypelst = checkBoxList;
            return PartialView("_CreateEventType");
        }
        public class CheckBoxList
        {
            public int Value { get; set; }
            public string Key { get; set; }
        }

        // GET: EventTypes/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        [HttpGet]
        public ActionResult _EventTypeList()
        {
            var EventTypelst = Roster.Business.Factory.Repository.EventTypeRepository.GetEventTypeList();
            return PartialView("_EventTypeList", EventTypelst);
        }
        // GET: EventTypes/Create
        [HttpPost]
        public ActionResult SaveEventType(string Name, string ColorCode)
        {
            EventTypeModel request = new EventTypeModel();
            request.Name = Name;
            request.ColorCode = ColorCode;
            Roster.Business.Factory.Repository.EventTypeRepository objsave = new Business.Factory.Repository.EventTypeRepository();
            objsave.AddUpdateRoster(request);
            List<CheckBoxList> checkBoxList = new List<CheckBoxList>();
            checkBoxList.Add(new CheckBoxList() { Key = "#d96666", Value = 1 });
            checkBoxList.Add(new CheckBoxList() { Key = "#e67399", Value = 2 });
            checkBoxList.Add(new CheckBoxList() { Key = "#b373b3", Value = 3 });
            checkBoxList.Add(new CheckBoxList() { Key = "#8c66d9", Value = 4 });
            checkBoxList.Add(new CheckBoxList() { Key = "#668cb3", Value = 5 });
            checkBoxList.Add(new CheckBoxList() { Key = "#668cd9", Value = 6 });
            checkBoxList.Add(new CheckBoxList() { Key = "#59bfb3", Value = 7 });
            checkBoxList.Add(new CheckBoxList() { Key = "#65ad89", Value = 8 });
            checkBoxList.Add(new CheckBoxList() { Key = "#4cb052", Value = 9 });
            checkBoxList.Add(new CheckBoxList() { Key = "#8cbf40", Value = 10 });
            checkBoxList.Add(new CheckBoxList() { Key = "#bfbf4d", Value = 11 });
            checkBoxList.Add(new CheckBoxList() { Key = "#e0c240", Value = 12 });
            checkBoxList.Add(new CheckBoxList() { Key = "#f2a640", Value = 13 });
            checkBoxList.Add(new CheckBoxList() { Key = "#e6804d", Value = 14 });
            checkBoxList.Add(new CheckBoxList() { Key = "#be9494", Value = 15 });
            checkBoxList.Add(new CheckBoxList() { Key = "#a992a9", Value = 16 });
            checkBoxList.Add(new CheckBoxList() { Key = "#8997a5", Value = 17 });
            checkBoxList.Add(new CheckBoxList() { Key = "#94a2be", Value = 18 });
            checkBoxList.Add(new CheckBoxList() { Key = "#85aaa5", Value = 19 });
            checkBoxList.Add(new CheckBoxList() { Key = "#a7a77d", Value = 20 });
            checkBoxList.Add(new CheckBoxList() { Key = "#c4a883", Value = 21 });
            checkBoxList.Add(new CheckBoxList() { Key = "#c7561e", Value = 22 });
            checkBoxList.Add(new CheckBoxList() { Key = "#b5515d", Value = 23 });
            checkBoxList.Add(new CheckBoxList() { Key = "#c244ab", Value = 24 });
            checkBoxList.Add(new CheckBoxList() { Key = "#603f99", Value = 25 });
            checkBoxList.Add(new CheckBoxList() { Key = "#536ca6", Value = 26 });
            checkBoxList.Add(new CheckBoxList() { Key = "#3640ad", Value = 27 });
            checkBoxList.Add(new CheckBoxList() { Key = "#3c995b", Value = 28 });
            checkBoxList.Add(new CheckBoxList() { Key = "#5ca632", Value = 29 });
            checkBoxList.Add(new CheckBoxList() { Key = "#7ec225", Value = 30 });
            checkBoxList.Add(new CheckBoxList() { Key = "#a7b828", Value = 31 });
            checkBoxList.Add(new CheckBoxList() { Key = "#cf9911", Value = 32 });
            checkBoxList.Add(new CheckBoxList() { Key = "#d47f1e", Value = 33 });
            checkBoxList.Add(new CheckBoxList() { Key = "#b56414", Value = 34 });
            checkBoxList.Add(new CheckBoxList() { Key = "#914d14", Value = 35 });
            checkBoxList.Add(new CheckBoxList() { Key = "#ab2671", Value = 36 });
            checkBoxList.Add(new CheckBoxList() { Key = "#9643a5", Value = 37 });
            checkBoxList.Add(new CheckBoxList() { Key = "#4585a3", Value = 38 });
            checkBoxList.Add(new CheckBoxList() { Key = "#737373", Value = 39 });
            checkBoxList.Add(new CheckBoxList() { Key = "#41a587", Value = 40 });
            checkBoxList.Add(new CheckBoxList() { Key = "#d1bc36", Value = 41 });
            checkBoxList.Add(new CheckBoxList() { Key = "#ad2d2d", Value = 42 });
            checkBoxList.Add(new CheckBoxList() { Key = "#e37c23", Value = 43 });
            checkBoxList.Add(new CheckBoxList() { Key = "#08766d", Value = 44 });
            checkBoxList.Add(new CheckBoxList() { Key = "#78002e", Value = 45 });
            checkBoxList.Add(new CheckBoxList() { Key = "#e6be19", Value = 46 });
            checkBoxList.Add(new CheckBoxList() { Key = "#73b532", Value = 47 });
            checkBoxList.Add(new CheckBoxList() { Key = "#d53f20", Value = 48 });
            checkBoxList.Add(new CheckBoxList() { Key = "#00adef", Value = 49 });
            checkBoxList.Add(new CheckBoxList() { Key = "#000000", Value = 50 });
            checkBoxList.Add(new CheckBoxList() { Key = "#000000", Value = 51 });
            checkBoxList.Add(new CheckBoxList() { Key = "#000000", Value = 52 });
            checkBoxList.Add(new CheckBoxList() { Key = "#000000", Value = 53 });
            checkBoxList.Add(new CheckBoxList() { Key = "#000000", Value = 54 });
            checkBoxList.Add(new CheckBoxList() { Key = "#000000", Value = 55 });
            checkBoxList.Add(new CheckBoxList() { Key = "#000000", Value = 56 });
            checkBoxList.Add(new CheckBoxList() { Key = "#660066", Value = 56 });
            ViewBag.EventTypelst = checkBoxList;
            return PartialView("_CreateEventType");
        }
        [HttpPost]
        public ActionResult CheckExists(long? EventId)
        {

            string practiceList = string.Empty;
            StringBuilder sBuild = new StringBuilder();
            string strEmpCount = Convert.ToString(Roster.Business.Factory.Repository.EventTypeRepository.CheckExists(Convert.ToInt64(EventId)));
            return Json(strEmpCount, JsonRequestBehavior.AllowGet);
        }

        // POST: EventTypes/Create
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

        // GET: EventTypes/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EventTypes/Edit/5
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
        // GET: EventTypes/Delete/5
        [HttpPost]
        public ActionResult Delete(long? id)
        {
            Roster.Business.Factory.Repository.EventTypeRepository.DeleteEventType(Convert.ToInt64(id));
            var EventTypelst = Roster.Business.Factory.Repository.EventTypeRepository.GetEventTypeList();
            return PartialView("_EventTypeList", EventTypelst);
        }
    }
}
