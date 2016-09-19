using CsvHelper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNet.Identity;
using Nemiro.OAuth;
using Roster.Business;
using Roster.Business.Factory.Repository;
using Roster.Business.Factory.ViewModels;
using Roster.Web.App_Start;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Roster.Web.Controllers
{
    [CustomAuthorize("Administration", "Company")]
    public class UsersController : Controller
    {
        private static string gFolder = System.Web.HttpContext.Current.Server.MapPath("~") + "/Template";
        #region Manage Administrator and manager
        /// <summary>
        /// get user module
        /// </summary>
        /// <returns></returns>
        public ActionResult Users(int Id = 0)
        {
            SetCookees("Manager");
            string Role = GetRole();
            ViewBag.Role = Role;

            ViewBag.UserId = Id;
            return View();
        }

        /// <summary>
        /// Get view for add administrator
        /// </summary>
        /// <returns></returns>
        public ActionResult Admin(int Id = 0)
        {
            SetCookees("Admin");
            ViewBag.UserId = Id;
            ViewBag.Role = GetRole();
            return View("Users");
        }
        /// <summary>
        /// get staff list administrator or manager
        /// </summary>
        /// <param name="Id">long</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UserList(long Id = 0)
        {
            int RoleId = GetUserRoleForManage();
            ViewBag.UserRole = GetRole();
            return PartialView("_UserList", EmployeeRepository.GetAllUser(Id, RoleId));
        }
        /// <summary>
        /// Get administrator and manager detail
        /// </summary>
        /// <param name="Id">long</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ManageUsers(long Id = 0)
        {
            ViewBag.ManageRole = GetRole();
            return PartialView("_ManageUsers", EmployeeRepository.GetUserDetails(Id));
        }
        /// <summary>
        /// add update administrator and manager
        /// </summary>
        /// <param name="model">EmployeeCompositModel</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ManageUsers(EmployeeCompositModel model)
        {
            var attachedFile = System.Web.HttpContext.Current.Request.Files["ProfilePic"];
            if (attachedFile != null && attachedFile.ContentLength > 0)
            {
                string ext = Path.GetExtension(attachedFile.FileName);
                String UUID = Guid.NewGuid().ToString();
                string path = System.IO.Path.Combine(Server.MapPath("~/Upload/"), UUID + ext);
                attachedFile.SaveAs(path);
                model.picture = UUID + ext;
            }
            model.RoleId = GetUserRoleForManage();
            model.CompanyId = Convert.ToInt64(UserCache.CompanyId);
            var result = EmployeeRepository.AddUpdateUser(model);
            if (model.Id == 0)
            {
                if (result > 0)
                {
                  
                    try
                    {
                        string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                        string Token = MyExtensions.Encrypt(result.ToString());
                        string Link = "<a href='" + baseUrl + "Account/CreatePassword?token=" + Token + "'>Click here</a>";
                        var path = gFolder + @"\UserCreateEmail.html";
                        EmployeeRepository.SendMailToCreatePassword(model.Email, model.Name, Link, path);
                    }
                    catch(Exception ex)
                    {
                        
                    
                    }
                  
                }
            }
            if (Request.Cookies["subMenu"] != null)
            {
                Response.Cookies["subMenu"].Expires = DateTime.Now.AddDays(-1);
            }

            if (GetRole() == "Manager")
            {
                return RedirectToAction("Employee");
            }
            else
            {
                return RedirectToAction("Employee");
            }

        }
        /// <summary>
        /// delete user
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Delete(int Id)
        {
            var result = EmployeeRepository.DeleteEmployee(Id);
            if (Request.Cookies["subMenu"] != null)
            {
                Response.Cookies["subMenu"].Expires = DateTime.Now.AddDays(-1);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Manage Qr Code
        /// <summary>
        /// get view for show qrcode
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult QRCode()
        {
            EmployeeRepository objemprepo = new EmployeeRepository();
            var result = objemprepo.UserDetail(Server.MapPath("~/Uploads/QRCode"));

            return View(result);
        }

        /// <summary>
        /// get qrcode list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult _QRCode()
        {
            EmployeeRepository objemprepo = new EmployeeRepository();
            var result = objemprepo.UserDetail(Server.MapPath("~/Uploads/QRCode"));
            return PartialView(result);
        }
        /// <summary>
        /// get qrcode list for download in pdf
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult _QrCodePdf(string Ids)
        {
            List<Int64> IdList = Ids.Trim(',').Split(',').Select(X => Convert.ToInt64(X)).ToList();
            EmployeeRepository objemprepo = new EmployeeRepository();
            var result = objemprepo.UserDetailByIDs(Server.MapPath("~/Uploads/QRCode"), IdList);
            //ViewBag.UserList = result;
            return PartialView(result);
        }
        /// <summary>
        /// download qrcode in pdf
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Pdfdownload(string Ids)
        {
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
            string basepath = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            EmployeeRepository objemprepo = new EmployeeRepository();
            var result = objemprepo.QRCodePdf("~/Users/QRCode", Server.MapPath("~/Uploads/pdf"), baseUrl + "Users/_QrCodePdf?Ids=" + Ids);

            string path = basepath + "/Uploads/pdf/" + result.Replace("htm", "pdf");
            string filename = Server.MapPath("~/Uploads/pdf/" + result.Replace("htm", "pdf")); ;
            string newfilename = "QrCode_" + DateTime.Now.Date.ToString("dd/MMM/yyyy") + ".pdf";
            AlertMesssage(UserCache.Email, "QR Code", "QR Code", filename, newfilename);
            Response.ContentType = "Application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + "QrCode_" + DateTime.Now.Date.ToString("dd/MMM/yyyy") + ".pdf");
            Response.TransmitFile(Server.MapPath("~/Uploads/pdf/") + result.Replace("htm", "pdf"));
            Response.Flush();

            //return Redirect(path);
            return null;
        }

        private void AlertMesssage(string ToMail, string Subject, string body, string filepath, string newfilename)
        {
            string[] Attechment = new string[] { filepath };
           
            //UserCache.Email
            //MailMessage msg = new MailMessage("wwwsmtp@dotsquares.com", UserCache.Email, Subject, body);
            //System.Net.Mail.Attachment attachment;
            //attachment = new System.Net.Mail.Attachment(filepath);
            string dropboxpath = FileToDropbox(filepath, newfilename);
            //msg.Body = "Dropbox URL :" + dropboxpath;
            //msg.Attachments.Add(attachment);
            // msg.IsBodyHtml = true;
           // Roster.Business.Generic.MailClass.Send(msg);

            SendMessage.SendEmail(ToMail, Subject, "Dropbox URL :" + dropboxpath, Attechment);
        }
        private Stream TestStream(string path)
        {
            Stream fs = System.IO.File.OpenRead(path);
            return fs;
        }
        private string FileToDropbox(string Path, string newfilename)
        {
            string returnUrl = string.Empty;
            var token = "33a1Qd8VE_AAAAAAAAAACGn1PZCgadgHhEbCx8A18rhZWpNkXdI0GSQ3wGAoXH3h";
            string newpath = Path;
            string serverPath = Path;
            System.IO.MemoryStream data = new System.IO.MemoryStream();
            System.IO.Stream str = TestStream(newpath);

            str.CopyTo(data);
            data.Seek(0, SeekOrigin.Begin); // <-- missing line
            byte[] buf = new byte[data.Length];
            data.Read(buf, 0, buf.Length);

            // you can upload to any folder:
            // serverPath = "/folder_name/" + System.IO.Path.GetFileName(FileUpload1.FileName);
            // folder_name - should exist in dropbox

            var result = OAuthUtility.Put
            (
              "https://api-content.dropbox.com/1/files_put/auto/",
              new HttpParameterCollection
        { 
          { "access_token", token },
          { "overwrite", "true" },
          { "path", newfilename },
          { data } 
        }
            );

            if (result.StatusCode != 200)
            {
                // error
                Response.Write(result["error"].ToString());
            }
            else
            {
                // ok
                returnUrl = String.Format("https://api-content.dropbox.com/1/files/auto{0}?access_token={1}", result["path"], token);
            }

            return returnUrl;
        }

        #endregion

        #region Manage Employee
        /// <summary>
        /// export eployee according to selected employee or location
        /// </summary>
        /// <param name="IsEmployee">bool</param>
        /// <param name="EmployeeId">long</param>
        /// <param name="Location">int</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ExportEmployee(bool IsEmployee, long EmployeeId = 0, int Roster = 0)
        {
            var result = EmployeeRepository.GetAllUserList(IsEmployee, EmployeeId, Roster, Convert.ToInt64(UserCache.CompanyId));
            string contextIdStr = Request.Form["hdnContextId"];
            int contextId = 0;
            Int32.TryParse(contextIdStr, out contextId);
            var excelBytes = result.ToExcelBytes<AllEmployee>();
            var fileName = string.Format("Employee {0}-{1}.xlsx", contextId, DateTime.Now.ToString("MMddyyyyHHmmssfff"));
            return excelBytes != null ? File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName) : null;
        }


        /// <summary>
        /// for download in pdf
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ExportEmployeePdf(bool IsEmployee, long EmployeeId = 0, int Roster = 0)
        {
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
            string basepath = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            EmployeeRepository objemprepo = new EmployeeRepository();
            var parameter = baseUrl + "Users/ExportStaffPdf?IsEmployee=" + IsEmployee.ToString().ToLower() + "&EmployeeId=" + EmployeeId + "&Roster=" + Roster + "&CompanyId=" + UserCache.CompanyId;
            var result = objemprepo.ExportStaffPdf("~/Users/QRCode", Server.MapPath("~/Upload"), parameter);

            string path = basepath + "/Upload/" + result.Replace("htm", "pdf");


            Response.ContentType = "Application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + "Employee_" + DateTime.Now.Date.ToString("dd/MMM/yyyy") + ".pdf");
            Response.TransmitFile(Server.MapPath("~/Upload/") + result.Replace("htm", "pdf"));
            Response.Flush();

            //return Redirect(path);
            return null;
        }

        /// <summary>
        /// get list for download in pdf
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ExportStaffPdf(bool IsEmployee, long EmployeeId = 0, int Roster = 0, long CompanyId = 0)
        {

            var data = EmployeeRepository.GetAllUserList(IsEmployee, EmployeeId, Roster, CompanyId);
            return PartialView(data);
        }
        /// <summary>
        /// for get export staff module
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ExportStaff()
        {
            return View();
        }
        public ActionResult OnlineMembersChat()
        {

            return View();
        }
        /// <summary>
        /// get all employee
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Employee()
        {
            return View();
        }


        [HttpGet]
        public ActionResult EmployeeList(string SearchText)
        {
            return PartialView("_EmployeeList", EmployeeRepository.GetAllUserEmployee(SearchText));
        }


        [HttpGet]
        public ActionResult ManageEmployee()
        {

            return View(EmployeeRepository.GetEmployee());
        }
        /// <summary>
        /// add new employee
        /// </summary>
        /// <param name="model">EmployeeCompositModel</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ManageEmployee(EmployeeCompositModel model)
        {
            if (ModelState.IsValid)
            {
                var attachedFile = System.Web.HttpContext.Current.Request.Files["ProfilePic"];

                if (attachedFile != null && attachedFile.ContentLength > 0)
                {
                    string ext = Path.GetExtension(attachedFile.FileName);

                    String UUID = Guid.NewGuid().ToString();

                    string path = System.IO.Path.Combine(Server.MapPath("~/Upload/"), UUID + ext);
                    attachedFile.SaveAs(path);

                    model.picture = UUID + ext;
                }
                model.Role = "User";
                model.CompanyId = Convert.ToInt64(UserCache.CompanyId);
                var result = EmployeeRepository.AddEmployee(model);
                if (result > 0)
                {
                    string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                    string Token = MyExtensions.Encrypt(result.ToString());
                    string Link = "<a href='" + baseUrl + "Account/CreatePassword?token=" + Token + "'>Click here</a>";
                    var path=gFolder + @"\UserCreateEmail.html";
                    EmployeeRepository.SendMailToCreatePassword(model.Email, model.Name, Link, path);

                }
                return RedirectToAction("Employee");
            }
            return RedirectToAction("Employee");

        }

        [HttpGet]
        public ActionResult EditEmployee(int Id)
        {
            EmployeeRepository obj = new EmployeeRepository();
            var result = EmployeeRepository.GetEmployeeDetails(Id);
            return PartialView(result);
        }
        /// <summary>
        /// add new employee
        /// </summary>
        /// <param name="model">EmployeeCompositModel</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditEmployee(EmployeeCompositModel model)
        {
            var attachedFile = System.Web.HttpContext.Current.Request.Files["ProfilePic"];
            model.CompanyId = Convert.ToInt64(UserCache.CompanyId);
            if (attachedFile != null && attachedFile.ContentLength > 0)
            {
                string ext = Path.GetExtension(attachedFile.FileName);

                String UUID = Guid.NewGuid().ToString();
                string path = System.IO.Path.Combine(Server.MapPath("~/Upload/"), UUID + ext);
                attachedFile.SaveAs(path);
                model.picture = UUID + ext;
            }
            var result = EmployeeRepository.UpdateEmployee(model);
            return RedirectToAction("Employee");

        }

        [HttpGet]
        public ActionResult ImportStaff()
        {

            return View();
        }

        /// <summary>
        /// get employee detail
        /// </summary>
        /// <param name="Id">int</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _UpdateEmployee(int Id)
        {
            EmployeeRepository obj = new EmployeeRepository();

            var result = EmployeeRepository.GetEmployeeDetails(Id);
            if (Request.Cookies["subMenu"] != null)
            {
                Response.Cookies["subMenu"].Expires = DateTime.Now.AddDays(-1);
            }
            return PartialView("_UpdateEmployee", result);
        }
        /// <summary>
        /// Update employee
        /// </summary>
        /// <param name="model">EmployeeCompositModel</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateEmployee(EmployeeCompositModel model)
        {
            var attachedFile = System.Web.HttpContext.Current.Request.Files["ProfilePic"];

            if (attachedFile != null && attachedFile.ContentLength > 0)
            {
                string ext = Path.GetExtension(attachedFile.FileName);

                String UUID = Guid.NewGuid().ToString();
                string path = System.IO.Path.Combine(Server.MapPath("~/Upload/"), UUID + ext);
                attachedFile.SaveAs(path);
                model.picture = UUID + ext;
            }

            var result = EmployeeRepository.UpdateEmployee(model);
            if (Request.Cookies["subMenu"] != null)
            {
                Response.Cookies["subMenu"].Expires = DateTime.Now.AddDays(-1);
            }
            return RedirectToAction("Employee");
        }
        /// <summary>
        /// delete employee
        /// </summary>
        /// <param name="Id">int</param>
        /// <returns></returns>
        public ActionResult DeleteEmployee(int Id)
        {
            var result = EmployeeRepository.DeleteEmployee(Id);
            if (result > 0)
            {
                Session["IsDeleted"] = "True";
            }
            else
            {
                Session["IsDeleted"] = null;

            }

            return RedirectToAction("Employee");
        }
        /// <summary>
        /// add bulk employee from csv file
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public  ActionResult InsertEmpFromCsv()
        {
            var attachedFile = System.Web.HttpContext.Current.Request.Files["CsvDoc"];

            if (attachedFile == null)
            {
                return Content("Csv has no data.");
            }
            string ids = "";
            List<string> myStringColumn = new List<string>();
            ICsvParser csvParser = new CsvParser(new StreamReader(attachedFile.InputStream));
            CsvReader csvReader = new CsvReader(csvParser);

            int count = 0;
            try
            {
                var path = gFolder + @"\UserCreateEmail.html";
                EmployeeCompositModel modelCheck = new EmployeeCompositModel();
                while (csvReader.Read())
                {

                    EmployeeCompositModel objInsertModel = new EmployeeCompositModel();
                    objInsertModel.Email = csvReader.GetField<string>("Email");
                    objInsertModel.Phone = csvReader.GetField<string>("Phone");
                    if (!EmployeeRepository.CheckMobileAndEmailExists(0, objInsertModel.Email, modelCheck.Phone))
                    {

                        objInsertModel.Name = csvReader.GetField<string>("FirstName");
                        objInsertModel.LastName = csvReader.GetField<string>("LastName");
                        objInsertModel.Role = csvReader.GetField<string>("Role");
                        objInsertModel.City = csvReader.GetField<string>("City");
                        var Qualification = csvReader.GetField<string>("Qualification");
                      //  objInsertModel.Location = csvReader.GetField<string>("Location");
                        objInsertModel.Address = csvReader.GetField<string>("Address");
                        objInsertModel.Dob = csvReader.GetField<string>("Dob");


                        objInsertModel.Grade = csvReader.GetField<string>("Grade");
                        objInsertModel.LicenceNumber = csvReader.GetField<string>("LicenceNumber");
                        objInsertModel.EmgContactName = csvReader.GetField<string>("EmergContactName");
                        objInsertModel.EmgContact1 = csvReader.GetField<string>("EmergContactNo");
                        objInsertModel.illnesses = csvReader.GetField<string>("Illnesses");
                        objInsertModel.BasePrice = Convert.ToDecimal(csvReader.GetField<string>("BasePrice"));
                        objInsertModel.CompanyId = Convert.ToInt64(UserCache.CompanyId);
                        List<EmployeeQualification> list = new List<EmployeeQualification>();
                        if (string.IsNullOrEmpty(Qualification))
                        {
                            Qualification = "";
                        }
                        string[] arr = Qualification.Split(',');
                        for (int i = 0; i < arr.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(arr[i])))
                            {
                                list.Add(new EmployeeQualification { Qualification = arr[i].ToString() });
                            }

                        }
                        objInsertModel.ListQualification = list;
                        if (!EmployeeRepository.CheckMobileAndEmailExists(0, objInsertModel.Email, null))
                        {
                            if (!EmployeeRepository.CheckMobileAndEmailExists(0, null, objInsertModel.Phone))
                            {
                                var result = EmployeeRepository.AddEmployee(objInsertModel);
                                if (result != 0)
                                {
                                        count = count + 1;                                   
                                        string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                                        string Token = MyExtensions.Encrypt(result.ToString());
                                        string Link = "<a href='" + baseUrl + "Account/CreatePassword?token=" + Token + "'>Click here</a>";
                                        
                                        
                                        EmployeeRepository.SendMailToCreatePassword(objInsertModel.Email, objInsertModel.Name, Link, path);
                                     
                                }

                            }
                        }
                    }

                }
                return Content(count.ToString());
            }
            catch (Exception ex)
            {
                return Content("0");
            }

            return Content("0");

        }
        /// <summary>
        /// check phone number or email are exist or not
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult CheckMobileAndEmailExists(EmployeeCompositModel model)
        {
            var result = EmployeeRepository.CheckMobileAndEmailExists(model.Id, model.Email, model.Phone);
            if (result)
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult IsUserOnQuickBlox(string email)
        {
            var result = EmployeeRepository.GetPwd(email);
            return Json(result, JsonRequestBehavior.AllowGet);          

        }


        #endregion

        #region managetab cookes
        /// <summary>
        /// get user is manager or admin
        /// </summary>
        /// <returns></returns>
        public string GetRoleForAdminManager()
        {
            string t = Request.Url.ToString();
            string Role = "Admin";
            if (t.Contains("Manager"))
            {
                Role = "Manager";
            }
            return Role;

        }
        /// <summary>
        /// get user roleid
        /// </summary>
        /// <returns></returns>
        public int GetUserRoleForManage()
        {
            int RoleId = 4;
            if (GetRole() == "Manager")
            {
                RoleId = 3;
            }
            return RoleId;
        }
        /// <summary>
        /// set role cookes 
        /// </summary>
        /// <param name="Value"></param>
        public void SetCookees(string Value)
        {
            HttpCookie cookie = Request.Cookies["Preferences"];
            if (cookie == null)
            {
                cookie = new HttpCookie("Preferences");
            }

            cookie["Role"] = Value;
            cookie.Expires = DateTime.Now.AddYears(1);
            Response.Cookies.Add(cookie);


        }
        /// <summary>
        /// get role name
        /// </summary>
        /// <returns></returns>
        public string GetRole()
        {
            HttpCookie cookie = Request.Cookies["Preferences"];
            if (cookie == null)
            {
                cookie = new HttpCookie("Preferences");
            }

            return cookie["Role"];


        }
        public string GetMenuCookies()
        {
            HttpCookie cookie = Request.Cookies["Preferences"];
            if (cookie == null)
            {
                cookie = new HttpCookie("Preferences");
            }

            return Request.Cookies["subMenu"].Value;


        }
        #endregion

        [HttpPost]
        public ActionResult ExportToPDF(string BaseString)
        {
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




            Response.ContentType = "Application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + "QrCode_" + DateTime.Now.Date.ToString("dd/MMM/yyyy") + ".pdf");
            Response.TransmitFile(Server.MapPath("~/Upload/") + pdf);
            Response.Flush();


            //ScriptManager.RegisterStartupScript(this, this.GetType(), "Js_script", "<script type=\"text/javascript\"> $('#loading').hide();$('.ajax-loader').hide(); </script>", false);
            string filename = string.Format("{0:ddMMMyyyy}", DateTime.UtcNow.Date.ToString()) + ".pdf";
            string newfilename = filename;
            string filewrite = BasePath() + "/Upload/" + pdf;
            List<Workerlst> obj = new List<Workerlst>();
            obj.Add(new Workerlst { URL = filewrite });
            var details = obj.ToArray();
            return Json(details, JsonRequestBehavior.AllowGet);
        }
        public String BasePath()
        {
            return String.Format("http://{0}{1}", Request.ServerVariables["HTTP_HOST"], (Request.ApplicationPath.Equals("/") ? String.Empty : Request.ApplicationPath));
        }
    }
    public class Workerlst
    {
        public string URL { get; set; }
    }
}