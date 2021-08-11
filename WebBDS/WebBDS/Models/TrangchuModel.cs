using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBDS.Models
{
    public class TrangchuModel
    {
        public TrangchuModel() { }
        public List<LoaiBDS> loaiBDS { get; set; }
        public List<BDS> BDS { get; set; }
    }
}