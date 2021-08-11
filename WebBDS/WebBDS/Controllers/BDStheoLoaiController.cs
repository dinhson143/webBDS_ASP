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
    public class BDStheoLoaiController : Controller
    {
        public string idLoai { get; set; }
        [Route("BDStheoLoai")]
        // GET: BDStheoLoai
        public ActionResult Index()
        {
            List<BDS> list = new List<BDS>();

            idLoai = (string)Session[CommonConstants.LOAI_SESSION];
            if (idLoai == null)
            {
                return View(list);
            }
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL+"BDStheoLoai/" + idLoai);
                var responseTask = client.GetAsync("");
                responseTask.Wait();

                var result2 = responseTask.Result;
                if (result2.IsSuccessStatusCode)
                {
                    var readJob = result2.Content.ReadAsAsync<List<BDS>>();
                    readJob.Wait();
                    list = readJob.Result;
                }
                else
                {                   
                    ViewData["mess"] = "Server Bảo Trì !";
                }
            }
            return View(list);
        }

        [HttpPost]
        public String GetBDStheoloai(string id)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            Session.Add(CommonConstants.LOAI_SESSION, id);

            return JsonConvert.SerializeObject(data);
        }



        [HttpPost]
        public String GetID_BDS(string id)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            Session.Add(CommonConstants.IDbds_SESSION, id);

            return JsonConvert.SerializeObject(data);
        }


    }
}