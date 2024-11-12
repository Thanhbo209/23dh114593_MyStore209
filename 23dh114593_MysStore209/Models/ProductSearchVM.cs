using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _23dh114593_MysStore209.Models
{
    public class ProductSearchVM
    {
        // Tiêu chí tìm kiếm theo tên hoặc mô tả sản phẩm
        public string SearchTerm { get; set; }

        // Tiêu chí tìm kiếm theo giá
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // Thứ tự sắp xếp (VD: "PriceAsc", "PriceDesc")
        public string SortOrder { get; set; }

        // Thuộc tính hỗ trợ phân trang
        public int PageNumber { get; set; } = 1; // Trang hiện tại (mặc định trang 1)
        public int PageSize { get; set; } = 10; // Số sản phẩm mỗi trang

        // Danh sách sản phẩm đã phân trang
        public IPagedList<Product> Products { get; set; }
        public int? CategoryID { get; set; } // ID danh mục
    }
}