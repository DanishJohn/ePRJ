using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using ePRJ.Models;
using System.Web.Security;

namespace ePRJ.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        private string dataUrl = "http://localhost/Data/";
        HttpClient client = new HttpClient();
        public ActionResult Index()
        {
            var vehicledata = client.GetAsync(dataUrl + "api/Vehicle/");
            var extracted = vehicledata.Result.Content.ReadAsAsync<List<Vehicle>>().Result;
            return View(extracted);
        }
        public ActionResult Product()
        {
            var vehicles = client.GetAsync(dataUrl + "api/Vehicle/");
            var extracted = vehicles.Result.Content.ReadAsAsync<List<Vehicle>>().Result;
            if (TempData["data"] != null)
            {
                return View(TempData["data"]);
            }
            return View(extracted);
        }
        public ActionResult Search(string vehicleName, string vehicleBrand,string minimum, string maximum)
        {
            int min = Convert.ToInt32(minimum);
            int max = Convert.ToInt32(maximum);
            var vehicles = client.GetAsync(dataUrl + "api/Vehicle/");
            var extracted = vehicles.Result.Content.ReadAsAsync<List<Vehicle>>().Result;
            if (vehicleName != "" && vehicleBrand == "")
            {
                var searched = extracted.Where(b => b.vehicle_name.Contains(vehicleName));
                TempData["data"] = searched;
                return RedirectToAction("Product");
            }
            else if (vehicleName == "" && vehicleBrand != "")
            {
                var searched = extracted.Where(b => b.vehicle_brand.Contains(vehicleBrand));
                TempData["data"] = searched;
                return RedirectToAction("Product");
            }
            else
            {
                var searched = extracted.Where(b => b.vehicle_brand.Contains(vehicleBrand) && b.vehicle_name.Contains(vehicleName));
;                TempData["data"] = searched;
                return RedirectToAction("Product");
            }
        }
        public ActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LogIn(string mail, string pass) {
            var acc = client.GetAsync(dataUrl + "api/Account/GetCustomer/" + mail + "/").Result;
            if (!acc.IsSuccessStatusCode)
            {
                ViewBag.Message = "This Email does not exsist.";
                return View();
            }
            var data = acc.Content.ReadAsAsync<Account>().Result;
            if (!(data.UserPassword == pass))
            {
                ViewBag.Message = "Your Password is incorrect";
                return View();
            } else
            {
                if (!data.UserStatus)
                {
                    ViewBag.Message = "Your Account has been deactivated.";
                    return View();
                }
                FormsAuthentication.SetAuthCookie(mail, true);
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult SignUp()
        {
            return View();
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public ActionResult SignUp(Account posted)
        {
            posted.UserRole = "Customer";
            posted.UserStatus = true;
            var action = client.PostAsJsonAsync(dataUrl + "api/Account/", posted).Result;
            if (!action.IsSuccessStatusCode)
            {
                var errormsg = action.Content.ReadAsStringAsync().GetAwaiter().GetResult() ;
                ViewBag.Message = errormsg;
                return View();
            }
            return RedirectToAction("LogIn","Home");
        }
    }
}