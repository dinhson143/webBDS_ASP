using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBDS.Models
{
    public class AdminListBDSmark_Model
    {
        public AdminListBDSmark_Model() { }
        public List<BDSmark> BDSmark { get; set; }
        public List<User> User { get; set; }
        public List<BDS> BDS { get; set; }
    }
}