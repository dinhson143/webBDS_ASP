using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using WebBDS.Common;
using WebBDS.Models;

namespace WebBDS.Controllers
{
    public class CustomerController : Controller
    {
        public string userID { get; set; }
        // GET: User

        [Route("Customer")]
        public ActionResult Index()
        {
            if (CommonConstants.listUser.Count == 0)
            {
                List<User> listus = CommonConstants.getlistUser();
                CommonConstants.listUser = listus;
            }

            // listBDS
            if (CommonConstants.listBDS.Count == 0)
            {
                List<BDS> listbds = CommonConstants.getlistBDS();
                CommonConstants.listBDS = listbds;
            }

            // listBDSmark
            if (CommonConstants.listBDSmark.Count == 0)
            {
                List<BDSmark> listbdsmark = CommonConstants.getlistBDSmark();
                CommonConstants.listBDSmark = listbdsmark;
            }


            if (CommonConstants.listLich.Count == 0)
            {
                List<Lich> listLich = CommonConstants.getlistLich();
                CommonConstants.listLich = listLich;
            }


            userID = (string)Session[CommonConstants.USER_SESSION];
            string token = (string)Session[CommonConstants.AccessToken_SESSION];
            if (CommonConstants.User.Email != userID)
            {
                List<User> getUser = CommonConstants.getlistUserID(userID, token);
                if (getUser.Count > 0)
                {
                    CommonConstants.User = getUser[0];
                }
            }
            return View(CommonConstants.User);
        }


        [HttpPost]
        public String Edit(string id,string idIF, string name, string email, string address,string phone, string password, string role)
        {
            tempUser user = new tempUser();
            Dictionary<string, object> data = new Dictionary<string, object>();


            user.Address = address;
            user.Email = email;
            user.Name = name;
            user.Phone = phone;
            user.Role = role;
            user.Password = password;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL+"user/" +id);
                var postJob = client.PutAsJsonAsync<tempUser>("",user);
                postJob.Wait();

                var postResult = postJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    using (var client2 = new HttpClient())
                    {
                        client2.BaseAddress = new Uri(CommonConstants.URL+"userInfor/" + idIF);
                        var postJob2 = client2.PutAsJsonAsync<tempUser>("", user);
                        postJob.Wait();

                        var postResult2 = postJob2.Result;
                        if (postResult2.IsSuccessStatusCode)
                        {
                            data.Add("mgs", "Update thành công");
                            List<User> listus = CommonConstants.getlistUser();
                            CommonConstants.listUser = listus;
                            string token = (string)Session[CommonConstants.AccessToken_SESSION];
                            List<User> getUser = CommonConstants.getlistUserID(userID,token);
                            if (getUser.Count > 0)
                            {
                                CommonConstants.User = getUser[0];
                            }
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(data);
        }

        [HttpPost]
        public String Delete(string id,string idIF)
        {
            // xóa listBDSmark
            List<BDSmark> listBDSmark = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL+"Get/BDSmark/");
                var responseTask = client.GetAsync(id.ToString());
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readJob = result.Content.ReadAsAsync<List<BDSmark>>();
                    readJob.Wait();
                    listBDSmark = readJob.Result;
                }
            }
            if (listBDSmark != null)
            {
                foreach(var item in listBDSmark)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(CommonConstants.URL+"BDSmark/");
                        var deleteJob = client.DeleteAsync(item._id.ToString());

                        var result = deleteJob.Result;
                        if (!result.IsSuccessStatusCode)
                        {
                            break;
                        }
                    }
                }
            }



            // xóa listLich
            List<Lich> listLich = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL+"Lich/");
                var responseTask = client.GetAsync(id.ToString());
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readJob = result.Content.ReadAsAsync<List<Lich>>();
                    readJob.Wait();
                    listLich = readJob.Result;
                }
            }

            if (listLich != null)
            {
                foreach (var item in listLich)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(CommonConstants.URL+"Lich/");
                        var deleteJob = client.DeleteAsync(item._id.ToString());

                        var result = deleteJob.Result;
                        if (!result.IsSuccessStatusCode)
                        {
                            break;
                        }
                    }
                }
            }
















            bool check = false;
            Dictionary<string, object> data = new Dictionary<string, object>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL+"auth/delete/");
                var deleteJob = client.DeleteAsync(id.ToString()+"/"+idIF.ToString());

                var result = deleteJob.Result;
                if (result.IsSuccessStatusCode)
                {
                    check = true;
                    List<User> listus = CommonConstants.getlistUser();
                    CommonConstants.listUser = listus;
                }
            }
            data.Add("mgs", check);
            return JsonConvert.SerializeObject(data);
        }

        [HttpPost]
        public String Logout(string id)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (id != "")
            {
                Session[CommonConstants.USER_SESSION] = null;
                CommonConstants.User = new User();
            }
            return JsonConvert.SerializeObject(data);
        }


        [Route("Customer/ListBDSmark")]
        [HttpGet]
        public ActionResult ListBDSmark()
        {
            userID = (string)Session[CommonConstants.USER_SESSION];
            if (userID == null)
            {
                return RedirectToAction("Index", "Login");
            }

            List<BDSmark> listBDSmark = CommonConstants.listBDSmark;
            
          
            List<User> listUser = CommonConstants.listUser;
           
            List<User> listUser_end = new List<User>() ;
            foreach (var item in listUser)
            {
                if (item.Email == userID)
                {
                    listUser_end.Add(item);
                }
            }

            List<BDSmark> listBDSmarkOfUser = new List<BDSmark>();

            foreach (var item in listBDSmark)
            {
                if (item.IDuser == listUser_end[0]._id)
                {
                    listBDSmarkOfUser.Add(item);
                }
            }

            //
            List<BDS> listBDS = CommonConstants.listBDS;       


            AdminListBDSmark_Model list = new AdminListBDSmark_Model();
            list.User = listUser_end;
            list.BDSmark = listBDSmarkOfUser;
            list.BDS = listBDS;

            return View(list);
        }


        [Route("Customer/ListLich")]
        [HttpGet]
        public ActionResult ListLich()
        {
            userID = (string)Session[CommonConstants.USER_SESSION];
            if (userID == null)
            {
                return RedirectToAction("Index", "Login");
            }

            List<Lich> listLich = CommonConstants.listLich;




            List<User> listUser = CommonConstants.listUser;           

            List<User> listUser_end = new List<User>();
            foreach (var item in listUser)
            {
                if (item.Email == userID)
                {
                    listUser_end.Add(item);
                }
            }

            List<Lich> listLichOfUser = new List<Lich>();

            foreach (var item in listLich)
            {
                if (item.IDuser == listUser_end[0]._id)
                {
                    listLichOfUser.Add(item);
                }
            }

            //
            List<BDS> listBDS = CommonConstants.listBDS;
            

            AdminListLich_Model list = new AdminListLich_Model();
            list.User = listUser_end;
            list.Lich = listLichOfUser;
            list.BDS = listBDS;

            return View(list);
        }
    }
}