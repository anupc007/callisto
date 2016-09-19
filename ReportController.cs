using Roster.Web.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Roster.Business.Factory.Repository;
using Roster.Business.Factory.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace Roster.Web.Controllers
{
    [CustomAuthorize("Reports", "Company")]
    public class ReportController : Controller
    {
        public ActionResult Report()
        {
            return View();
        }
        public ActionResult EmployeeReport()
        {
            return View();
        }
        public ActionResult _EmployeeReport(string StartDate = "", string EndDate = "")
        {
            var lstReport = ReportRepository.LstEmployeeReport(0, StartDate, EndDate);
            return PartialView("_EmployeeReport", lstReport);
        }

        public ActionResult EventReport()
        {
            return View();
        }
        public ActionResult _EventReport()
        {
            var lstReport = ReportRepository.ExportEventReport(0);
            return PartialView("_EventReport", lstReport);
        }
        public ActionResult RosterReport()
        {
            return View();
        }
        public ActionResult _RosterReport()
        {
            var lstReport = ReportRepository.ExportRosterReport(0);
            return PartialView("_RosterReport", lstReport);
        }

        public ActionResult LocationReport()
        {
            return View();
        }
        public ActionResult _LocationReport()
        {
            var lstReport = ReportRepository.ExportLocationReport(0);
            return PartialView("_LocationReport", lstReport);
        }
        [HttpGet]
        public ActionResult ExportEmployeeReport()
        {
            var result = ReportRepository.ExportEmployeeReport(0, "", "");
            string contextIdStr = Request.Form["hdnContextId"];
            int contextId = 0;
            Int32.TryParse(contextIdStr, out contextId);
            var excelBytes = result.ToExcelBytes<ExportReportModel>();
            var fileName = string.Format("Employee {0}-{1}.xlsx", contextId, DateTime.Now.ToString("MMddyyyyHHmmssfff"));
            return excelBytes != null ? File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName) : null;
        }
        [HttpGet]
        public ActionResult ExportEventReport()
        {
            var result = ReportRepository.ExportEventReportExcel(0);
            string contextIdStr = Request.Form["hdnContextId"];
            int contextId = 0;
            Int32.TryParse(contextIdStr, out contextId);
            var excelBytes = result.ToExcelBytes<ExportEventReeportExcel>();
            var fileName = string.Format("Employee {0}-{1}.xlsx", contextId, DateTime.Now.ToString("MMddyyyyHHmmssfff"));
            return excelBytes != null ? File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName) : null;
        }
        [HttpGet]
        public ActionResult ExportRosterReport()
        {
            var result = ReportRepository.ExportRosterReportExcel(0);
            string contextIdStr = Request.Form["hdnContextId"];
            int contextId = 0;
            Int32.TryParse(contextIdStr, out contextId);
            var excelBytes = result.ToExcelBytes<ExportRosterReeport>();
            var fileName = string.Format("Employee {0}-{1}.xlsx", contextId, DateTime.Now.ToString("MMddyyyyHHmmssfff"));
            return excelBytes != null ? File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName) : null;
        }
        [HttpGet]
        public ActionResult ExportLocationReport()
        {
            var result = ReportRepository.ExportLocationReportExcel(0);
            string contextIdStr = Request.Form["hdnContextId"];
            int contextId = 0;
            Int32.TryParse(contextIdStr, out contextId);
            var excelBytes = result.ToExcelBytes<ExportLocationReeportExcel>();
            var fileName = string.Format("Employee {0}-{1}.xlsx", contextId, DateTime.Now.ToString("MMddyyyyHHmmssfff"));
            return excelBytes != null ? File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName) : null;
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult EventReportPdf(Int64 Event = 0)
        {
            var data = ReportRepository.ExportEventReport(0);
            return PartialView("EventReportPdf", data);

        }
        [HttpGet]
        public ActionResult CallEventReportPdf(long EventId = 0)
        {
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
            string basepath = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            EmployeeRepository objemprepo = new EmployeeRepository();
            var parameter = baseUrl + "Report/EventReportPdf?Event=" + EventId;
            var result = objemprepo.ExportStaffPdf("~/Users/QRCode", Server.MapPath("~/Upload"), parameter);

            string path = basepath + "/Upload/" + result.Replace("htm", "pdf");


            Response.ContentType = "Application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + "EventReport_" + DateTime.Now.Date.ToString("dd/MMM/yyyy") + ".pdf");
            Response.TransmitFile(Server.MapPath("~/Upload/") + result.Replace("htm", "pdf"));
            Response.Flush();

            //return Redirect(path);
            return null;
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult EmployeeReportPdf()
        {
            var data = ReportRepository.LstEmployeeReport(0, "", "");
            return PartialView(data);
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult RosterReportPdf()
        {
            var data = ReportRepository.ExportRosterReport(0);
            return PartialView(data);
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult LocationReportPdf()
        {
            var data = ReportRepository.ExportLocationReport(0);
            return PartialView(data);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ExportStaffPdf(long Roster = 0)
        {

            var lstReport = ReportRepository.ExportRosterReport(0);
            return PartialView("_RosterReport", lstReport);
        }

        [HttpGet]
        public ActionResult CallExportRosterPdf(long Roster = 0)
        {
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
            string basepath = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            EmployeeRepository objemprepo = new EmployeeRepository();
            var parameter = baseUrl + "Report/ExportStaffPdf?Roster=" + Roster;
            var result = objemprepo.ExportStaffPdf("~/Users/QRCode", Server.MapPath("~/Upload"), parameter);

            string path = basepath + "/Upload/" + result.Replace("htm", "pdf");


            Response.ContentType = "Application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + "RosterReport_" + DateTime.Now.Date.ToString("dd/MMM/yyyy") + ".pdf");
            Response.TransmitFile(Server.MapPath("~/Upload/") + result.Replace("htm", "pdf"));
            Response.Flush();

            //return Redirect(path);
            return null;
        }
        public JsonResult ExportToPDF(string BaseString)
        {
            string filewrite = "";
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
            string base64 = BaseString.Split(',')[1];
            string Pic_Path = Server.MapPath("~/Upload/GPNG_" + Guid.NewGuid() + ".png");
            using (FileStream fs = new FileStream(Pic_Path, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    byte[] data = Convert.FromBase64String(base64);
                    bw.Write(data);
                    bw.Close();
                }
            }
            /******************************* PDF Generation ********************************/
            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(Pic_Path);
            Document doc = new Document(new Rectangle(1.1f * img.Width, 1.1f * img.Height));
            PdfPTable tableLayout = new PdfPTable(1);
            string pdf = "PDF_" + Guid.NewGuid() + ".pdf";
            PdfWriter.GetInstance(doc, new FileStream(Server.MapPath("~/Upload/" + pdf), FileMode.Create));
            doc.Open();
            doc.Add(img);
            doc.Close();

            //string filename = Guid.NewGuid().ToString() + ".pdf"; //string.Format("{0:ddMMMyyyy}", DateTime.UtcNow.Date.ToString()) + ".pdf";
            filewrite = baseUrl + "/Upload/" + pdf; ;
            byte[] fl = Convert.FromBase64String(base64);
            return Json(filewrite);
        }
        [HttpGet]
        public virtual ActionResult Download(string filewrite)
        {
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
            //int n = file.LastIndexOf('\\');
            //string z = file.Substring(n);
            //string filename = z.Replace("\\", "");
            //string path = Server.MapPath("~/Upload/" + file);
            return File(filewrite, "application/pdf", filewrite);
        }

        [HttpGet]
        public ActionResult PayrollReport()
        {
            return View();
        }
        [HttpGet]
        public ActionResult _PayrollReport(string StartDate = "", string EndDate = "", long RosterId = 0)
        {
            if (RosterId == 0)
                RosterId = Convert.ToInt64(Roster.Business.SelectListHelper.GetRosterLst().FirstOrDefault().Value);
            var lstReport = PayrollRepository.LstPayrollReport(RosterId, StartDate, EndDate);
            return PartialView("_PayrollReport", lstReport);
        }

    }

}