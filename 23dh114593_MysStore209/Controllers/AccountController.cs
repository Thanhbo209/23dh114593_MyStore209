using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using _23dh114593_MysStore209.Models;

public class AccountController : Controller
{
    private MyStoreEntities db = new MyStoreEntities();

    //  mã hóa mật khẩu
    //private string HashPassword(string password)
    //{
    //    using (SHA256 sha256 = SHA256.Create())
    //    {
    //        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
    //        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    //    }
    //}

    // GET: Account/Register
    public ActionResult Register()
    {
        return View();
    }

    // POST: Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Kiểm tra tên đăng nhập đã tồn tại
            if (db.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                return View(model);
            }

            // Tạo User
            var user = new User
            {
                Username = model.Username,
                Password = model.Password.Trim(), // Lưu mật khẩu sau khi loại bỏ khoảng trắng
                UserRole = "C" // Vai trò khách hàng
            };

            // Tạo Customer
            var customer = new Customer
            {
                CustomerName = model.CustomerName,
                CustomerPhone = model.CustomerPhone,
                CustomerEmail = model.CustomerEmail,
                CustomerAddress = model.CustomerAddress,
                Username = model.Username
            };

            // Lưu dữ liệu vào cơ sở dữ liệu
            db.Users.Add(user);
            db.Customers.Add(customer);
            db.SaveChanges();

            // Chuyển hướng đến trang đăng nhập
            return RedirectToAction("Login");
        }

        return View(model);
    }
    // GET: Account/Login
    public ActionResult Login()
    {
        return View();
    }

    // POST: Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Tìm kiếm người dùng theo tên đăng nhập


            var user = db.Users.FirstOrDefault(u => u.Username == model.Username);
            if (user == null)
            {
                // Nếu không tìm thấy tên đăng nhập
                ModelState.AddModelError("", "Tên đăng nhập không tồn tại.");
                return View(model);
            }

            // Loại bỏ khoảng trắng dư thừa từ mật khẩu trong cơ sở dữ liệu trước khi so sánh


            string storedPassword = user.Password.Trim();
            if (storedPassword != model.Password)
            {
                // Nếu mật khẩu không khớp
                ModelState.AddModelError("", "Mật khẩu không đúng.");
                return View(model);
            }

            // Đăng nhập thành công, lưu thông tin vào Session
            Session["Username"] = user.Username;
            Session["UserRole"] = user.UserRole;

            // Điều hướng theo vai trò của người dùng
            if (user.UserRole == "C")
            {
                // Role là Customer => Điều hướng đến View/Home/Index
                return RedirectToAction("Index", "Home");
            }
            else if (user.UserRole == "A")
            {
                // Role là Admin => Điều hướng đến TrangAdmin.cshtml
                return RedirectToAction("TrangAdmin", "HomeAdmin", new { area = "Admin" });
            }
        }

        // Nếu ModelState không hợp lệ, quay lại view

        return View(model);
    }

    // Đăng xuất
    public ActionResult Logout()
    {
        Session.Clear();
        return RedirectToAction("Login");
    }
}
