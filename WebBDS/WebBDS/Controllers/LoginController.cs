using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Owin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Security.Claims;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebBDS.Common;
using WebBDS.Models;

namespace WebBDS.Controllers
{
    public class LoginController : Controller
    {
        public class Login
        {
            public Login()
            {

            }
            public string Email { get; set; }
            public string Password { get; set; }
        }
        [Required]
        public string mess { get; set; }
        [Route("Login")]
        public ActionResult Index()
        {
            return View();
        }

        //
        public static async Task<string> GetAuthorizeToken(string id, string pw)
        {
            // Initialization.  
            string responseObj = string.Empty;

            // Posting.  
            Login login = new Login();
            login.Email = id;
            login.Password = pw;
            Dictionary<string, object> data = new Dictionary<string, object>();
            using (var client = new HttpClient())
            {

                // Setting Base address.  
                client.BaseAddress = new Uri("http://localhost:2000/api/auth/login");

                // Setting content type.  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Initialization.  
                HttpResponseMessage response = new HttpResponseMessage();
                List<KeyValuePair<string, string>> allIputParams = new List<KeyValuePair<string, string>>();

                // Convert Request Params to Key Value Pair.  

                // URL Request parameters.  
                HttpContent requestParams = new FormUrlEncodedContent(allIputParams);

                // HTTP POST  
                response = await client.PostAsync("Token", requestParams).ConfigureAwait(false);

                // Verification  
                if (response.IsSuccessStatusCode)
                {
                    // Reading Response.  

                }
            }

            return responseObj;
        }
        //



        [Route("Login/Create")]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(User user, InforUser infor)
        {
            List<User> list = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL);
                var responseTask = client.GetAsync("user");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readJob = result.Content.ReadAsAsync<List<User>>();
                    readJob.Wait();
                    list = readJob.Result;
                }
                else
                {
                    list = (List<User>)(IEnumerable<User>)Enumerable.Empty<AdminController>();
                    ViewData["mess"] = "Server Bảo Trì !";
                    return View();
                }
            }
            foreach (var item in list)
            {
                if (user.Email == item.Email)
                {
                    mess = "Gmail đã được đăng kí.";
                    ViewData["mess"] = mess;
                    return View();
                }
            }




            infor.Role = "customer";
            user.UserInfor = infor;
            var tempUser = new tempUser();
            tempUser.Name = user.UserInfor.Name;
            tempUser.Password = user.Password;
            tempUser.Phone = user.UserInfor.Phone;
            tempUser.Address = user.UserInfor.Address;
            tempUser.Email = user.Email;
            tempUser.Role = user.UserInfor.Role;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL + "auth/register");
                var postJob = client.PostAsJsonAsync<tempUser>("register", tempUser);
                postJob.Wait();

                var postResult = postJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    mess = "Đăng kí thành công";
                    ViewData["mess"] = mess;
                    return View();
                }
            }
            ModelState.AddModelError(string.Empty, "Server occured error. Please check width admin!");
            mess = "Server occured error or Phone number is used. Please check width admin!";
            ViewData["mess"] = mess;
            return View();
        }

        [HttpPost]
        public string CheckLogin(string id, string pw)
        {
            List<User> user = null;
            User temp = new User();
            Login login = new Login();
            login.Email = id;
            login.Password = pw;


            Dictionary<string, object> data = new Dictionary<string, object>();
            Dictionary<string, string> tokenDetails = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL + "auth/login");
                var postJob = client.PostAsJsonAsync<Login>("login", login);
                postJob.Wait();

                if (postJob.IsCompleted)
                {
                    if (postJob.Result.Content.ReadAsStringAsync().Result.Contains("accessToken"))
                    {
                        tokenDetails = JsonConvert.DeserializeObject<Dictionary<string, string>>(postJob.Result.Content.ReadAsStringAsync().Result);
                    }
                }
                var result = postJob.Result;
                string token = "";
                if (tokenDetails != null)
                {
                    Session.Add(CommonConstants.AccessToken_SESSION, tokenDetails["accessToken"]);
                    token = (string)Session[CommonConstants.AccessToken_SESSION];
                    if (result.IsSuccessStatusCode == true)
                    {
                        using (var client2 = new HttpClient())
                        {
                            client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                            client2.BaseAddress = new Uri(CommonConstants.URL + "user/" + id);
                            //client2.BaseAddress = new Uri("http://localhost:2000/api/user/" + id);
                            var responseTask = client2.GetAsync("");
                            responseTask.Wait();

                            var result2 = responseTask.Result;
                            if (result2.IsSuccessStatusCode)
                            {
                                var readJob = result2.Content.ReadAsAsync<List<User>>();
                                readJob.Wait();
                                user = readJob.Result;
                            }
                            else
                            {
                                ViewData["mess"] = "Server Bảo Trì !";
                            }
                        }
                    }
                }
                //
                //

            }
            if (user != null) temp = user[0];
            bool check = true;
            string loai = "";
            if (temp.Email == null)
            {
                check = false;
                data.Add("mgs", check);
                return JsonConvert.SerializeObject(data);
            }
            else
            {
                Session.Add(CommonConstants.USER_SESSION, id);
            }

            if (temp.UserInfor.Role == "admin")
            {
                loai = "admin";
            }
            else if (temp.UserInfor.Role == "user")
            {
                loai = "user";
            }
            else
            {
                loai = "customer";
            }
            data.Add("mgs", check);
            data.Add("loai", loai);
            return JsonConvert.SerializeObject(data);
        }



        [Route("Login/QuenMK")]
        [HttpGet]
        public ActionResult QuenMK()
        {
            return View();
        }

        [HttpPost]
        public ActionResult QuenMK(string email)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            // random mật khẩu
            Random r = new Random();
            var x = r.Next(0, 1000000);
            string s = x.ToString("000000");
            // kiếm user
            List<User> listus = CommonConstants.getlistUser();
            User us = new User();
            foreach(var item in listus)
            {
                if (String.Compare(item.Email, email, true) == 0)
                {
                    us = item;
                    break;
                }
            }
            if (us.Email == null)
            {
                mess = "Email chưa được đăng kí.";
                ViewData["mess"] = mess;
                return View();
            }
            // update mật khẩu
            tempUser user = new tempUser();
            user.Address = us.UserInfor.Address;
            user.Email = us.Email;
            user.Name = us.UserInfor.Name;
            user.Phone = us.UserInfor.Phone;
            user.Role = us.UserInfor.Role;
            user.Password = s;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL + "user/" + us._id);
                var postJob = client.PutAsJsonAsync<tempUser>("", user);
                postJob.Wait();

                var postResult = postJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    // gửi mật khẩu cho mail
                    Gmail model = new Gmail();
                    MailMessage mm = new MailMessage("dinhson14399@gmail.com", email);

                    mm.Subject = "ĐỔI MẬT KHẨU";
                    mm.Body = "Mật khẩu mới của bạn: " + s +
                    " .Mọi thắc mắc xin liện hệ SDT:0347596714 !!!";
                    mm.IsBodyHtml = false;

                    SmtpClient smtp = new SmtpClient();

                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;

                    NetworkCredential nc = new NetworkCredential("dinhson14399@gmail.com", "tranthingocyen");

                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = nc;
                    smtp.Send(mm);
                }
                else
                {
                    mess = "Lỗi hệ thống.";
                    ViewData["mess"] = mess;
                    return View();
                }
            }


            

            data.Add("mgs", "");
            mess = "Mật khẩu mới đã được gửi về Email của bạn !!!";
            ViewData["mess"] = mess;
            return View();
        }
    }
}