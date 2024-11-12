using _23dh114593_MysStore209.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _23dh114593_MysStore209.Areas.Admin.Data;
namespace _23dh114593_MysStore209.Areas.Admin.Controllers
{
    public class AccountManagementController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();
        // GET: Admin/AccountManagement
        public ActionResult Index()
        {
            // Lấy danh sách tài khoản
            var accounts = db.Users
                .Select(u => new
                {
                    u.Username,
                    u.UserRole,
                    CustomerInfo = db.Customers.FirstOrDefault(c => c.Username == u.Username)
                })
                .ToList()
                .Select(a => new AccountViewModel
                {
                    Username = a.Username,
                    UserRole = a.UserRole,
                    CustomerName = a.CustomerInfo?.CustomerName,
                    CustomerEmail = a.CustomerInfo?.CustomerEmail,
                    CustomerAddress = a.CustomerInfo?.CustomerAddress,
                    CustomerPhone = a.CustomerInfo?.CustomerPhone
                });

            return View("ViewPage1", accounts);
        }

        // Tạo Admin Account
        // GET: 
        public ActionResult Create()
        {
            return View();
        }

        // POST: 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra tên đăng nhập đã tồn tại
                if (db.Users.Any(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }

                // Tạo đối tượng User
                var user = new User
                {
                    Username = model.Username,
                    Password = model.Password.Trim(), // Mã hóa mật khẩu nếu cần
                    UserRole = model.UserRole
                };

                // Lưu dữ liệu vào cơ sở dữ liệu
                db.Users.Add(user);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Admin/AccountManagement/Edit
        public ActionResult Edit(string username)
        {
            var user = db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return HttpNotFound();

            var model = new AccountViewModel
            {
                Username = user.Username,
                UserRole = user.UserRole,
                
            };

            return View(model);
        }

        // POST: Admin/AccountManagement/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.FirstOrDefault(u => u.Username == model.Username);
                if (user == null) return HttpNotFound();

                // Cập nhật thông tin tài khoản
                user.Password = model.Password.Trim();
                user.UserRole = model.UserRole;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Admin/AccountManagement/Delete
        public ActionResult Delete(string username)
        {
            // Tìm tài khoản trong bảng Users
            var user = db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return HttpNotFound();

            // Xóa dữ liệu liên quan trong bảng Customer trước
            var customer = db.Customers.FirstOrDefault(c => c.Username == username);
            if (customer != null)
            {
                db.Customers.Remove(customer);
            }

            // Xóa tài khoản trong bảng Users
            db.Users.Remove(user);

            db.SaveChanges();

            return RedirectToAction("Index");
        }
        
       
    }
}