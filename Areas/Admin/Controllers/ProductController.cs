using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Microsoft.Ajax.Utilities;
using ShoeStore.Common;
using ShoeStore.Entities;
using ShoeStore.Models;
using BCryptNet = BCrypt.Net.BCrypt;

namespace ShoeStore.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        // GET: Admin/Product


        public ActionResult Manager(int? size, int? page, string sortProperty, string searchString, string sortOrder = "")
        {
            ShoeStoreEntities db = new ShoeStoreEntities();
            var products = db.GetProducts(null, null).ToList();
            ViewBag.Products = products;
            //Lọc danh sách sản phẩm phù hợp với từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                var search = db.SearchProductByName(searchString);
                ViewBag.Products = search;
            }

            return View();
        }

        public ActionResult ThemSanPham()
        {
            ShoeStoreEntities db = new ShoeStoreEntities();
            var ListProduct = db.GetProducts(null, null);
            ViewBag.them = ListProduct;
            ViewBag.cate = db.GetCategory();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemSanPham([Bind(Include = "ProductName, ProductPrice, ProductCategory, Description")] GetProductDetail_Result product)
        {
            try
            {
                ShoeStoreEntities db = new ShoeStoreEntities();
                var existingProduct = db.Products.FirstOrDefault(p => p.ProductName == product.ProductName);
                if (existingProduct != null)
                {
                    TempData["SuccessMessage"] = "Tên sản phẩm đã tồn tại!";
                    return RedirectToAction("manager", "Product");
                }

                db.InsertProduct(product.ProductName, product.ProductPrice, product.ProductCategory, product.Description);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Thêm thành công!";
            }
            catch
            {
                TempData["SuccessMessage"] = "Lỗi";
            }

            return RedirectToAction("manager", "Product");
        }

        public ActionResult SuaSanPham(int? id)
        {
            ShoeStoreEntities db = new ShoeStoreEntities();
            var ListProduct = db.GetProductDetail(id).Single();
            ViewBag.sua = ListProduct;
            ViewBag.cate = db.GetCategory();
            return View();
        }

        [HttpPost]
        public ActionResult SuaSanPham(HttpPostedFileBase anh)
        {
            ShoeStoreEntities db = new ShoeStoreEntities();
            int Id = int.Parse(Request.Form["ProductID"]);
            var Name = Request.Form["ProductName"];
            double Price = double.Parse(Request.Form["ProductPrice"]);
            int Cate = int.Parse(Request.Form["ProductCategory"]);
            var Des = Request.Form["Description"];
            var ImageExtension = Request.Form["ImageExtension"];
            byte[] buffer = null;

            if (anh != null)
            {
                if (string.IsNullOrEmpty(ImageExtension))
                {
                    ModelState.AddModelError("ImageExtension", "Đuôi ảnh không được để trống");
                }
                else
                {
                    MemoryStream memoryStream = new MemoryStream();
                    anh.InputStream.CopyTo(memoryStream);
                    buffer = memoryStream.ToArray();

                }

            }
            db.UpdateProduct(Id, Name, Price, Cate, Des);
            db.UploadImageToProduct(buffer, ImageExtension, Id);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Sửa thành công!";
            return RedirectToAction("../Admin/Product/manager");


        }



        public ActionResult QLSanPham(int? id)
        {
            ShoeStoreEntities db = new ShoeStoreEntities();
            var ListProduct = db.GetProductDetail(id).SingleOrDefault();
            ViewBag.sua = ListProduct;
            var color = db.GetColor();
            ViewBag.Color = color;
            var size = db.GetSize();
            ViewBag.Size = size;
            ViewBag.TypeList = db.Get_All_ProductCS(id).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult UpdateSP(int id, int Productid, int size, int color, int quantity)
        {
            ShoeStoreEntities db = new ShoeStoreEntities();
            try
            {
                db.Update_ProductCS_Quantity(id, Productid, size, color, quantity);
                TempData["SuccessMessage"] = "Cập nhật thành công!";
            }
            catch
            {
                TempData["ErrorMessage"] = "Lỗi";
            }

            return RedirectToAction("QLSanPham", "Product", new { area = "Admin", id = Productid });
        }

        [HttpPost]
        public ActionResult ThemThongTin(int? id, int size, int color, int quantity)
        {
            ShoeStoreEntities db = new ShoeStoreEntities();
            try
            {
                var result = db.addSizeColor(id, size, color, quantity).Single();
                if (result == null || result == -1)
                {
                    TempData["SuccessMessage"] = "Màu và size của sản phẩm đã tồn tại!";
                }
                else
                {
                    TempData["SuccessMessage"] = "Thêm thành công! ";
                }
            }
            catch
            {
                TempData["ErrorMessage"] = "Lỗi";
            }
            return RedirectToAction("QLSanPham", "Product", new { area = "Admin", id = id });
        }

        public ActionResult XoaSanPham(int? id)
        {
            ShoeStoreEntities db = new ShoeStoreEntities();
            var ListProduct = db.GetProductDetail(id).Single();
            ViewBag.xoa = ListProduct;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XoaSanPham(int id)
        {
            int Id = int.Parse(Request.Form["ProductID"]);
            ShoeStoreEntities db = new ShoeStoreEntities();
            try
            {
                db.DeleteProduct(Id);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Xóa thành công!";
            }
            catch
            {
                TempData["ErrorMessage"] = "Lỗi";
            }
            
            return RedirectToAction("Manager");
        }


        public ActionResult ProfileView()
        {
            var nguoidung = Session[CommonConstants.Cus] as getUserSession_Result;

            var id = nguoidung.CustomerUsername;
            ShoeStoreEntities db = new ShoeStoreEntities();
            var ListProduct = db.Customer_Details(id).SingleOrDefault();

            if (ListProduct == null)
            {

                return HttpNotFound();
            }
            ViewBag.sua = ListProduct;

            return View();
        }


        [HttpPost]
        public ActionResult ProfileView(string CustomerUsername)
        {
            ShoeStoreEntities db = new ShoeStoreEntities();
            CustomerUsername = Request.Form["CustomerUsername"];
            var CustomerName = Request.Form["CustomerName"];
            var CustomerEmail = Request.Form["CustomerEmail"];
            var CustomerPNum = Request.Form["CustomerPNum"];
            var CustomerAddress = (string)Request.Form["CustomerAddress"];

            

            // Kiểm tra các ô input không được để trống
            if (string.IsNullOrEmpty(CustomerName))
            {
                ModelState.AddModelError("tennhanvien", "Name cannot be empty.");
            }


            if (string.IsNullOrEmpty(CustomerEmail))
            {
                ModelState.AddModelError("email", "Email cannot be empty.");
            }
            else
            {
                // Kiểm tra mật khẩu có đủ điều kiện
                string pattern = "^[A-Za-z0-9]+[A-Za-z0-9]*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)+$";
                Regex regex = new Regex(pattern);
                if (!regex.IsMatch(CustomerEmail))
                {
                    ModelState.AddModelError("email", "\r\nInvalid email.");
                }
            }


            if (string.IsNullOrEmpty(CustomerPNum))
            {
                ModelState.AddModelError("sodienthoai", "Phone number cannot be empty.");
            }
            else
            {
                // Kiểm tra mật khẩu có đủ điều kiện
                string pattern = "^0[0-9]{8,10}$";
                Regex regex = new Regex(pattern);
                if (!regex.IsMatch(CustomerPNum))
                {
                    ModelState.AddModelError("sodienthoai", "Phone numbers must start with 0 and be between 9 and 11 digits long");
                }
            }
            if (string.IsNullOrEmpty(CustomerAddress))
            {
                ModelState.AddModelError("Address", "\r\nAddress cannot be empty.");
            }

            // Kiểm tra xem có lỗi hay không
            if (ModelState.IsValid)
            {
                // Tiến hành lưu thông tin
                var result = db.UpdateCustomer(CustomerUsername, CustomerName, CustomerEmail, CustomerPNum, CustomerAddress);

                // Kiểm tra kết quả lưu thành công
                if (result != null)
                {
                    TempData["SuccessMessage"] = "Save successfully.";
                    //TempData["SuccessMessage"] = "success";
                }


                return RedirectToAction("ProfileView");
            }

            // Nếu có lỗi, gán giá trị lại vào ViewBag.sua và hiển thị lại View
            var nguoidung = Session[CommonConstants.Cus] as getUserSession_Result;
            var id = nguoidung.CustomerUsername;
            ViewBag.sua = db.Customer_Details(id).SingleOrDefault();
            return View();
        }


        public ActionResult DoiMk()
        {
            ShoeStoreEntities db = new ShoeStoreEntities();
            var nguoidung = Session[CommonConstants.Cus] as getUserSession_Result;
            var id = nguoidung.CustomerUsername;
            var ListProduct  = db.Customer_Details(id).SingleOrDefault();
            if (ListProduct == null)
            {
                return HttpNotFound();
            }
            ViewBag.sua = ListProduct;

            return View();
        }


        [HttpPost]
        public ActionResult DoiMk(string MatKhau, string OldMatKhau, string CustomerUsername)
        {
            ShoeStoreEntities db = new ShoeStoreEntities();

            CustomerUsername = Request.Form["CustomerUsername"];
            MatKhau = Request.Form["MatKhau"];

            if (string.IsNullOrEmpty(MatKhau))
            {
                ModelState.AddModelError("MatKhau", "Please enter a new password.");
            }
            else
            {
                // Kiểm tra mật khẩu có đủ điều kiện
                string pattern = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$";
                Regex regex = new Regex(pattern);
                if (!regex.IsMatch(MatKhau))
                {
                    ModelState.AddModelError("MatKhau", "The password must contain at least one uppercase letter, one lowercase letter, one special character, and one number. Minimum length is 8 characters.");
                }
            }

            if (ModelState.IsValid)
            {
                string hashedPassword = BCryptNet.HashPassword(MatKhau);
                var result = db.Cus_DoiMK(CustomerUsername, hashedPassword);
                if (result != null)
                {
                    TempData["SuccessMessage"] = "Change password successfully";
                }
                return RedirectToAction("ProfileView", "Product", new { CustomerUsername = CustomerUsername });
            }

            var nguoidung = Session[CommonConstants.Cus] as getUserSession_Result;
            var id = nguoidung.CustomerUsername;
            ViewBag.sua = db.Customer_Details(id).SingleOrDefault();
            return View();
        }
    }
}