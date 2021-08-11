using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using WebBDS.Models;

namespace WebBDS.Common
{
    public class CommonConstants
    {
        public static string USER_SESSION = "USER_SESSION";
        public static string LOAI_SESSION = "LOAI_SESSION";
        public static string IDbds_SESSION = "IDbds_SESSION";
        public static string AccessToken_SESSION = "AccessToken_SESSION";
        public static string URL = "https://apinodejsbds.herokuapp.com/api/";

        public static List<User> listUser = new List<User>();
        public static List<LoaiBDS> listLoaiBDS = new List<LoaiBDS>();
        public static List<BDS> listBDS = new List<BDS>();

        public static List<BDSmark> listBDSmark = new List<BDSmark>();

        public static List<Lich> listLich = new List<Lich>();

        public static User User = new User();




        // user
        public static List<User> getlistUser()
        {
            List<User> list = new List<User>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                var responseTask = client.GetAsync("user");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readJob = result.Content.ReadAsAsync<List<User>>();
                    readJob.Wait();
                    list = readJob.Result;
                }
            }
            return list;
        }
        public static List<User> getlistUserID(string userID,string token)
        {
            List<User> list = new List<User>();
            
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
                client.BaseAddress = new Uri(URL + "user/" + userID);
                var responseTask = client.GetAsync("");
                responseTask.Wait();

                var result2 = responseTask.Result;
                if (result2.IsSuccessStatusCode)
                {
                    var readJob = result2.Content.ReadAsAsync<List<User>>();
                    readJob.Wait();
                    list = readJob.Result;
                }
            }
            return list;
        }
        // loaiBDS
        public static List<LoaiBDS> getlistLoaiBDS()
        {
            List<LoaiBDS> list = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                var responseTask = client.GetAsync("loaiBDS");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readJob = result.Content.ReadAsAsync<List<LoaiBDS>>();
                    readJob.Wait();
                    list = readJob.Result;
                }
            }
            return list;
        }
        // BDS
        public static List<BDS> getlistBDS()
        {
            List<BDS> listBDS = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                var responseTask = client.GetAsync("BDS");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readJob = result.Content.ReadAsAsync<List<BDS>>();
                    readJob.Wait();
                    listBDS = readJob.Result;
                }
            }
            return listBDS;
        }
        
        // listBDSmark
        public static List<BDSmark> getlistBDSmark()
        {
            List<BDSmark> listBDSmark = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                var responseTask = client.GetAsync("BDSmark");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readJob = result.Content.ReadAsAsync<List<BDSmark>>();
                    readJob.Wait();
                    listBDSmark = readJob.Result;
                }
            }
            return listBDSmark;
        }

        public static List<Lich> getlistLich()
        {
            List<Lich> listLich = null;
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
                    listLich = readJob.Result;
                }
            }

            return listLich;
        }
    
        // check auth

    
    
    
    }
}