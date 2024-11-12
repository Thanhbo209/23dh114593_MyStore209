using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _23dh114593_MysStore209.Areas.Admin.Data
{
    public class AccountViewModel
    {
        public string Username { get; set; }
        public string UserRole { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }

        public string Password { get; set; } // Thêm thuộc tính Password
    }
}