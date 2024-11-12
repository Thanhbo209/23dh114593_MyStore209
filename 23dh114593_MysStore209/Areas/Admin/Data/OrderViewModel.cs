using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _23dh114593_MysStore209.Areas.Admin.Data
{
    public class OrderViewModel
    {
        public int OrderID { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public string AddressDelivery { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderDetailViewModel> OrderDetails { get; set; }
    }

    public class OrderDetailViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}