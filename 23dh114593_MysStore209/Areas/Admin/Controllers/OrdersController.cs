using _23dh114593_MysStore209.Areas.Admin.Data;
using _23dh114593_MysStore209.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _23dh114593_MysStore209.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();
        

        // GET: Admin/Orders
        public ActionResult Index()
        {
            var orders = db.Orders.Select(o => new OrderViewModel
            {
                OrderID = o.OrderID,
                CustomerName = o.Customer.CustomerName,
                OrderDate = o.OrderDate,
                AddressDelivery = o.AddressDelivery,
                TotalAmount = o.TotalAmount,
                OrderDetails = o.OrderDetails.Select(d => new OrderDetailViewModel
                {
                    ProductName = d.Product.ProductName,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice
                }).ToList()
            }).ToList();

            return View(orders);
        }

        public ActionResult OrderDetails(int id)
        {
            var order = db.Orders.Where(o => o.OrderID == id).Select(o => new OrderViewModel
            {
                OrderID = o.OrderID,
                CustomerName = o.Customer.CustomerName,
                OrderDate = o.OrderDate,
                AddressDelivery = o.AddressDelivery,
                TotalAmount = o.TotalAmount,
                OrderDetails = o.OrderDetails.Select(d => new OrderDetailViewModel
                {
                    ProductName = d.Product.ProductName,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice
                }).ToList()
            }).FirstOrDefault();

            if (order == null)
            {
                return HttpNotFound();
            }

            return PartialView("_OrderDetails", order.OrderDetails);
        }



        public JsonResult GetOrderDetails(int orderId)
        {
            // Lấy danh sách chi tiết đơn hàng từ database
            var orderDetails = db.OrderDetails
                .Where(od => od.OrderID == orderId)
                .Select(od => new OrderDetailViewModel
                {
                    ProductName = od.Product.ProductName,
                 
                    UnitPrice = od.UnitPrice,
                    Quantity = od.Quantity
                }).ToList();

            // Render partial view và trả về kết quả HTML
            var html = this.RenderPartialViewToString("_OrderDetails", orderDetails);
            return Json(html, JsonRequestBehavior.AllowGet);
        }

        // Hàm hỗ trợ render view thành string
        private string RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

    }
}