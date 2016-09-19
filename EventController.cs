using CsvHelper;
using DDay.iCal;
using DDay.iCal.Serialization;
using DDay.iCal.Serialization.iCalendar;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Roster.Business;
using Roster.Business.Factory.Repository;
using Roster.Business.Factory.ViewModels;
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
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;

namespace Roster.Web.Controllers
{
    [CustomAuthorize("Company","Event")]
    public class EventController : Controller
    {
        CalendarService service;
        static string gFolder = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/MyGoogleStorage");

        public string[] Scopes = { CalendarService.Scope.Calendar };
        public string ApplicationName = "Callisto Google App";
        // GET: Event
        public ActionResult Index()
        {
            return View();
        }

        // GET: Event/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Event/Create
        [HttpGet]
        public ActionResult CreateEvent()
        {
            Session["AssignManagerToEventList"] = null;
            List<EventTypeModel> model = EventTypeRepository.GetEventTypeList();
            ViewBag.EventType = model;
            return View();
        }

        [HttpPost]
        public ActionResult CreateEvent(EventModel model, FormCollection col)
        {
            var attachedFile = System.Web.HttpContext.Current.Request.Files["ProfilePic"];

            if (attachedFile != null && attachedFile.ContentLength > 0)
            {
                string ext = Path.GetExtension(attachedFile.FileName);

                String UUID = Guid.NewGuid().ToString();

                string path = System.IO.Path.Combine(Server.MapPath("~/Upload/"), UUID + ext);
                attachedFile.SaveAs(path);

                model.EventImage = UUID + ext;
            }

            EventRepository objEvent = new EventRepository();
            model.EventTypeId = Convert.ToInt32(col["hdnEventType"]);
            model.Lat = Convert.ToString(col["hdnLat"]);
            model.Lng = Convert.ToString(col["hdnLong"]);
            if (string.IsNullOrEmpty(Convert.ToString(col["hdnId"])))
                model.Id = 0;
            else
                model.Id = Convert.ToInt64(col["hdnId"]);
            model.address = Convert.ToString(col["txtaddress"]);
            model.ManagerId = Convert.ToInt32(col["ManagerId"]);
            int? eventId = objEvent.AddUpdateEvent(model);

            if (Session["AssignManagerToEventList"] != null)
            {
                List<ManagerData> objManagerData = (List<ManagerData>)Session["AssignManagerToEventList"];

                string managerIds = String.Join(",", objManagerData.ToArray().Select(x => x.ManagerId));
                // string Join<T>(",", IEnumerable<T> objManagerData); string.Join(",",  objManagerData.);
                objEvent.AddUpdateEventAssignManager(managerIds, Convert.ToInt32(eventId));
            }

            return RedirectToAction("EventList");
        }

        public class CheckBoxList
        {
            public int CategoryId { get; set; }
            public string CategoryDescription { get; set; }
            public bool Selected { get; set; }
        }
        [HttpGet]
        public ActionResult EventList()
        {
            List<EventModel> model = new List<EventModel>();
            return View(model);
        }
        [HttpGet]
        public ActionResult _EventList()
        {

            var Eventlst = EventRepository.lstEvent();
            return PartialView("_EventList", Eventlst);
        }
        [HttpPost]
        public ActionResult EventForCalendar(string Yearevent, long FilterType)
        {

            string practiceList = string.Empty;
            StringBuilder sBuild = new StringBuilder();
            string practiceDate = string.Empty;
            string instrumentName = string.Empty;
            EventRepository objeventrep = new EventRepository();
            List<GetEventForCalendarData> lst = objeventrep.GetEventForCalendar(Yearevent, FilterType);
            List<CalendarData> lstPractice = new List<CalendarData>();
            if (lst != null)
            {
                for (int i = 0; i < lst.Count; i++)
                {
                    practiceDate = lst[i].StartDate;
                    DateTime dt = Convert.ToDateTime(practiceDate);
                    if (!string.IsNullOrEmpty(practiceDate))
                        practiceDate = Convert.ToDateTime(practiceDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

                    instrumentName = lst[i].Title;
                    CalendarData obj = null;
                    obj = new CalendarData { start = practiceDate, title = instrumentName, backgroundColor = lst[i].colorcode, textColor = lst[i].txtcolor };
                    lstPractice.Add(obj);
                }
            }
            //if (lst.Count == 0)
            //{
            //    CalendarData obj = new CalendarData { start = "", title = "" };
            //    lstPractice.Add(obj);
            //}
            var details = lstPractice.ToArray();
            return Json(details, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult EventCalendar()
        {
            List<EventTypeModel> model = EventTypeRepository.GetEventTypeList();
            ViewBag.EventType = model;
            return View();
        }
        [HttpGet]
        public ActionResult Report()
        {
            EventReportModel model = new EventReportModel();
            return View(model);
        }

        [HttpGet]
        public ActionResult _ReportList(long? EventTypeId = 0)
        {
            var EventReportlst = EventRepository.lstEventReport(Convert.ToInt64(EventTypeId));
            return PartialView("_ReportList", EventReportlst);
        }
        // GET: Event/Edit/5
        public ActionResult Edit(long? id)
        {
            Session["AssignManagerToEventList"] = null;
            EventModel model = new EventModel();
            model = EventRepository.GetEventById(Convert.ToInt64(id));
            // model.StartDate = model.StartDate.Date;
            // model.EndDate = Convert.ToDateTime(model.EndDate).ToString("dd/MM/yyyy");
            List<EventTypeModel> model1 = EventTypeRepository.GetEventTypeList();
            ViewBag.EventTypelst = model1;
            ViewBag.EventType = Convert.ToString(model.EventTypeId);
            if (string.IsNullOrEmpty(model.address))
                model.address = "211 Franklin St, Melbourne VIC 3004, Australia";
            return View(model);
        }
        // POST: Event/Edit/5
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

        // GET: Event/Delete/5
        public ActionResult Delete(int id)
        {
            int isdeleted = EventRepository.DeleteEventById(id);
            return Json(isdeleted, JsonRequestBehavior.AllowGet);
            // return RedirectToAction("EventList");
        }

        // POST: Event/Delete/5
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


        [HttpGet]
        public ActionResult _AssignManagerToEvent(string ManagerName, int ManagerId, int EventId)
        {
            if (EventId > 0)
            {
                Session["AssignManagerToEventList"] = EventRepository.GetEventAssignManager(EventId);
            }
            if (ManagerId > 0)
            {
                List<ManagerData> objManagerData = new List<ManagerData>();
                ManagerData objSingleManagerData = new ManagerData();
                objSingleManagerData.ManagerId = ManagerId;
                objSingleManagerData.ManagerName = ManagerName;
                if (Session["AssignManagerToEventList"] != null)
                {
                    objManagerData = (List<ManagerData>)Session["AssignManagerToEventList"];

                    if ((objManagerData.Where(M => M.ManagerId == ManagerId)).SingleOrDefault() == null)
                    {
                        objManagerData.Add(objSingleManagerData);
                    }
                }
                else
                    objManagerData.Add(objSingleManagerData);

                Session["AssignManagerToEventList"] = objManagerData;
            }

            return PartialView("_AssignManagerToEvent");
        }

        public ActionResult DeleteRowFromAssignManager(int ManagerId)
        {

            List<ManagerData> objManagerDataList = new List<ManagerData>();
            ManagerData objSingleManagerData = new ManagerData();

            if (Session["AssignManagerToEventList"] != null)
            {
                objManagerDataList = (List<ManagerData>)Session["AssignManagerToEventList"];
            }


            objSingleManagerData = (objManagerDataList.Where(M => M.ManagerId == ManagerId)).SingleOrDefault();
            if (objSingleManagerData != null)
            {
                objManagerDataList.Remove(objSingleManagerData);
                Session["AssignManagerToEventList"] = objManagerDataList;
            }



            return PartialView("_AssignManagerToEvent");
        }
        public void ExportCalendar()
        {
            List<EventModel> obj = EventRepository.lstEvent();
           
                bool preview = (Request.QueryString[null] ?? String.Empty).Contains("preview");

                iCalendar iCal = new iCalendar();

                //DataTable events = Db.LoadEvents(DateTime.Today.AddDays(-7), DateTime.MaxValue);

                foreach (var item in obj)
                {
                    DDay.iCal.Event evt = iCal.Create<DDay.iCal.Event>();

                    evt.Start = new iCalDateTime((DateTime)item.StartDate);
                    evt.End = new iCalDateTime((DateTime)item.EndDate);
                    evt.Description = (string)item.Title;
                    evt.Summary = (string)item.Title;

                }

                iCalendarSerializer serializer = new iCalendarSerializer();
                string output = serializer.SerializeToString(iCal);

                if (preview)
                {
                    Response.ContentType = "text/calendar";
                }
                else
                {
                    Response.ContentType = "text/calendar";
                    // Response.Headers["Content-Disposition"] = "attachment; filename=calendar.ical";
                }

                Response.Write(output);
                Response.End();
           
            
        }
        [HttpGet]
        public async Task<string> ExportGoogleCalendar(string state="", string code="")
        {               
               string response=await AddEventOnGoogleCalender(UserCache.Email);
               return response;
        }
        public async Task<string> AddEventOnGoogleCalender(string userAccountEmail)
        {

            IAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow(
        new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = GetClientConfiguration().Secrets,
            DataStore = new FileDataStore(gFolder),
            Scopes = new[] { CalendarService.Scope.Calendar }
        });
           
            var uri = Request.Url.ToString();
            var code = Request["code"];

            if (code != null)
            {

                var token = await flow.ExchangeCodeForTokenAsync(userAccountEmail, code,
                     uri.Substring(0, uri.IndexOf("?")), CancellationToken.None);

               
                
                    var oauthState = AuthWebUtility.ExtracRedirectFromState(
                     flow.DataStore, userAccountEmail, Request["state"]).Result;
                     Response.Redirect(oauthState);
                
                // Extract the right state.
               
            }
            else
            {
                var result = new AuthorizationCodeWebApp(flow, uri, uri).AuthorizeAsync(userAccountEmail,
                    CancellationToken.None).Result;               
                if (result.RedirectUri != null)
                {
                    // Redirect the user to the authorization server.
                    Response.Redirect(result.RedirectUri);
                }
                else
                {
                    // The data store contains the user credential, so the user has been already authenticated.
                    service = new CalendarService(new BaseClientService.Initializer
                    {
                        ApplicationName = ApplicationName,
                        HttpClientInitializer = result.Credential
                    });


                    List<EventModel> obj = EventRepository.lstEvent();
                    List<Google.Apis.Calendar.v3.Data.Event> eventList = new List<Google.Apis.Calendar.v3.Data.Event>();
                    foreach (var item in obj)
                    {
                        eventList.Add(new Google.Apis.Calendar.v3.Data.Event
                        {
                            Summary = (string)item.Title,
                            Location = (string)item.address,
                            Description = (string)item.ShortDesc,
                            Start = new EventDateTime()
                            {
                                DateTime = item.StartDate,
                                TimeZone = "America/Chicago"
                            },
                            End = new EventDateTime()
                            {
                                DateTime = item.EndDate,
                                TimeZone = "America/Chicago"
                            },
                            Recurrence = new String[] { "RRULE:FREQ=DAILY;COUNT=2" }
                        });


                    }
                    String calendarId = "primary";
                    foreach (var item in eventList)
                    {
                        try
                        {
                            EventsResource.InsertRequest request = service.Events.Insert(item, calendarId);
                            Google.Apis.Calendar.v3.Data.Event createdEvent = request.Execute();
                        }
                        catch (Exception ex)
                        {


                        }
                    }
                }
            }
            return "Event are uploded on google calendar now you can close this window";
        }
        

        public static GoogleClientSecrets GetClientConfiguration()
        {
            using (var stream = new FileStream(gFolder + @"\client_secret.json", FileMode.Open, FileAccess.Read))
            {
                return GoogleClientSecrets.Load(stream);
            }
        }       
        private string DateFormat
        {
            get { return "yyyyMMddTHHmmssZ"; } // 20060215T092000Z
        }
    
    }

}
