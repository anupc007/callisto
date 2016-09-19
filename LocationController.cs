using Roster.Business.Factory.Repository;
using Roster.Business.Factory.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Roster.Web.App_Start;
using Roster.Business;

namespace Roster.Web.Controllers
{
   
    /// <summary>
    /// location module
    /// </summary>
    [CustomAuthorize("Administration", "Company")]
    public class LocationController : Controller
    {
        #region Location
        LocationRepository objRepo;
        /// <summary>
        /// LocationController constractor 
        /// </summary>
        public LocationController()
        {
            objRepo = new LocationRepository();
        }
        /// <summary>
        /// Render location module
        /// </summary>
        /// <returns></returns>
        public ActionResult Location()
        {
            return View();
        }
        /// <summary>
        /// Use for get location list
        /// </summary>
        /// <param name="SearchText">string</param>
        /// <returns>LocationList</returns>
        public ActionResult LocationList(string SearchText="")
        {
            return View(objRepo.GetLocationList(SearchText.Trim(), Convert.ToInt64(UserCache.UserId)));
        }
        /// <summary>
        /// get location detail for selected location 
        /// </summary>
        /// <param name="locationId">long?</param>
        /// <returns>View</returns>
        public ActionResult ManageLocation(long? locationId=0)
        {
            return View(objRepo.GetLocation(locationId.Value));
        }
        /// <summary>
        /// Add edit location 
        /// </summary>
        /// <param name="model">LocationModel</param>
        /// <returns>view</returns>
        [HttpPost]
        public ActionResult ManageLocation(LocationModel model)
        {
            model.CompanyId = Convert.ToInt64(UserCache.CompanyId);
            model.CreatedByUserId = Convert.ToInt64(UserCache.UserId);         
            var data= objRepo.AddUpdateLocation(model);
            if (data > 0)
             {
                  ClearMenuSelectCookes();              
                  return RedirectToAction("Location");
             }
            return View();
        }
        /// <summary>
        /// Delete selected location 
        /// </summary>
        /// <param name="id">int</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id)
        {          
            var result = objRepo.DeleteLocation(id);
            if (result >0)
            {
                return Json("Success");
            }
            else
            {
                return Json("Someone has tampered something");
            }

        }
        /// <summary>
        /// check location is exist or not 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult IsLocationExist(LocationModel model)
        { 
            return string.IsNullOrEmpty(objRepo.IsLocationExist(model)) ? Json(true, JsonRequestBehavior.AllowGet) : Json(false, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Clear menu selection 
        /// </summary>
        public void ClearMenuSelectCookes()
        {
            if (Request.Cookies["subMenu"] != null)
            {
                Response.Cookies["subMenu"].Expires = DateTime.Now.AddDays(-1);
            }
        }
        #endregion
    }

}