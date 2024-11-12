using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _23dh114593_MysStore209.Models;
using PagedList;


namespace _23dh114593_MysStore209.Controllers
{
    public class ProductsController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities(); // Database context
        // Action tìm kiếm và phân trang sản phẩm
        public ActionResult Search(ProductSearchVM model)
        {


            // Lấy danh sách sản phẩm từ cơ sở dữ liệu
            var products = db.Products.AsQueryable();

            // Lọc theo tiêu chí tìm kiếm
            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                products = products.Where(p => p.ProductName.Contains(model.SearchTerm) ||
                                               p.ProductDecription.Contains(model.SearchTerm));
            }

            if (model.MinPrice.HasValue)
            {
                products = products.Where(p => p.ProductPrice >= model.MinPrice.Value);
            }

            if (model.MaxPrice.HasValue)
            {
                products = products.Where(p => p.ProductPrice <= model.MaxPrice.Value);
            }

            // Lọc theo danh mục sản phẩm (nếu có)
            if (model.CategoryID.HasValue)
            {
                products = products.Where(p => p.CategoryID == model.CategoryID);
            }

            // Sắp xếp theo tiêu chí
            switch (model.SortOrder)
            {
                case "PriceAsc":
                    products = products.OrderBy(p => p.ProductPrice);
                    break;
                case "PriceDesc":
                    products = products.OrderByDescending(p => p.ProductPrice);
                    break;
                case "NameAsc":
                    products = products.OrderBy(p => p.ProductName);
                    break;
                case "NameDesc":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;
                default:
                    products = products.OrderBy(p => p.ProductName);
                    break;
            }

            // Phân trang
            model.Products = products.ToPagedList(model.PageNumber, model.PageSize);

            // Trả về View với dữ liệu đã phân trang
            return View(model);
        }

        // Chi tiết sản phẩm
        public ActionResult Details(int id)
        {
            // Lấy thông tin sản phẩm dựa trên ID
            var product = db.Products.FirstOrDefault(p => p.ProductID == id);

            if (product == null)
            {
                return HttpNotFound(); // Trả về lỗi 404 nếu không tìm thấy sản phẩm
            }

            return View(product); // Trả về View hiển thị chi tiết sản phẩm
        }
    }
}