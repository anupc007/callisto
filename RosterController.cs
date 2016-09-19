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
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Roster.Web.Controllers
{
    [CustomAuthorize("Rosters", "Company")]
    public class RosterController : Controller
    {
        /// <summary>
        /// Roster constroctor
        /// </summary>
        RosterRepository objRepo;
        public RosterController()
        {
            objRepo = new RosterRepository();
        }

        #region Assign shift
        /// <summary>
        /// Manage roster module
        /// </summary>
        /// <param name="Id">long</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Roster(long Id = 15)
        {
            ViewBag.Roster = Id;
            return View();
        }
        /// <summary>
        /// Get Assigned roster shift
        /// </summary>
        /// <param name="Id">long</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AssignRoster(long Id = 47)
        {
            ViewBag.RosterId = Id;
            return PartialView(objRepo.GetAssignedRoster(Id));
        }
        /// <summary>
        /// Assign shift to Roster employee 
        /// </summary>
        /// <param name="model">RosterModel</param>
        /// <returns>Roster</returns>
        [HttpPost]
        public ActionResult AssignRoster(RosterModel model)
        {
            var data = objRepo.AddUpdateRoster(model);
            if (data > 0)
            {
                return RedirectToAction("Roster", new { Id = model.Id });
            }
            else
            {

                return Json(data, JsonRequestBehavior.AllowGet);
            }


        }
        /// <summary>
        /// Add zone and position for roster employee 
        /// </summary>
        /// <param name="Zone">string</param>
        /// <param name="Position">string</param>
        /// <param name="EmployeeId">long</param>
        /// <param name="RosterId">long</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveZonePosition(string Zone, string Position, long EmployeeId, long RosterId)
        {
            objRepo.SaveZonePosition(Zone, Position, EmployeeId, RosterId);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// use to Publish Roster
        /// </summary>
        /// <param name="RosterId">long</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PublishRoster(long RosterId)
        {

            return Json(objRepo.PublishRoster(RosterId), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// use to assign budget to roster
        /// </summary>
        /// <param name="RosterId"></param>
        /// <param name="Budget"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AssignBudget(long RosterId, decimal Budget)
        {
            objRepo.AssignBudget(RosterId, Budget);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// save roster tamplate from  assigned  roster
        /// </summary>
        /// <param name="RosterId">long</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveRosterTemplate(long RosterId)
        {
            objRepo.SaveRosterTemplate(RosterId);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// add new employee on roster shift
        /// </summary>
        /// <param name="EmployeeName">string</param>
        /// <param name="EmployeeId">long</param>
        /// <param name="StartDate">DateTime</param>
        /// <param name="EndDate">DateTime</param>
        /// <returns>Json</returns>
        [HttpGet]
        public ActionResult AddNewRosterEmployee(long RosterId,string EmployeeName, long EmployeeId, DateTime StartDate, DateTime EndDate)
        {
            Employee objEmployeeRoster = new Employee();
            List<AssignRosterModel> listRoster = new List<AssignRosterModel>();
            List<Holiday> ListHoliday = objRepo.GetHolidayList(RosterId);
            bool IsHolidayOnShift = false;
             var stSt =  Helpers.GetDateWithFormate(StartDate.ToString("MM/dd/yyyy"));
            var stEnd =  Helpers.GetDateWithFormate(EndDate.ToString("MM/dd/yyyy"));
            for (var dt = stSt; dt <= stEnd; dt = dt.AddDays(1))
            {
                IsHolidayOnShift = false;
                IsHolidayOnShift = objRepo.IsHoliday(ListHoliday, dt.ToString("dd"));
                listRoster.Add(new AssignRosterModel { DateOfShift = dt, IsHoliday = IsHolidayOnShift });
            }
            objEmployeeRoster.Id = EmployeeId;
            objEmployeeRoster.Name = EmployeeName;
            objEmployeeRoster.ListOfRosterDetail = listRoster;
            return Json(objEmployeeRoster, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// get all employee for assign roster shift
        /// </summary>
        /// <param name="RosterId">long</param>
        /// <returns>json</returns>
        [HttpGet]
        public ActionResult GetAllEmployee(long RosterId, string SearchText)
        {
            return Json(EmployeeRepository.lstEmployee(RosterId, SearchText), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// delete shift
        /// </summary>
        /// <param name="id">long</param>
        /// <returns>json</returns>
        [HttpPost]
        public ActionResult DeleteShift(long id)
        {
            return Json(objRepo.DeleteShift(id), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ExportRosterShift(long RosterId)
        {
            var result = objRepo.ExportRoster(RosterId);
            string contextIdStr = Request.Form["hdnContextId"];
            int contextId = 0;
            Int32.TryParse(contextIdStr, out contextId);
            var excelBytes = result.ToExcelBytes2<ExportRosterList>();
            var fileName = string.Format("Roster Shift {0}-{1}.xlsx", contextId, DateTime.Now.ToString("MMddyyyyHHmmssfff"));
            return excelBytes != null ? File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName) : null;
        }
        #endregion

        #region Create Roster
        /// <summary>
        /// get roster detail
        /// </summary>
        /// <param name="RosterId">long</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreateRoster(long RosterId = 0, bool IsTamplate = false)
        {
            CreateRosterModel model = new CreateRosterModel();
            if (RosterId != 0)
            {

                model = objRepo.GetRosterDetail(RosterId, IsTamplate);
                if (IsTamplate)
                {
                    model.RosterId = 0;
                }
            }
            else
            {
                List<LocationManager> listLocMngr = new List<LocationManager>();
                listLocMngr.Add(new LocationManager { ManagerID = 0 });
                model.LocationManager = listLocMngr;

            }
            return View(model);
        }

        /// <summary>
        /// check if roster name is exist
        /// </summary>
        /// <param name="RosterName">string</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult IsRosterExist(CreateRosterModel model)
        {
            return Json(objRepo.IsRosterExist(model.RosterName, model.RosterId), JsonRequestBehavior.AllowGet);
        }
       /// <summary>
       /// chek if is roster allready exist 
       /// </summary>
       /// <param name="model"></param>
       /// <returns></returns>
        [HttpGet]
        public ActionResult IsRosterInEventRange(CreateRosterModel model)
        {



            bool result = true;

            if (model.EventId > 0 && !string.IsNullOrEmpty(model.StartDate) && !string.IsNullOrEmpty(model.EndDate))
            {
                var EventDate = EventRepository.GetEventDate(model.EventId);
               
                if (EventDate.StartDate <= Helpers.GetDateWithFormate(model.StartDate) && EventDate.EndDate >= Helpers.GetDateWithFormate(model.EndDate) && EventDate.EndDate >= Helpers.GetDateWithFormate(model.StartDate))
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// create new roster
        /// </summary>
        /// <param name="model">CreateRosterModel</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateRoster(CreateRosterModel model)
        {
            if (ModelState.IsValid)
            {
                model.UserId = Convert.ToInt32(UserCache.UserId);
                var result = objRepo.CreateRoster(model);
                if (result > 0)
                {
                    return RedirectToAction("Roster", new { id = result });
                }
                else
                {
                    ModelState.AddModelError("", ".AlreadyExist");
                    return View(model);
                }

            }
            return RedirectToAction("CreateRoster");
        }
        #endregion

        #region RosterSummaries UploadRoster Unavailability SharingRoster
        /// <summary>
        /// get roster Summaries
        /// </summary>
        /// <param name="SearchText">string</param>
        /// <returns _RosterSummaries>view</returns>
        [HttpGet]
        public ActionResult GetRosterSummaries(string SearchText)
        {
            return PartialView("_RosterSummaries", objRepo.GetRosterSummaries(SearchText, Convert.ToInt64(UserCache.UserId),Convert.ToInt64(UserCache.CompanyId)));
        }
        /// <summary>
        /// Get Manager List
        /// </summary>
        /// <param name="LocationId">long</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetManagerList(int LocationId)
        {
            return Json(objRepo.GetManagerList(LocationId), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// get view for upload bulk roster
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UploadRoster()
        {
            return View();
        }
        /// <summary>
        /// upload bulk roster shift from csv
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InsertRosterFromCsv()
        {
            var attachedFile = System.Web.HttpContext.Current.Request.Files["CsvDoc"];

            if (attachedFile == null)
            {
                return Content("Csv has no data.");
            }
            List<string> myStringColumn = new List<string>();
            //  ICsvParser csvParser = new CsvParser(new StreamReader(attachedFile.InputStream));
            CsvReader csvReader = new CsvReader(new StreamReader(attachedFile.InputStream));

            int count = 0;
            int TotalCount = 0;
            try
            {
                UploadRosterModel objRoster = new UploadRosterModel();
                objRoster.RosterName = System.Web.HttpContext.Current.Request.Form["RosterName"];
                objRoster.EventId = Convert.ToInt64(!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Form["EventId"]) ? System.Web.HttpContext.Current.Request.Form["EventId"] : "0");
                objRoster.LocationId = Convert.ToInt64(System.Web.HttpContext.Current.Request.Form["LocationId"]);
                objRoster.Budget = Convert.ToDecimal(System.Web.HttpContext.Current.Request.Form["Budget"]);
                objRoster.UserId = Convert.ToInt64(UserCache.UserId);

                var RosterId = objRepo.CreateRosterByCsv(objRoster);
                string strZone = string.Empty;
                string strPosition = string.Empty;
                while (csvReader.Read())
                {
                    try
                    {
                        strZone = string.Empty;
                        strPosition = string.Empty;
                        TotalCount = TotalCount + 1;
                        AssignRosterModel objInsertModel = new AssignRosterModel();
                        if (objRoster.EventId > 0)
                        {
                            strZone = csvReader.GetField<string>("Zone");
                            strPosition = csvReader.GetField<string>("Position");
                        }
                        string strEmail = csvReader.GetField<string>("Email");
                        string strShiftDate = csvReader.GetField<string>("ShiftDate");
                        string strTimeFrom = csvReader.GetField<string>("StartTime");
                        string strTimeTo = csvReader.GetField<string>("EndTime");
                        int EmpId = EmployeeRepository.GetUserId(strEmail);
                        if (EmpId > 0 && strShiftDate != "" && strTimeTo != "")
                        {
                            //if (TimeSpan.Parse(strTimeFrom) < TimeSpan.Parse(strTimeTo))
                            //{
                            objInsertModel.EmployeeId = EmpId;
                            objInsertModel.DateOfShift = Convert.ToDateTime(strShiftDate);
                            objInsertModel.RosterId = RosterId;
                            objInsertModel.Position = strPosition;
                            objInsertModel.Zone = strZone;

                            DateTime dt = DateTime.Parse(strTimeFrom); // No error checking
                            DateTime dt2 = DateTime.Parse(strTimeTo); // No error checking
                            TimeSpan st = dt.TimeOfDay;
                            TimeSpan et = dt2.TimeOfDay;


                            objInsertModel.ShiftTimeFrom = st;
                            objInsertModel.ShiftTimeTo = et;
                            objInsertModel.Id = 0;
                            var result = objRepo.AddUpdateRostercsv(objInsertModel);
                            if (result > 0)
                            {
                                count = count + 1;
                            }

                            //}
                        }
                    }

                    catch (Exception ex)
                    {

                    }

                }

                return Content(count.ToString() + "," + Convert.ToString(TotalCount - count));
            }
            catch (Exception ex)
            {
                return Content("0");
            }



        }
        /// <summary>
        /// get view for roster employee Unavailability 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Unavailability()
        {
            return View();
        }
        /// <summary>
        /// get roster summaries
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Summaries()
        {
            return View(objRepo.GetRosterSumriesTime(Convert.ToInt64(UserCache.UserId)));
        }

        /// <summary>
        /// get roster summaries
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditRoster()
        {
            return View("RosterList", objRepo.GetRosterSumriesTime(Convert.ToInt64(UserCache.UserId)));
        }

        [HttpGet]
        public ActionResult _RosterList(string SearchText)
        {
            return PartialView(objRepo.GetRosterForEdit(SearchText, Convert.ToInt64(UserCache.UserId)));
        }

        /// <summary>
        /// get UpComing Roster
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpComingRoster(long UserId = 0, long LocationId = 0)
        {
            return View(objRepo.GetCommingRoster(UserId, LocationId));
        }

        /// <summary>
        /// get roster sharing view with roster header detail
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RosterSharing()
        {
            ViewBag.RosterSharingHeader = objRepo.GetRosterSharingHeader();
            return View();
        }
        /// <summary>
        /// roster sharing partial view 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _Sharing()
        {
            return PartialView();
        }
        /// <summary>
        /// share roster with location or manager
        /// </summary>
        /// <param name="Model">RosterSharing</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _Sharing(RosterSharing Model)
        {
            var data = objRepo.RosterSharing(Model);
            return RedirectToAction("RosterSharing");
        }

        [HttpGet]
        public ActionResult _ApprovedRoster(string SearchText)
        {
            return PartialView("_ApprovedRoster", objRepo.GetRosterSummaries(SearchText, Convert.ToInt64(UserCache.UserId), Convert.ToInt64(UserCache.CompanyId)));
        }

        [HttpGet]
        public ActionResult GetCompany(string SearchText)
        {
            return PartialView("_CompanyList", EmployeeRepository.GetCompanyOrAdmin(SearchText, true, 0));
        }
        [HttpGet]
        public ActionResult GetAdministrator(string SearchText, long CompanyId = 0)
        {
            return PartialView("_UserList", EmployeeRepository.GetCompanyOrAdmin(SearchText, false, CompanyId));
        }
        /// <summary>
        /// get shared roster
        /// </summary>
        /// <param name="Type">string</param>
        /// <param name="Id">long</param>
        /// <returns>json</returns>
        [HttpGet]
        public ActionResult GetSharingRoster(string Type, long Id)
        {
            return Json(EmployeeRepository.GetListForSharing(Type, Id), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// get all employee pending leave
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _PendingLeave()
        {
            return PartialView(objRepo.GetPendingLeave(Convert.ToInt64(UserCache.UserId), Convert.ToInt64(UserCache.CompanyId)));
        }
        /// <summary>
        /// get employee unavailability summary
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _UnAvailabilitySummary()
        {
            ViewBag.Year = System.DateTime.UtcNow.Year;
            return PartialView(objRepo.GetMonthlyUnavilability(Convert.ToInt64(UserCache.UserId), Convert.ToInt64(UserCache.CompanyId)));
        }
        /// <summary>
        /// get calener
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _Calender()
        {
            return PartialView();
        }
        #endregion

        #region Roster AccessControls
        /// <summary>
        /// get access control on roster
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AccessControls()
        {
            Session["SelectedRosterList"] = null;
            RosterAccessControlModel objRosterAccessControlModel = new RosterAccessControlModel();
            //objRosterAccessControlModel.ListRosterData = RosterRepository.GetRoster();
            //objRosterAccessControlModel.ListManagerData = RosterRepository.GetManager();
            return View(objRosterAccessControlModel);


        }
        [HttpGet]
        public ActionResult GetAccessRosterControls(string SearchRosterText)
        {
            RosterAccessControlModel objRosterAccessControlModel = new RosterAccessControlModel();
            objRosterAccessControlModel.ListRosterData = RosterRepository.GetRoster();
            objRosterAccessControlModel.ListManagerData = RosterRepository.GetManager();
            if (SearchRosterText != "")
            {
                if (objRosterAccessControlModel.ListRosterData.Count > 0)
                {
                    var rosterData = objRosterAccessControlModel.ListRosterData.Where(oh => oh.RosterName.ToLower().Contains(SearchRosterText.ToLower())).ToList();
                    objRosterAccessControlModel.ListRosterData = rosterData;
                }
            }
            //if (SearchManagerText != "")
            //{
            //    if (objRosterAccessControlModel.ListManagerData.Count > 0)
            //    {
            //        var managerData = objRosterAccessControlModel.ListManagerData.Where(oh => oh.Name.ToLower().Contains(SearchManagerText.ToLower())).ToList();
            //        objRosterAccessControlModel.ListManagerData = managerData;
            //    }


            //}
            // return View(objRosterAccessControlModel);
            return PartialView("_AccessControlRoster", objRosterAccessControlModel);

        }

        [HttpGet]
        public ActionResult GetAccessManagerControls(string SearchManagerText)
        {
            RosterAccessControlModel objRosterAccessControlModel = new RosterAccessControlModel();
            objRosterAccessControlModel.ListRosterData = RosterRepository.GetRoster();
            objRosterAccessControlModel.ListManagerData = RosterRepository.GetManager();
            //if (SearchRosterText != "")
            //{
            //    if (objRosterAccessControlModel.ListRosterData.Count > 0)
            //    {
            //        var rosterData = objRosterAccessControlModel.ListRosterData.Where(oh => oh.RosterName.ToLower().Contains(SearchRosterText.ToLower())).ToList();
            //        objRosterAccessControlModel.ListRosterData = rosterData;
            //    }
            //}
            if (SearchManagerText != "")
            {
                if (objRosterAccessControlModel.ListManagerData.Count > 0)
                {
                    var managerData = objRosterAccessControlModel.ListManagerData.Where(oh => oh.Name.ToLower().Contains(SearchManagerText.ToLower())).ToList();
                    objRosterAccessControlModel.ListManagerData = managerData;
                }


            }
            // return View(objRosterAccessControlModel);
            return PartialView("_AccessControlManager", objRosterAccessControlModel);

        }
        /// <summary>
        /// assign access on roster
        /// </summary>
        /// <param name="obj">RosterAccessControlModel</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AccessControls(RosterAccessControlModel obj)
        {

            string msz = string.Empty;
            try
            {
                if (obj != null)
                {
                    string[] PermissionIds = obj.PermissionIds.Split(',');
                    foreach (var PermissionId in PermissionIds)
                    {
                        switch (PermissionId)
                        {
                            case "1":
                                obj.ViewFullRosterAndBudget = true;
                                break;
                            case "2":
                                obj.ViewFullRosterWithoutBudget = true;
                                break;
                            case "3":
                                obj.ViewRosterSummariesView = true;
                                break;
                            case "4":
                                obj.AmendFullRosterBudget = true;
                                break;
                            case "5":
                                obj.AmendFullRosterWithoutBudget = true;
                                break;
                            case "6":
                                obj.AmendPartial = true;
                                break;
                            case "7":
                                obj.AmendReAllocation = true;
                                break;




                        }
                    }
                    // List<RosterData> objRosterDataList = new List<RosterData>();
                    //if (Session["SelectedRosterList"] != null)
                    //{
                    // objRosterDataList = (List<RosterData>)Session["SelectedRosterList"];
                    string[] RosterIds = obj.RosterIds.Split(',');
                    if (RosterIds.Count() > 0)
                    // if (objRosterDataList != null && objRosterDataList.Count > 0)
                    {
                        string[] ManagerIds = obj.ManagerIds.Split(',');
                        if (ManagerIds.Count() > 0)
                        {
                            foreach (var ManagerId in ManagerIds)
                            {

                                RosterAccessControlModel objNew;
                                foreach (var RosterId in RosterIds)
                                {
                                    objNew = new RosterAccessControlModel();
                                    objNew.Id = 0;
                                    // objNew.RosterId = item.Id;
                                    objNew.RosterId = Convert.ToInt64(RosterId);
                                    objNew.ManagerId = Convert.ToInt64(ManagerId);

                                    objNew.ViewFullRosterAndBudget = obj.ViewFullRosterAndBudget;
                                    objNew.ViewFullRosterWithoutBudget = obj.ViewFullRosterWithoutBudget;

                                    objNew.ViewRosterSummariesView = obj.ViewRosterSummariesView;

                                    objNew.AmendFullRosterBudget = obj.AmendFullRosterBudget;

                                    objNew.AmendFullRosterWithoutBudget = obj.AmendFullRosterWithoutBudget;

                                    objNew.AmendPartial = obj.AmendPartial;

                                    objNew.AmendReAllocation = obj.AmendReAllocation;
                                    objRepo.AddUpdateRosterAccessControl(objNew);
                                }
                            }
                            msz = "Saved successfully";
                        }
                        else
                            msz = "Plaese select atleast one manager";
                    }
                    else
                        msz = "Plaese select atleast one roster";
                }
                // }
                //else
                //  msz = "Value is not valid";
            }
            catch (Exception ex)
            { msz = "Some error occured, try again later"; }


            return Content(msz);
        }

        /// <summary>
        /// select roster for assign access on view part
        /// </summary>
        /// <param name="RosterName"></param>
        /// <param name="RosterId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _SelectedRoster(string RosterName, int RosterId)
        {
            if (Session["SelectedRosterList"] != null)
            {
                AddNewRowToSelectedRoster(RosterName, RosterId);
            }
            else
            { SetInitialRow(RosterName, RosterId); }
            return PartialView("_SelectedRoster");
        }
        /// <summary>
        /// set access 
        /// </summary>
        /// <param name="RosterName"></param>
        /// <param name="RosterId"></param>
        private void SetInitialRow(string RosterName, int RosterId)
        {
            List<RosterData> objRosterDataList = new List<RosterData>();
            RosterData objSingleRosterData = new RosterData();
            objSingleRosterData.SelectedRosterRowNumber = 1;
            objSingleRosterData.Id = RosterId;
            objSingleRosterData.RosterName = RosterName;

            objRosterDataList.Add(objSingleRosterData);

            Session["SelectedRosterList"] = objRosterDataList;
        }
        /// <summary>
        /// add new roster for access
        /// </summary>
        /// <param name="RosterName"></param>
        /// <param name="RosterId"></param>
        private void AddNewRowToSelectedRoster(string RosterName, int RosterId)
        {
            if (Session["SelectedRosterList"] != null)
            {
                List<RosterData> objRosterDataList = (List<RosterData>)Session["SelectedRosterList"];
                RosterData objSingleRosterData = new RosterData();
                objSingleRosterData.SelectedRosterRowNumber = objRosterDataList.Count + 1;
                objSingleRosterData.Id = RosterId;
                objSingleRosterData.RosterName = RosterName;
                objRosterDataList.Add(objSingleRosterData);

                Session["SelectedRosterList"] = objRosterDataList;
            }

        }
        /// <summary>
        /// delete row from access list 
        /// </summary>
        /// <param name="RosterId"></param>
        /// <returns></returns>
        public ActionResult DeleteRowFromSelectedRoster(int RosterId)
        {

            List<RosterData> objRosterDataList = new List<RosterData>();
            RosterData objSingleRosterData = new RosterData();

            if (Session["SelectedRosterList"] != null)
            {
                objRosterDataList = (List<RosterData>)Session["SelectedRosterList"];
            }


            objSingleRosterData = (objRosterDataList.Where(M => M.Id == RosterId)).SingleOrDefault();
            if (objSingleRosterData != null)
            {
                objRosterDataList.Remove(objSingleRosterData);
                Session["SelectedRosterList"] = objRosterDataList;
            }



            return PartialView("_SelectedRoster");
        }

        #endregion

        #region AccessMatrix
        /// <summary>
        /// get access matrix list accordin to roster
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AccessMatrix()
        {
            ViewBag.AccessMatrixData = objRepo.GetAccessMatrix();
            return View();
        }
        #endregion

        #region Leave
        /// <summary>
        /// get leave detail
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Leave()
        {
            ViewBag.PendingLeaveData = objRepo.GetPendingLeave(Convert.ToInt64(UserCache.UserId),Convert.ToInt64(UserCache.CompanyId));
            ViewBag.MonthlyLeaveSnapshotData = objRepo.GetMonthlyLeaveSnapshot(Convert.ToInt64(UserCache.UserId), Convert.ToInt64(UserCache.CompanyId));
            ViewBag.LeaveHeader = objRepo.GetLeaveHeader();

            return View();
        }

        /// <summary>
        /// leave data for show on calendar
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LeaveDataForCalendar()
        {

            string practiceList = string.Empty;
            StringBuilder sBuild = new StringBuilder();
            string practiceDate = string.Empty;
            string instrumentName = string.Empty;

            List<GetLeaveDataForCalendarData> lst = objRepo.GetLeaveDataForCalendar(Convert.ToInt64(UserCache.UserId), Convert.ToInt64(UserCache.CompanyId));
            List<CalendarData> lstPractice = new List<CalendarData>();
            if (lst != null)
            {
                for (int i = 0; i < lst.Count; i++)
                {

                    practiceDate = lst[i].FromDate;
                    DateTime dt = Convert.ToDateTime(practiceDate);
                    if (!string.IsNullOrEmpty(practiceDate))
                        practiceDate = Convert.ToDateTime(practiceDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

                    instrumentName = lst[i].Name;

                    CalendarData obj = new CalendarData { start = practiceDate, title = instrumentName };
                    lstPractice.Add(obj);
                }
            }
            var details = lstPractice.ToArray();
            return Json(details, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Approve DisApprove Leave
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ActionResult ApproveDisApproveLeave(int id, int status)
        {
            string msz = string.Empty;
            try
            {
                bool isapproved = objRepo.ApproveDisApproveLeave(id, Convert.ToBoolean(status));
                if (isapproved)
                    msz = "Success";
                else
                    msz = "Failure";
            }
            catch (Exception ex)
            {
                msz = "Failure";

            }
            return Json(msz, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ExportLeaveDataToPdf()
        {
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
            string basepath = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            EmployeeRepository objemprepo = new EmployeeRepository();
            var parameter = baseUrl + "Roster/ExportLeavePDF";
            var result = objemprepo.ExportStaffPdf("~/Roster/ExportLeavePDF", Server.MapPath("~/Upload"), parameter);

            string path = basepath + "/Upload/" + result.Replace("htm", "pdf");


            Response.ContentType = "Application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + "MonthlyLeaveSnapshot_" + DateTime.Now.Date.ToString("dd/MMM/yyyy") + ".pdf");
            Response.TransmitFile(Server.MapPath("~/Upload/") + result.Replace("htm", "pdf"));
            Response.Flush();

            //return Redirect(path);
            return null;
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ExportLeavePDF()
        {

            //   var data = objRepo.GetMonthlyLeaveSnapshot();
            ViewBag.MonthlyLeaveSnapshotData = objRepo.GetMonthlyLeaveSnapshot(Convert.ToInt64(UserCache.UserId), Convert.ToInt64(UserCache.CompanyId));
            return PartialView();
        }

        #endregion


    }

}