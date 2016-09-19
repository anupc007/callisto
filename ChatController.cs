using Roster.Web.App_Start;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;

namespace Roster.Web.Controllers
{
      [CustomAuthorize("Administration", "Company")]
    public class ChatController : Controller
    {

        // GET: Chat
        public ActionResult Chat()
        {
            return View();
        }

    }

}
