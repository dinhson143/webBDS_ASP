using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBDS.Models
{
    public class InforUser
    {
        public InforUser()
        {
        }
        public string _id { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }
}