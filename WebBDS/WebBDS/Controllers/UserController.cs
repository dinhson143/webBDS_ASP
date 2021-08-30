using FireSharp.Interfaces;
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
    public class UserController : Controller
    {
        // GET: User
        public string userID { get; set; }
        public string mess { get; set; }
        private static string ApiKey = "AIzaSyDsIhxUtoEuX-GsYhTCd3T6tSUr2VA2MiA";
        private static string Bucket = "bds-asp-mvc.appspot.com";
        private static string AuthEmail = "dinhson14399@gmail.com";
        private static string AuthPassword = "tranthingocyen";
        public ActionResult Index()
        {
            if (CommonConstants.listUser.Count == 0)
            {
                List<User> listus = CommonConstants.getlistUser();
                CommonConstants.listUser = listus;
            }

            // loaiBDS
            if (CommonConstants.listLoaiBDS.Count == 0)
            {
                List<LoaiBDS> listlbds = CommonConstants.getlistLoaiBDS();
                CommonConstants.listLoaiBDS = listlbds;
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
            // listLich
            if (CommonConstants.listLich.Count == 0)
            {
                List<Lich> listLich = CommonConstants.getlistLich();
                CommonConstants.listLich = listLich;
            }



            //user
            userID = (string)Session[CommonConstants.USER_SESSION];
            string token = (string)Session[CommonConstants.AccessToken_SESSION];
            if (CommonConstants.User.Email != userID)
            {
                List<User> getUser = CommonConstants.getlistUserID(userID,token);
                if (getUser.Count > 0)
                {
                    CommonConstants.User = getUser[0];
                }
            }
            return View(CommonConstants.User);
        }
        [Route("User/ListUser")]
        [HttpGet]
        public ActionResult ListUser()
        {
            List<User> list = null;
            list = CommonConstants.listUser;
            return View(list);
        }

        [Route("User/LoaiBDS")]
        [HttpGet]
        public ActionResult LoaiBDS()
        {
            List<LoaiBDS> list = null;
            list = CommonConstants.listLoaiBDS;
            return View(list);
        }


        [Route("User/ListBDS")]
        [HttpGet]
        public ActionResult ListBDS()
        {
            List<BDS> listBDS = CommonConstants.listBDS;
            // List Loại BDS
            List<LoaiBDS> listLoaiBDS = CommonConstants.listLoaiBDS;


            AdminListBDS_Model obj = new AdminListBDS_Model();
            obj.BDS = listBDS;
            obj.loaiBDS = listLoaiBDS;

            return View(obj);
        }



        [HttpPost]
        public string EditBDS(string id, string idLoai, string name, string mota, string vitri, string dientich, string gia, string kinhdo, string vido, string status, string image)
        {
            string[] Image = image.Split(',');
            Dictionary<string, object> data = new Dictionary<string, object>();
            BDS dt = new BDS();
            dt._id = id;
            dt.maloaiBDS = idLoai;
            dt.Name = name;
            dt.Mota = mota;
            dt.Vitri = vitri;
            dt.Dientich = dientich;
            dt.Gia = gia;
            dt.KinhDo = kinhdo;
            dt.ViDo = vido;
            dt.Status = status;
            dt.Image = Image.ToList();
            TempData["bdsEdit"] = dt;
            return JsonConvert.SerializeObject(data);
        }




        [Route("User/ListBDSmark")]
        [HttpGet]
        public ActionResult ListBDSmark()
        {
            List<BDSmark> listBDSmark = CommonConstants.listBDSmark;


            List<User> listUser = CommonConstants.listUser;

            //
            List<BDS> listBDS = CommonConstants.listBDS;

            AdminListBDSmark_Model list = new AdminListBDSmark_Model();
            list.User = listUser;
            list.BDSmark = listBDSmark;
            list.BDS = listBDS;

            return View(list);
        }


        [Route("User/Lichhenyeucau")]
        [HttpGet]
        public ActionResult Lichhenyeucau()
        {
            List<User> listUser = CommonConstants.listUser;

            List<Lich> listLich = CommonConstants.listLich;


            List<BDS> listBDS = CommonConstants.listBDS;

            AdminListLich_Model list = new AdminListLich_Model();
            list.User = listUser;
            list.Lich = listLich;
            list.BDS = listBDS;

            return View(list);
        }







        [Route("User/CreateUser")]
        [HttpGet]
        public ActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateUser(User user, InforUser infor)
        {
            List<User> list = CommonConstants.listUser;
            foreach (var item in list)
            {
                if (user.Email == item.Email)
                {
                    mess = "Gmail đã được đăng kí.";
                    ViewData["mess"] = mess;
                    return View();
                }
            }


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
                //client.BaseAddress = new Uri("http://localhost:5000/api/auth/register");
                var postJob = client.PostAsJsonAsync<tempUser>("register", tempUser);
                postJob.Wait();

                var postResult = postJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    mess = "Đăng kí thành công";
                    ViewData["mess"] = mess;
                    List<User> listus = CommonConstants.getlistUser();
                    CommonConstants.listUser = listus;
                    return View();
                }
            }
            ModelState.AddModelError(string.Empty, "Server occured error. Please check width admin!");
            mess = "Server occured error or Phone number is used. Please check width admin!";
            ViewData["mess"] = mess;
            return View();
        }


        [Route("Thongke")]
        public ActionResult Thongke()
        {
            // bds
            List<BDS> listBDS = CommonConstants.listBDS;



            /// bds mark
            List<BDSmark> listBDSmark = CommonConstants.listBDSmark;


            // 
            List<Thongke> listThongke = new List<Thongke>();
            foreach (var x in listBDS)
            {
                var dem = 0;
                foreach (var y in listBDSmark)
                {
                    if (String.Compare(x._id, y.IDbds) == 0)
                    {
                        dem++;
                    }
                }
                if (dem > 0)
                {
                    Thongke tk = new Thongke();
                    tk.NameBDS = x.Name;
                    tk.Soluong = dem;
                    listThongke.Add(tk);
                }
            }




            return View(listThongke);
        }

    }
}