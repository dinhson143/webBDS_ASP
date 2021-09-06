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
    public class TrangChuController : Controller
    {
        public string userID { get; set; }
        public ActionResult Index()
        {
            List<BDS> listBDS = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL);
                var responseTask = client.GetAsync("BDS");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readJob = result.Content.ReadAsAsync<List<BDS>>();
                    readJob.Wait();
                    listBDS = readJob.Result;
                }
                else
                {
                    ViewData["mess"] = "Server Bảo Trì !";
                }
            }

            // List Loại BDS
            List<LoaiBDS> listLoaiBDS = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL);
                var responseTask = client.GetAsync("loaiBDS");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readJob = result.Content.ReadAsAsync<List<LoaiBDS>>();
                    readJob.Wait();
                    listLoaiBDS = readJob.Result;
                }
                else
                {
                    ViewData["mess"] = "Server Bảo Trì !";
                }
            }
            TrangchuModel obj = new TrangchuModel();
            obj.BDS = listBDS;
            obj.loaiBDS = listLoaiBDS;
            return View(obj);
        }
        [HttpPost]
        public string CheckDangNhap(string id, string pw)
        {

            // chưa login 
            Dictionary<string, object> data = new Dictionary<string, object>();
            userID = (string)Session[CommonConstants.USER_SESSION];
            if (userID == null)
            {
                data.Add("value", "");
                return JsonConvert.SerializeObject(data);
            }



            // get thông tin user và về profile 
            List<User> list = new List<User>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL+"user/" + userID);
                var responseTask = client.GetAsync("");
                responseTask.Wait();

                var result2 = responseTask.Result;
                if (result2.IsSuccessStatusCode)
                {
                    var readJob = result2.Content.ReadAsAsync<List<User>>();
                    readJob.Wait();
                    list = readJob.Result;
                }
                else
                {
                    ViewData["mess"] = "Server Bảo Trì !";
                }
            }
            User user = CommonConstants.User;
                data.Add("value", user.UserInfor.Role);
            return JsonConvert.SerializeObject(data);
        }
    }
}