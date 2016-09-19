using Roster.Business.Factory.Repository;
using Roster.Business.Factory.ViewModels;
using Roster.Web.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Roster.Web.Controllers
{
    [CustomAuthorize("Messages", "Company")]
    public class MessageSendController : Controller
    {
        // GET: MessageSend
        [HttpGet]
        public ActionResult MessageSend(string msg="")
        {
            if (!string.IsNullOrEmpty(msg))
            {
                ViewBag.Message = msg;
            }
                return View();
        }

        [HttpPost]
        public ActionResult MessageSend(MessageSendModel msgModel)
        {
            MessageSendRepository.SendPushMessage(msgModel);
            return RedirectToAction("MessageSend", new {msg="Message send successfully " });
        }

        [HttpPost]
        public ActionResult EmployeCnt(long? EventId)
        {

            string practiceList = string.Empty;
            StringBuilder sBuild = new StringBuilder();
            string strEmpCount = MessageSendRepository.GetEmployeeCount(Convert.ToInt64(EventId));
            return Json(strEmpCount, JsonRequestBehavior.AllowGet);
        }

        // GET: MessageSend/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MessageSend/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MessageSend/Create
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

        // GET: MessageSend/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MessageSend/Edit/5
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

        // GET: MessageSend/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MessageSend/Delete/5
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
