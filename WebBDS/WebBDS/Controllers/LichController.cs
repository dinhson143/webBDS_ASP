using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using WebBDS.Common;
using WebBDS.Models;
using System.Net.Mail;
using System.Net;

namespace WebBDS.Controllers
{

    public class LichController : Controller
    {
        public string userID { get; set; }
        public string idBDS { get; set; }
        // GET: Lich
        [Route("Lich")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("Lich/CreateLich")]
        [HttpGet]
        public ActionResult CreateLich()
        {
            userID = (string)Session[CommonConstants.USER_SESSION];
            if (userID == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }



        [HttpPost]
        public ActionResult CreateLich(Lich lich)
        {
            userID = (string)Session[CommonConstants.USER_SESSION];
            lich.IDuser = userID;
            lich.Xacnhan = false;
            idBDS = (string)Session[CommonConstants.IDbds_SESSION];
            lich.IDbds = idBDS;

            lich.Date = lich.Date.ToString();

            List<Lich> list = new List<Lich>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL);
                var responseTask = client.GetAsync("Lich");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readJob = result.Content.ReadAsAsync<List<Lich>>();
                    readJob.Wait();
                    list = readJob.Result;
                }
                else
                {
                    ViewData["mess"] = "Server Bảo Trì !";
                }
            }



            


            List<User> listUS = new List<User>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL+ "user/" + userID);
                var responseTask = client.GetAsync("");
                responseTask.Wait();

                var result2 = responseTask.Result;
                if (result2.IsSuccessStatusCode)
                {
                    var readJob = result2.Content.ReadAsAsync<List<User>>();
                    readJob.Wait();
                    listUS = readJob.Result;
                }
                else
                {                   
                    ViewData["mess"] = "Server Bảo Trì !";
                }
            }
            User user = new User();
            if (listUS.Count > 0)
            {
                user = listUS[0];
            }


            lich.IDuser = user._id;


            foreach (var item in list)
            {
                if (item.IDbds == lich.IDbds && item.IDuser == lich.IDuser)
                {
                    ViewData["mess"] = "Bạn đã có lịch cho bds này";
                    return View();
                }              
                else if(item.IDuser == lich.IDuser)
                {
                    DateTime x = DateTime.Parse(item.Date);
                    DateTime y = DateTime.Parse(lich.Date);
                    if (x.Date.Day == y.Date.Day && x.Date.Month == y.Date.Month && x.Date.Year ==y.Date.Year)
                    {
                        ViewData["mess"] = "Ngày bạn chọn đã có cuôc hẹn";
                        return View();
                    }
                }
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL+"Lich");
                var postJob = client.PostAsJsonAsync<Lich>("", lich);
                postJob.Wait();

                var postResult = postJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    ViewData["mess"] = "Đặt lịch thành công";
                }
                else
                {
                    ViewData["mess"] = "Đặt lịch thất bại";
                }
            }

            return View();
        }


        [HttpPost]
        public String DeleteLich(string id)
        {
            bool check = false;
            Dictionary<string, object> data = new Dictionary<string, object>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL+"Lich/");
                var deleteJob = client.DeleteAsync(id.ToString());

                var result = deleteJob.Result;
                if (result.IsSuccessStatusCode)
                {
                    check = true;
                    List<Lich> listLich = CommonConstants.getlistLich();
                    CommonConstants.listLich = listLich;
                }
            }
            data.Add("mgs", check);
            return JsonConvert.SerializeObject(data);
        }


        [HttpPost]
        public String XacnhanLich(string id)
        {


            bool check = false;
            Dictionary<string, object> data = new Dictionary<string, object>();

            List<Lich> list = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL+"Get/");
                var responseTask = client.GetAsync(id.ToString());
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readJob = result.Content.ReadAsAsync<List<Lich>>();
                    readJob.Wait();
                    list = readJob.Result;
                }
            }



            Lich x = new Lich();
            if (list != null)
            {
                x = list[0];
            }
          
            x.Xacnhan = true;





            Gmail model = new Gmail();
            MailMessage mm = new MailMessage("dinhson14399@gmail.com", x.Email);

            mm.Subject = "XÁC NHẬN LỊCH HẸN";
            mm.Body = "Lịch hẹn của bạn đã được chúng tôi xác nhận. Vui lòng đến đúng giớ với thông tin ghi trên lịch hẹn: " +
               x.Date + " tại Sảnh A Cenatral City District 7 HCM" + "  Mọi thắc mắc xin liện hệ SDT:0347596714";
            mm.IsBodyHtml = false;

            SmtpClient smtp = new SmtpClient();

            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;

            NetworkCredential nc = new NetworkCredential("dinhson14399@gmail.com", "tranthingocyen");

            smtp.UseDefaultCredentials = true;
            smtp.Credentials = nc;
            smtp.Send(mm);











            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(CommonConstants.URL+"Lich/");
                var postJob = client.PutAsJsonAsync<Lich>(id.ToString(), x);
                postJob.Wait();

                var postResult = postJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    check = true;
                    List<Lich> listLich = CommonConstants.getlistLich();
                    CommonConstants.listLich = listLich;
                }
            }
            data.Add("mgs", check);
            return JsonConvert.SerializeObject(data);
        }
    }
}