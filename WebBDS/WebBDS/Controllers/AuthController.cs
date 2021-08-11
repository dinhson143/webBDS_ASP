using Facebook;
using Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Web.Security;
using System.Net.Http;
using WebBDS.Models;
using WebBDS.Common;

namespace WebBDS.Controllers
{
    public class AuthController : Controller
    {
        // GET: Auth
        [Route("Auth")]
        public ActionResult Index()
        {
            return View();
        }
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        [AllowAnonymous]
        public ActionResult Facebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(
                new
                {
                    client_id = "1028232201317827",
                    client_secret = "6f4f1bb9cfe7f84ef4f8565132ce788d",
                    redirect_uri = RedirectUri.AbsoluteUri,
                    response_type = "code",
                    scope = "email"
                });
            return Redirect(loginUrl.AbsoluteUri);
        }
        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token",
                new
                {
                    client_id = "1028232201317827",
                    client_secret = "6f4f1bb9cfe7f84ef4f8565132ce788d",
                    redirect_uri = RedirectUri.AbsoluteUri,
                    code = code
                });
            var accessToken = result.access_token;
            Session["AccessToken"] = accessToken;
            fb.AccessToken = accessToken;
            dynamic me = fb.Get("me?fields=link,first_name,currency,last_name,email,gender,locale,timezone,verified,picture,age_range");
            
            string id = me.id;
            string name =me.first_name +" "+ me.last_name;

            //FormsAuthentication.SetAuthCookie(email, false);

            List<User> list = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL);
                var responseTask = client.GetAsync("user");
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
                    list = (List<User>)(IEnumerable<User>)Enumerable.Empty<AdminController>();
                    ViewData["mess"] = "Server Bảo Trì !";
                    return View();
                }
            }
            foreach (var item in list)
            {
                if (id == item.Email)
                {
                    Session.Add(CommonConstants.USER_SESSION, id);
                    return RedirectToAction("Index", "Customer");
                }
            }

            var tempUser = new tempUser();
            tempUser.Name = name;
            tempUser.Password = id;
            tempUser.Phone = id;
            tempUser.Address = id;
            tempUser.Email = id;
            tempUser.Role = "customer";

            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri("http://localhost:5000/api/auth/register");
                client.BaseAddress = new Uri(CommonConstants.URL+"auth/register");
                var postJob = client.PostAsJsonAsync<tempUser>("register", tempUser);
                postJob.Wait();

                var postResult = postJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    Session.Add(CommonConstants.USER_SESSION, id);
                    return RedirectToAction("Index", "Customer");
                }
            }
            ModelState.AddModelError(string.Empty, "Server occured error. Please check width admin!");

            return RedirectToAction("Index", "Login");
        }




        public ActionResult GoogleLogin(string email,string name)
        {

            List<User> list = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL);
                var responseTask = client.GetAsync("user");
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
                    list = (List<User>)(IEnumerable<User>)Enumerable.Empty<AdminController>();
                    ViewData["mess"] = "Server Bảo Trì !";
                    return View();
                }
            }
            foreach (var item in list)
            {
                if (email == item.Email)
                {
                    Session.Add(CommonConstants.USER_SESSION, email);
                    return RedirectToAction("Index", "User");
                }
            }



            var tempUser = new tempUser();
            tempUser.Name = name;
            tempUser.Password = email;
            tempUser.Phone = email;
            tempUser.Address = email;
            tempUser.Email = email;
            tempUser.Role = "customer";

            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri("http://localhost:5000/api/auth/register");
                client.BaseAddress = new Uri(CommonConstants.URL + "auth/register");
                var postJob = client.PostAsJsonAsync<tempUser>("register", tempUser);
                postJob.Wait();

                var postResult = postJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    Session.Add(CommonConstants.USER_SESSION, email);
                    return RedirectToAction("Index", "User");
                }
            }
            ModelState.AddModelError(string.Empty, "Server occured error. Please check width admin!");

            return View();
        }    
    }
}