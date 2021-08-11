using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WebBDS.Models
{
    public class User
    {
        public User()
        {
            
        }

       
        public string Email { get; set; }
        public string Password { get; set; }
        
        public string _id { get; set; }
        public InforUser UserInfor { get; set; }
    }
}