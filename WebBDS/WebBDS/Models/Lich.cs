using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBDS.Models
{
    public class Lich
    {
        public Lich()
        {

        }  
        public string _id { get; set; }
        public string IDuser { get; set; }
        public string IDbds { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Sdt { get; set; }
        public string Message { get; set; }
        public bool Xacnhan { get; set; }
        public string Date { get; set; }
    }
}