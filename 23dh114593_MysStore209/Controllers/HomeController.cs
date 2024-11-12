using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using System.Diagnostics;
using _23dh114593_MysStore209.Models;
namespace _23dh114593_MysStore209.Controllers
{
    public class HomeController : Controller
    {


        private MyStoreEntities db = new MyStoreEntities();

        public class CartItem
        {
            public int ProductID { get; set; }
            public int Quantity { get; set; }
        }

        public ActionResult Index()
        {
            var products = db.Products.ToList(); // Lấy danh sách sản phẩm từ cơ sở dữ liệu

            if (products == null || !products.Any())
            {
                // Log nếu danh sách sản phẩm rỗng hoặc null
                System.Diagnostics.Debug.WriteLine("Không có sản phẩm nào.");
            }

            return View(products); // Truyền danh sách sản phẩm sang View


        }

        public ActionResult Navbar()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Services()
        {
            return View();
        }

        public ActionResult Menu()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult AddToCart(int productId)
        {
            if (Session["Username"] == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để đặt món." }, JsonRequestBehavior.AllowGet);
            }

            // Lấy giỏ hàng từ Session
            var cart = Session["Cart"] as List<int>;
            if (cart == null)
            {
                cart = new List<int>();
            }

            // Thêm ProductID vào giỏ hàng
            cart.Add(productId);
            Session["Cart"] = cart;

            return Json(new { success = true, cartCount = cart.Count }, JsonRequestBehavior.AllowGet);
        }

        // Hiển thị giỏ hàng
        public ActionResult Cart()
        {
            // Lấy danh sách sản phẩm trong giỏ hàng từ Session
            var cart = Session["Cart"] as List<int> ?? new List<int>();

            // Lấy thông tin sản phẩm từ database
            var products = db.Products.Where(p => cart.Contains(p.ProductID)).ToList();

            return View(products);
        }

        // Xóa sản phẩm khỏi giỏ hàng
        public JsonResult RemoveFromCart(int productId)
        {
            var cart = Session["Cart"] as List<int>;
            if (cart != null)
            {
                cart.Remove(productId);
                Session["Cart"] = cart;
            }
            return Json(new { success = true, cartCount = cart?.Count ?? 0 }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ConfirmOrder()
        {
            var username = Session["Username"]?.ToString();

            // Kiểm tra nếu người dùng chưa đăng nhập
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy giỏ hàng từ Session
            var cart = Session["Cart"] as List<int>; // Session["Cart"] chứa danh sách ProductID
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Cart"); // Nếu giỏ hàng trống, chuyển hướng về trang giỏ hàng
            }

            // Lấy danh sách sản phẩm từ database
            var products = db.Products.Where(p => cart.Contains(p.ProductID)).ToList();

            // Truyền danh sách sản phẩm qua ViewBag
            ViewBag.Products = products;

            // Lấy thông tin khách hàng từ cơ sở dữ liệu
            var customer = db.Customers.FirstOrDefault(c => c.Username == username);

            if (customer == null)
            {
                return RedirectToAction("Index"); // Chuyển hướng nếu không tìm thấy khách hàng
            }
            return View(customer);
        }

        public ActionResult OrderSuccess()
        {
            return View(); // Đảm bảo file OrderSuccess.cshtml tồn tại trong Views/Home
        }
        [HttpPost]
        public ActionResult PlaceOrder(string address, string phone)
        {
            // Lấy Username từ Session
            var username = Session["Username"]?.ToString();
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy giỏ hàng từ Session
            var cart = Session["Cart"] as List<int>; // Danh sách ProductID
            if (cart == null || !cart.Any())
            {
                // Nếu giỏ hàng trống, chuyển về trang giỏ hàng
                TempData["ErrorMessage"] = "Giỏ hàng của bạn trống.";
                return RedirectToAction("Cart", "Home");
            }

            // Lấy thông tin khách hàng từ database
            var customer = db.Customers.FirstOrDefault(c => c.Username == username);
            if (customer == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Lấy danh sách sản phẩm từ database
            var products = db.Products.Where(p => cart.Contains(p.ProductID)).ToList();

            // Tạo đơn hàng
            var order = new Order
            {
                CustomerID = customer.CustomerID,
                OrderDate = DateTime.Now,
                TotalAmount = products.Sum(p => p.ProductPrice), // Tính tổng tiền
                AddressDelivery = address,
                PaymentStatus = "Pending" // Trạng thái mặc định
            };
            db.Orders.Add(order);
            db.SaveChanges();

            // Thêm chi tiết đơn hàng
            foreach (var product in products)
            {
                var orderDetail = new OrderDetail
                {
                    OrderID = order.OrderID,
                    ProductID = product.ProductID,
                    Quantity = 1,
                    UnitPrice = product.ProductPrice
                };
                db.OrderDetails.Add(orderDetail);
            }
            db.SaveChanges();

            // Xóa giỏ hàng sau khi đặt hàng thành công
            Session["Cart"] = null;

            // Chuyển hướng tới trang thành công
            return RedirectToAction("OrderSuccess");
        }

    }
}