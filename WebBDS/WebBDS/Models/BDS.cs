using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBDS.Models
{
    public class BDS
    {
        public BDS()
        {

        }
        public string _id { get; set; }
        public string maloaiBDS { get; set; }
        public string Name { get; set; }
        public string Mota { get; set; }
        public string Vitri { get; set; }
        public string Dientich { get; set; }
        public string Gia { get; set; }
        public string KinhDo { get; set; }
        public string ViDo { get; set; }

        public string Status { get; set; }
        public List<String> Image { get; set; }
    }
}