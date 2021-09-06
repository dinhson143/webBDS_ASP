using Firebase.Auth;
using Firebase.Storage;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebBDS.Common;
using WebBDS.Models;
using User = WebBDS.Models.User;

namespace WebBDS.Controllers
{
    public class BDSController : Controller
    {
        // GET: BDS
        IFirebaseConfig config = new FireSharp.Config.FirebaseConfig()
        {
            AuthSecret = "Yc3HItx8xYxfdktTU31jV1siLWE2vknIZAt8gSJg",
            BasePath = "https://bds-asp-mvc-default-rtdb.firebaseio.com/"
        };
        public string userID { get; set; }
        public string idBDS { get; set; }
        public string mess { get; set; }
        private static string ApiKey = "AIzaSyDsIhxUtoEuX-GsYhTCd3T6tSUr2VA2MiA";
        private static string Bucket = "bds-asp-mvc.appspot.com";
        private static string AuthEmail = "dinhson14399@gmail.com";
        private static string AuthPassword = "tranthingocyen";
        //IFirebaseClient client;
        public ActionResult Index()
        {
            return View();
        }


        [Route("BDS/CreateBDS")]
        [HttpGet]
        public ActionResult CreateBDS()
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
            return View(list);
        }
        [HttpPost]
        public async Task<ActionResult> CreateBDS(BDS duan, HttpPostedFileBase[] file)
        {
            List<BDS> list = null;
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
                    list = readJob.Result;
                }
                else
                {
                    list = (List<BDS>)(IEnumerable<BDS>)Enumerable.Empty<AdminController>();
                    ViewData["mess"] = "Server Bảo Trì !";
                    return View();
                }
            }

            // kiem tra tên bds đã tồn tại
            foreach (var item in list)
            {
                if (item.Name == duan.Name)
                {
                    mess = "Bất Động Sản Đã Tồn Tại";
                    ViewData["mess"] = mess;
                    return View();
                }
            }



            // upload ảnh lên firebase
            List<String> listLink = new List<string>();
            // upload ảnh lên firebase
            for (var i = 0; i < file.Length; i++)
            {
                FileStream stream;
                string link = "";
                if (file[i].ContentLength > 0)
                {
                    string path = Path.Combine(Server.MapPath("~/Content/images/"), file[i].FileName);
                    file[i].SaveAs(path);
                    stream = new FileStream(Path.Combine(path), FileMode.Open);
                    // await Task.Run(() => (link = Upload(stream, file.FileName)));
                    link = await Upload(stream, file[i].FileName);
                }
                listLink.Add(link);               
            }
            
            duan.Image = listLink;
            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri("http://localhost:5000/api/BDS/");
                client.BaseAddress = new Uri(CommonConstants.URL+"BDS/");
                var postJob = client.PostAsJsonAsync<BDS>("", duan);
                postJob.Wait();

                var postResult = postJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    ViewData["mess"] = "Thêm thành công";
                    List<BDS> listbds = CommonConstants.getlistBDS();
                    CommonConstants.listBDS = listbds;
                }
            }


            List<LoaiBDS> listLBDS = CommonConstants.listLoaiBDS;      
            return View(listLBDS);
        }
        public async Task<string> Upload(FileStream stream, string filename)
        {
            string link = "";
            var auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

            var cancellation = new CancellationTokenSource();

            var task = new FirebaseStorage(
                Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                })
                .Child("images")
                .Child(filename)
                .PutAsync(stream, cancellation.Token);
            try
            {
                link = await task;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception was thrown: {0} ", ex);
            }
            return link;
        }





        [Route("BDS/EditBDS")]
        [HttpGet]
        public ActionResult EditBDS()
        {
            List<LoaiBDS> list = CommonConstants.listLoaiBDS;          
            return View(list);
        }

        [HttpPost]
        public async Task<ActionResult> EditBDS(BDS duan, HttpPostedFileBase[] file,string luachon)
        {

            List<string> Image = duan.Image.ToList();
            string[] Images = Image[0].Split(',');
            List<String> listLink = new List<string>();


            // delete kí tự trong string image
            for(int i = 0; i < Images.Length; i++)
            {
                string rs = Images[i].Replace("amp;", "");
                listLink.Add(rs);
            }
            
            List<BDS> list = CommonConstants.listBDS;         
            // kiem tra tên bds đã tồn tại
            foreach (var item in list)
            {
                if (item.Name == duan.Name && item._id != duan._id)
                {
                    mess = "Tên Bất Động Sản Đã Tồn Tại";
                    ViewData["mess"] = mess;
                    return View();
                }
            }


            // add image vào list image có sẵn 
            if (luachon == "Khong" && file[0] != null)
            {
                    // upload ảnh lên firebase
                    for (var i = 0; i < file.Length; i++)
                    {
                        FileStream stream;
                        string link = "";
                        if (file[i].ContentLength > 0)
                        {
                            string path = Path.Combine(Server.MapPath("~/Content/images/"), file[i].FileName);
                            file[0].SaveAs(path);
                            stream = new FileStream(Path.Combine(path), FileMode.Open);
                            // await Task.Run(() => (link = Upload(stream, file.FileName)));
                            link = await Upload(stream, file[i].FileName);
                        }
                        listLink.Add(link);
                    }
            }
            else if(file[0] != null && luachon=="Co")
            {
                listLink = new List<string>(); ;
                    // upload ảnh lên firebase
                    for (var i = 0; i < file.Length; i++)
                    {
                        FileStream stream;
                        string link = "";
                        if (file[i].ContentLength > 0)
                        {
                            string path = Path.Combine(Server.MapPath("~/Content/images/"), file[i].FileName);
                            file[0].SaveAs(path);
                            stream = new FileStream(Path.Combine(path), FileMode.Open);
                            // await Task.Run(() => (link = Upload(stream, file.FileName)));
                            link = await Upload(stream, file[i].FileName);
                            
                        }
                        if (link != null)
                        {
                            listLink.Add(link);
                        }
                    }
            }
            duan.Image = listLink;
           
            
            
            // update
          
            Dictionary<string, object> data = new Dictionary<string, object>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL + "BDS/");
                var postJob = client.PutAsJsonAsync<BDS>(duan._id.ToString(), duan);
                postJob.Wait();

                var postResult = postJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    data.Add("mgs", "Update thành công");
                    List<BDS> listbds = CommonConstants.getlistBDS();
                    CommonConstants.listBDS = listbds;
                }
                else
                {
                    List<LoaiBDS> listLBDS = CommonConstants.listLoaiBDS;
                    ViewData["mess"] = "Update thất bại";
                    return View(listLBDS);
                }
            }


            
            ViewData["mess"] = "Update thành công";
            return RedirectToAction("ListBDS", "Admin");
        }



        [HttpPost]
        public String DeleteBDS(string id)
        {
            bool check = false;
            Dictionary<string, object> data = new Dictionary<string, object>();
            // get list lịch 
            List<Lich> listLich = CommonConstants.getlistLich();
            CommonConstants.listLich = listLich;
            foreach(var item in listLich)
            {
                if(item.IDbds == id)
                {
                    data.Add("mgs", check);
                    return JsonConvert.SerializeObject(data);
                }
            }


           
            // nếu ko có lịch hẹn thì mới xóa
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL + "BDS/");
                var deleteJob = client.DeleteAsync(id.ToString());

                var result = deleteJob.Result;
                if (result.IsSuccessStatusCode)
                {
                    check = true;
                    List<BDS> listbds = CommonConstants.getlistBDS();
                    CommonConstants.listBDS = listbds;
                }
            }
            data.Add("mgs", check);
            return JsonConvert.SerializeObject(data);
        }

        [HttpPost]
        public String DeleteBDSmark(string id)
        {
            bool check = false;
            Dictionary<string, object> data = new Dictionary<string, object>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL + "BDSmark/");
                var deleteJob = client.DeleteAsync(id.ToString());

                var result = deleteJob.Result;
                if (result.IsSuccessStatusCode)
                {
                    check = true;
                    List<BDSmark> listbdsmark = CommonConstants.getlistBDSmark();
                    CommonConstants.listBDSmark = listbdsmark;
                    data.Add("mgs", check);
                    return JsonConvert.SerializeObject(data);
                }
                else
                {
                    data.Add("mgs", check);
                    return JsonConvert.SerializeObject(data);
                }
            }           
        }



        public ActionResult ChiTietBDS()
        {
            List<BDS> list = new List<BDS>();

            idBDS = (string)Session[CommonConstants.IDbds_SESSION];
            if (idBDS == null)
            {
                return View(list);
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL+"BDS/" + idBDS);
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
            BDS bds = new BDS();
            bds = list[0];
            return View(bds);
        }

        [HttpPost]
        public String AddCart(string id)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            bool check = true;
            userID = (string)Session[CommonConstants.USER_SESSION];
            User user = new User();

            // kiểm tra user đã login
            if (userID == null)
            {
                check = false;
                data.Add("mgs", check);
                return JsonConvert.SerializeObject(data);
            }
            else
            {
                user = CommonConstants.User;
            }

            // get list BDS mark
            List<BDSmark> list = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL);
                var responseTask = client.GetAsync("BDSmark");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readJob = result.Content.ReadAsAsync<List<BDSmark>>();
                    readJob.Wait();
                    list = readJob.Result;
                }              
            }

            // kiểm tra xem user đã thích BDS hay chưa
            if (list != null)
            {
                foreach (var item in list)
                {
                    if (item.IDbds == id && item.IDuser == user._id)
                    {
                        data.Add("note", "Bất Động Sản đã được quan tâm");
                        data.Add("mgs", check);
                        return JsonConvert.SerializeObject(data);
                    }
                }
            }







            // nếu chưa thì add
            BDSmark x = new BDSmark();
            x.IDuser = user._id;
            x.IDbds = id;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(CommonConstants.URL+"BDSmark");
                var postJob = client.PostAsJsonAsync<BDSmark>("", x);
                postJob.Wait();

                var postResult = postJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    data.Add("note", "Đã đánh dấu Bất Động Sản");
                    data.Add("mgs", check);
                }
            }
            return JsonConvert.SerializeObject(data);
        }



        // 

        [HttpPost]
        public String DatlichBDSmark(string id)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            Session[CommonConstants.IDbds_SESSION] = id;

            return JsonConvert.SerializeObject(data);
        }
    }
}