using _23dh114593_MysStore209.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _23dh114593_MysStore209.Areas.Admin.Controllers
{
    public class ProductManagementController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();
        // GET: Admin/ProductManagement
        // GET: Admin/ProductManagement/Index
        public ActionResult Index()
        {
            // Lấy danh sách sản phẩm
            var products = db.Products.ToList();
            return View(products);
        }
        // GET: Admin/ProductManagement/Create
        public ActionResult Create()
        {
            ViewBag.Categories = db.Categories.ToList();
            return View();
        }

        // POST: Admin/ProductManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra nếu `CategoryID` hợp lệ
                if (!db.Categories.Any(c => c.CategoryID == model.CategoryID))
                {
                    ModelState.AddModelError("CategoryID", "Danh mục không hợp lệ.");
                    ViewBag.Categories = db.Categories.ToList();
                    return View(model);
                }

                // Thêm sản phẩm vào cơ sở dữ liệu
                db.Products.Add(model);
                db.SaveChanges();

                // Điều hướng đến danh sách sản phẩm
                return RedirectToAction("Index");
            }

            // Nếu không hợp lệ, tải lại danh mục
            ViewBag.Categories = db.Categories.ToList();
            return View(model);
        }
        public ActionResult Edit(int id)
        {
            System.Diagnostics.Debug.WriteLine("ProductID được truyền vào: " + id);
            var product = db.Products.FirstOrDefault(p => p.ProductID == id);
            if (product == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy sản phẩm
            }
            ViewBag.Categories = db.Categories.ToList(); // Lấy danh mục để hiển thị
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product model)
        {
            if (ModelState.IsValid)
            {
                // Tìm sản phẩm cần cập nhật
                var product = db.Products.FirstOrDefault(p => p.ProductID == model.ProductID);

                if (product == null)
                {
                    return HttpNotFound();
                }

                // Cập nhật thông tin sản phẩm
                product.ProductName = model.ProductName;
                product.ProductDecription = model.ProductDecription;
                product.ProductPrice = model.ProductPrice;
                product.ProductImage = model.ProductImage;
                product.CategoryID = model.CategoryID;

                db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                return RedirectToAction("Index");
            }

            ViewBag.Categories = db.Categories.ToList(); // Nếu lỗi, tải lại danh mục
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var product = db.Products.FirstOrDefault(p => p.ProductID == id);

            if (product == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy sản phẩm
            }

            return View(product); // Hiển thị thông tin sản phẩm để xác nhận xóa
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var product = db.Products.FirstOrDefault(p => p.ProductID == id);

            if (product != null)
            {
                db.Products.Remove(product); // Xóa sản phẩm
                db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
            }

            return RedirectToAction("Index");
        }
    }
    
}