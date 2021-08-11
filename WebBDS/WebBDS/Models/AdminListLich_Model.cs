using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBDS.Models
{
    public class AdminListLich_Model
    {
        public AdminListLich_Model() { }
        public List<Lich> Lich { get; set; }
        public List<User> User { get; set; }
        public List<BDS> BDS { get; set; }
    }
}