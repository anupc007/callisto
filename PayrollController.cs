using Roster.Web.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Roster.Web.Controllers
{
    [CustomAuthorize("Payroll", "Company")]
    public class PayrollController : Controller
    {
        public ActionResult Export()
        {
            return View();
        
        }
        public ActionResult Report()
        {
            return View();

        }
    }
}