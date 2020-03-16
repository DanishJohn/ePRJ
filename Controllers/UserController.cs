using ePRJ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows;
using System.Security;
using System.Web.Security;
using System.Net.Http;

namespace ePRJ.Controllers
{
    [MyCustomAuthorize(LoginPage ="~/Home/Login", Roles = "Customer")]
    public class UserController : Controller
    {
        HttpClient client = new HttpClient();
        public UserController()
        {
            client.BaseAddress = new Uri("http://localhost/Data/");
        }
        [HttpGet]
        public ActionResult Profiles()
        {
                var username = System.Web.HttpContext.Current.User.Identity.Name;
                var user = client.GetAsync(client.BaseAddress + "api/Account/GetUser/" + username + "/").Result.Content.ReadAsAsync<Account>().Result;
                return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Profiles(Account user)
        {
            if (ModelState.IsValid)
            {
                var task = client.PutAsJsonAsync(client.BaseAddress + "api/Account/", user);
                if (task.Result.IsSuccessStatusCode)
                {
                    ViewBag.Message = "Edit Successful";
                }
                return View(user);
            }
            else
                return View(user);
        }

    }
}
