using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShoeStore.Models;

namespace ShoeStore.Areas.Admin.Controllers
{
    public class ReceiptController : Controller
    {
        private ShoeStoreEntities db = new ShoeStoreEntities();

        // GET: Admin/Receipt/DanhSachDonHang
        public ActionResult OrderList()
        {
            var Receipt = db.Receipts;
            return View(Receipt.ToList());
        }

        //GET: Admin/Receipt/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            else
            {
                var receipt = db.Receipt_Details(id).SingleOrDefault();
                ViewBag.Details = receipt;
                return View();
            }
        }

        // POST: Admin/Receipt/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            db.Receipt_Delete(id);
            db.SaveChanges();
            return RedirectToAction("OrderList");
        }

        //GET: Admin/Receipt/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            else
            {
                int idreceiptt = Convert.ToInt32(id);
                var DetailList = db.DetailReceipts.Where(n => n.ReceiptId == idreceiptt).Include(h=>h.Receipt);
                ViewBag.Detailss = DetailList.ToList();
                var Receipt = db.Receipts;
                ViewBag.Receipts = Receipt.ToList();
                return View();
            }
        }

        // POST: Admin/Receipt/Details/5
        //Confirm Receipt
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public ActionResult Details(int id)
        {
            db.Receipt_Confirm(id);
            db.SaveChanges();
            return RedirectToAction("OrderList");
        }

    }
}