using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using ePRJ.Models;
using System.IO;
using System.Threading.Tasks;
using System.Web.Security;

namespace ePRJ.Controllers

{
    [MyCustomAuthorize(LoginPage = "~/Admin/Login", Roles = "Admin, Employee")]
    public class AdminController : Controller
    {
        // GET: Admin
        private string dataURL = "http://localhost/Data/";
        HttpClient client = new HttpClient();
        public AdminController()
        {
            client.BaseAddress = new Uri(dataURL);
        }
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string mail, string pass)
        {
            var users = client.GetAsync(client.BaseAddress + "api/Account/GetAllUsers/").Result.Content.ReadAsAsync<List<Account>>().Result;
            var validate = users.Exists(b => b.UserEmail == mail && b.UserPassword == pass);
            if (validate && (Roles.IsUserInRole(mail,"Admin") || Roles.IsUserInRole(mail,"Employee")))
            {
                FormsAuthentication.SetAuthCookie(mail, true);
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                ViewBag.UnAuthorized = "You are either not authorized to see this page, or your account does not exist, your password and username were wrong";
                 return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Admin");
        }
        public ActionResult Car()
        {
            var dataVehicle = client.GetAsync(dataURL + "api/Vehicle/");
            var res = dataVehicle.Result.Content.ReadAsAsync<List<Vehicle>>().Result;
            ViewBag.Message = TempData["Message"];
            return View(res);
        }
        public ActionResult CreateCar()
        {
            var idList = new List<SelectListItem>();
            var getReceipts = client.GetAsync(dataURL + "api/Receipt/");
            var Receipts = getReceipts.Result.Content.ReadAsAsync<List<Receipts>>().Result;
            var getcar = client.GetAsync(dataURL + "api/Vehicle/");
            var addedCarList = getcar.Result.Content.ReadAsAsync<List<Vehicle>>().Result;
            var missingCarList = Receipts.Where(item => addedCarList.All(b => b.vehicle_id != item.vehicle_id));
            foreach(var car in missingCarList)
            {
                idList.Add(new SelectListItem { Text = car.vehicle_id, Value = car.vehicle_id });
            }
            ViewBag.idList = idList.AsEnumerable();
            return View();
        }
        
        [HttpPost]
        public ActionResult CreateCar(Vehicle vehicle, HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
                var savePath = Path.Combine(Server.MapPath("~/Images/"), file.FileName);
                vehicle.img_path = savePath;
                var postedCar = client.PostAsJsonAsync(dataURL + "api/Vehicle/", vehicle);
                if (postedCar.Result.IsSuccessStatusCode)
                {
                    ChangeStatus(vehicle.vehicle_id);
                    file.SaveAs(savePath);
                    TempData["Message"] = "Created";
                    return RedirectToAction("Car", "Admin");
                }
            }
            TempData["Message"] = "Failed";
            return RedirectToAction("Car", "Admin");
        }
        public async Task<ActionResult> Delete(string id)
        {
            var data = client.GetAsync(client.BaseAddress + "/api/Vehicle/" + id).Result.Content.ReadAsAsync<Vehicle>().Result;
            var filename = Path.GetFileName(data.img_path);
            var serverpath = Server.MapPath("~/Images/" + filename);
            if (System.IO.File.Exists(serverpath))
            {
                System.IO.File.Delete(serverpath);
            }
            var deleted = await client.DeleteAsync(client.BaseAddress + "/api/Vehicle/" + id);
            if (deleted.IsSuccessStatusCode)
            {

                
                TempData["Message"] = "Deleted";
            }
            else
            {
                TempData["Message"] = "Failed";
            }
            return RedirectToAction("Car", "Admin");
        }
        public ActionResult CarDetails(string id)
        {
            var car = client.GetAsync(client.BaseAddress + "/api/Vehicle/" + id);
            if (car.Result.IsSuccessStatusCode)
            {
                var data = car.Result.Content.ReadAsAsync<Vehicle>().Result;
                return View(data);
            }
            return RedirectToAction("Car","Admin");
        }
        public ActionResult EditCar(string id)
        {
            var car = client.GetAsync(client.BaseAddress + "api/Vehicle/" + id).Result.Content.ReadAsAsync<Vehicle>().Result;
            return View(car);
        }

        [HttpPost]
        public ActionResult EditCar(Vehicle car, HttpPostedFileBase file)
        {
            if (file != null)
            {
                if (System.IO.File.Exists(Server.MapPath("~/Images/" + Path.GetFileName(car.img_path))))
                {
                    System.IO.File.Delete(Server.MapPath("~/Images/" + Path.GetFileName(car.img_path)));
                }
                var filepath = Server.MapPath("~/Images/" + file.FileName);
                file.SaveAs(filepath);
                car.img_path = filepath;
                var putData = client.PutAsJsonAsync(client.BaseAddress + "api/Vehicle/", car);
                if (!putData.Result.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Error editing Car's detail";
                }
                return RedirectToAction("EditCar", car);
            }
            var Data = client.PutAsJsonAsync(client.BaseAddress + "api/Vehicle/", car);
            if (!Data.Result.IsSuccessStatusCode) 
            {
                ViewBag.Error = "Error editing Car detail";
            }
            return View(car);
        }

        public ActionResult Receipts()
        {
            var receipts = client.GetAsync(client.BaseAddress + "api/Receipt/");
            var extracted = receipts.Result.Content.ReadAsAsync<List<Receipts>>().Result;
            if (TempData["status-message"] != null)
            {
                ViewBag.Message = TempData["status-message"];
            }
            return View(extracted);
        }
        public ActionResult Accounts()
        {
            var accounts = client.GetAsync(client.BaseAddress + "api/Account/GetAllUsers");
            var extracted = accounts.Result.Content.ReadAsAsync<List<Account>>().Result;
            if (TempData["error"] != null)
            {
                ViewBag.Error = TempData["error"]; 
            }
            return View(extracted);
        }
        [MyCustomAuthorize(LoginPage = "~/Admin/Login",Roles ="Admin")]
        public ActionResult CreateAccount()
        {
            var roleList = new List<SelectListItem>()
            {
               new SelectListItem() {Text = "Customer", Value = "Customer"},
               new SelectListItem {Text = "Employee", Value = "Employee"},
               new SelectListItem {Text = "Admin", Value = "Admin"}
            };
            ViewBag.Roles = roleList.AsEnumerable();
            return View();
        }
        [HttpPost]
        public ActionResult CreateAccount(Account acc) 
        {
            acc.UserStatus = true;
            var action = client.PostAsJsonAsync(client.BaseAddress + "api/Account/", acc).Result;
            if (!action.IsSuccessStatusCode)
            {
                var errormsg = action.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                ViewBag.Message = errormsg;
                var roleList = new List<SelectListItem>()
            {
               new SelectListItem() {Text = "Customer", Value = "Customer"},
               new SelectListItem {Text = "Employee", Value = "Employee"},
               new SelectListItem {Text = "Admin", Value = "Admin"}
            };
                ViewBag.Roles = roleList.AsEnumerable();
                return View();
            }
            return RedirectToAction("Accounts", "Admin");
        }
        [MyCustomAuthorize(Roles =("Admin"),LoginPage = "~/Admin/Login")]
        public ActionResult DisableAcc(string id)
        {
            var statsData = client.GetAsync(client.BaseAddress + "api/Account/GetUser/" + id + "/").Result.Content.ReadAsAsync<Account>().Result;
            if (statsData.UserStatus)
                statsData.UserStatus = false;
            else
                statsData.UserStatus = true;
            var putTask = client.PutAsJsonAsync(client.BaseAddress + "api/Account/", statsData);
            if (!putTask.Result.IsSuccessStatusCode)
            {
                TempData["error"] = "Error changing status";
            }
            return RedirectToAction("Accounts", "Admin");
        }
        public ActionResult CreateReceipt()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateReceipt(Receipts receipt)
        {
            var data = client.PostAsJsonAsync(client.BaseAddress + "api/Receipt/", receipt);
            if (data.Result.IsSuccessStatusCode)
            {
                return RedirectToAction("Receipts", "Admin");
            }
            ViewBag.Message = "Failed to Create";
            return View();
        }

        public ActionResult ChangeStats(string id)
        {
            ChangeStatus(id);
            return RedirectToAction("Receipts", "Admin");
        }
        public void ChangeStatus(string id)
        {
            var receipt = client.GetAsync(client.BaseAddress + "api/Receipt/" + id);
            var data = receipt.Result.Content.ReadAsAsync<Receipts>().Result;
            
            if (data.receipt_status)
            {
                data.receipt_status = false;
                var status = client.PutAsJsonAsync(client.BaseAddress + "api/Receipt/", data).Result.IsSuccessStatusCode;

            }
            else
            {
                data.receipt_status = true;
                var status =  client.PutAsJsonAsync(client.BaseAddress + "api/Receipt/", data).Result.IsSuccessStatusCode;
            }
            
        }
        public ActionResult Service()
        {
            var data = client.GetAsync(client.BaseAddress + "api/Service/").Result;
            var extracted = data.Content.ReadAsAsync<List<Service>>().Result;
            return View(extracted);
        }
        public ActionResult CreateService()
        {
            var customers = client.GetAsync(client.BaseAddress + "api/Account/").Result.Content.ReadAsAsync<List<Account>>().Result;
            var custList = new List<SelectListItem>();
            foreach (var item in customers)
            {
                custList.Add(new SelectListItem() { Text = item.UserEmail, Value = item.UserEmail });
            }
            var idList = new List<SelectListItem>();
            var getcar = client.GetAsync(dataURL + "api/Vehicle/");
            var addedCarList = getcar.Result.Content.ReadAsAsync<List<Vehicle>>().Result;
            foreach (var car in addedCarList)
            {
                idList.Add(new SelectListItem { Text = car.vehicle_id, Value = car.vehicle_id });
            }
            ViewBag.idList = idList.AsEnumerable();
            ViewBag.CustList = custList.AsEnumerable();
            ViewBag.StatList = new List<SelectListItem>()
            {
                new SelectListItem() {Text = "Ready", Value = "true"},
                new SelectListItem() {Text = "Not Ready" , Value = "false"}
            };
            return View();
        }
        [HttpPost]
        public ActionResult CreateService(Service ticket)
        {
            var posted = client.PostAsJsonAsync(client.BaseAddress + "api/Service", ticket).Result;
            if (!posted.IsSuccessStatusCode)
            {
                ViewBag.Error = "Error creating Ticket";
            }
            return RedirectToAction("Service");
        }
        public ActionResult DeleteService(string id)
        {
            var del = client.DeleteAsync(client.BaseAddress + "api/Service/" + id);
            if (!del.Result.IsSuccessStatusCode)
            {
                ViewBag.Error = "Error deleting";
            }
            return RedirectToAction("Service", "Admin");
        }

        public ActionResult EditService(int id)
        {
            var data = client.GetAsync(client.BaseAddress + "api/Service/" + id).Result.Content.ReadAsAsync<Service>().Result;
            var customers = client.GetAsync(client.BaseAddress + "api/Account/").Result.Content.ReadAsAsync<List<Account>>().Result;
            var custList = new List<SelectListItem>();
            foreach (var item in customers)
            {
                custList.Add(new SelectListItem() { Text = item.UserEmail, Value = item.UserEmail });
            }
            var idList = new List<SelectListItem>();
            var getcar = client.GetAsync(dataURL + "api/Vehicle/");
            var addedCarList = getcar.Result.Content.ReadAsAsync<List<Vehicle>>().Result;
            foreach (var car in addedCarList)
            {
                idList.Add(new SelectListItem { Text = car.vehicle_id, Value = car.vehicle_id });
            }
            ViewBag.idList = idList.AsEnumerable();
            ViewBag.CustList = custList.AsEnumerable();
            return View(data);
        }
        
        public ActionResult Purchase()
        {
            var data = client.GetAsync(client.BaseAddress + "api/Purchase/").Result.Content.ReadAsAsync<List<Bill>>().Result;
            return View(data);
        }
        public ActionResult GenerateBillForm()
        {
            var idList = new List<SelectListItem>();
            var getcar = client.GetAsync(dataURL + "api/Vehicle/");
            var addedCarList = getcar.Result.Content.ReadAsAsync<List<Vehicle>>().Result;
            foreach (var car in addedCarList)
            {
                idList.Add(new SelectListItem { Text = car.vehicle_id, Value = car.vehicle_id });
            }
            ViewBag.idList = idList.AsEnumerable();
            ViewBag.Error = TempData["error"];
            return View();
        }
        [HttpPost]
        public ActionResult GenerateBillForm(string id)
        {
            var car = client.GetAsync(dataURL + "api/Vehicle/" + id).Result;
            if (car.IsSuccessStatusCode)
            {
                var data = car.Content.ReadAsAsync<Vehicle>().Result;
                TempData["bill"] = data;
                return RedirectToAction("CreateBill");
            }
            return View();
        }
        public ActionResult CreateBill()
        {
                var billData = (Vehicle)TempData["bill"];
            string[] data =
            {
                billData.vehicle_id,
                billData.vehicle_name,
                billData.vehicle_brand,
                Convert.ToString(billData.vehicle_price),
                Convert.ToString(billData.vehicle_stock)
            };
            ViewBag.BillData = data;
            return View();
        }

        [HttpPost]
        public ActionResult CreateBill(Bill bill)
        {
            if (ModelState.IsValid)
            {
                var postData = client.PostAsJsonAsync(client.BaseAddress + "api/Purchase/", bill);
                if (!postData.Result.IsSuccessStatusCode)
                {
                    TempData["error"] = "Error issuing Bill";
                    return RedirectToAction("GenerateBillForm");
                }
                var carGet = client.GetAsync(client.BaseAddress + "api/Vehicle/" + bill.vehicle_id).Result.Content.ReadAsAsync<Vehicle>().Result;
                carGet.vehicle_stock = carGet.vehicle_stock - (int)bill.quantity;
                var postedCarData = client.PutAsJsonAsync(client.BaseAddress + "api/Vehicle/", carGet);
                return RedirectToAction("Purchase");
            }
            return RedirectToAction("GenerateBillForm");
        }
        
        public ActionResult Reports()
        {
            return View();
        }

        public ActionResult StockReport()
        {
            var data = client.GetAsync(client.BaseAddress + "api/Vehicle/").Result.Content.ReadAsAsync<List<Vehicle>>().Result;
            return View(data);
        }
        public ActionResult CustomerReport()
        {
            var custData = client.GetAsync(client.BaseAddress + "api/Account/").Result.Content.ReadAsAsync<List<Account>>().Result;
            return View(custData);
        }
        public ActionResult VehicleReport()
        {
            var data = client.GetAsync(client.BaseAddress + "api/Vehicle/").Result.Content.ReadAsAsync<List<Vehicle>>().Result;
            return View(data);
        }

        public ActionResult WaitingList()
        {
            var data = client.GetAsync(client.BaseAddress + "api/Service/").Result.Content.ReadAsAsync<List<Service>>().Result;
            var waiting = data.Count(w => !w.service_status);
            ViewBag.Waiting = waiting;
            return View(data);
        }
        
    }
}