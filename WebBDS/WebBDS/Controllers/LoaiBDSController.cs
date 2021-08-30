using Newtonsoft.Json;
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
    public class LoaiBDSController : Controller
    {
        // GET: LoaiBDS
        public ActionResult Index()
        {
            return View();
        }


        [Route("LoaiBDS/CreateLoaiBDS")]
        [HttpGet]
        public ActionResult CreateLoaiBDS()
        {
            return View();
        }



        [HttpPost]
        public ActionResult CreateLoaiBDS(LoaiBDS loaiBDS)
        {
            List<LoaiBDS> list = null;
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
                    list = readJob.Result;
                }
                else
                {
                    list = (List<LoaiBDS>)(IEnumerable<LoaiBDS>)Enumerable.Empty<AdminController>();
                    ViewData["mess"] = "Server Bảo Trì !";
                }
            }

            foreach(var item in list)
            {
                if (item.Name == loaiBDS.Name)
                {
                    ViewData["mess"] = "Loại BDS đã tồn tại";
                    return View();
                }
            }






            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL+"loaiBDS/");
                //client.BaseAddress = new Uri("http://localhost:2000/api/loaiBDS");
                var postJob = client.PostAsJsonAsync<LoaiBDS>("", loaiBDS);
                postJob.Wait();

                var postResult = postJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    ViewData["mess"] = "Thêm thành công";
                    List<LoaiBDS> listlbds = CommonConstants.getlistLoaiBDS();
                    CommonConstants.listLoaiBDS = listlbds;
                }
            }
            return View();
        }

        [HttpPost]
        public String EditLBDS(string id, string name)
        {
            LoaiBDS x = new LoaiBDS();
            Dictionary<string, object> data = new Dictionary<string, object>();
            x.Name = name;
            x._id = id;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL+"loaiBDS/");
                var postJob = client.PutAsJsonAsync<LoaiBDS>(id.ToString(), x);
                postJob.Wait();

                var postResult = postJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    data.Add("mgs", "Update thành công");
                    List<LoaiBDS> listlbds = CommonConstants.getlistLoaiBDS();
                    CommonConstants.listLoaiBDS = listlbds;
                }
            }
            return JsonConvert.SerializeObject(data);
        }

        [HttpPost]
        public String DeleteLBDS(string id)
        {
            bool check = false;
            Dictionary<string, object> data = new Dictionary<string, object>();
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
                    listBDS = (List<BDS>)(IEnumerable<BDS>)Enumerable.Empty<AdminController>();
                    ViewData["mess"] = "Server Bảo Trì !";
                }
            }

            foreach(var item in listBDS)
            {
                if(item.maloaiBDS == id)
                {
                    check = false;
                    data.Add("mgs", check);
                    return JsonConvert.SerializeObject(data);
                }
            }




           
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL+"loaiBDS/");
                var deleteJob = client.DeleteAsync(id.ToString());

                var result = deleteJob.Result;
                if (result.IsSuccessStatusCode)
                {
                    check = true;
                    List<LoaiBDS> listlbds = CommonConstants.getlistLoaiBDS();
                    CommonConstants.listLoaiBDS = listlbds;
                    data.Add("mgs", check);
                }
            }
            return JsonConvert.SerializeObject(data);
        }
    }
}