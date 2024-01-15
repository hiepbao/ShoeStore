using ShoeStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoeStore.Areas.Admin.Controllers
{
    public class ThongKeController : Controller
    {
        ShoeStoreEntities db = new ShoeStoreEntities();
        // GET: Admin/ThongKe
        public ActionResult Index()
        {
            ViewBag.Year = new SelectList(db.ThongKe_SelectYear(), "Nam");
            return View();
        }

        public JsonResult Receipt_MonthChart(int year)
        {
            var result = db.Statistic_Receipt_Month(year).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Icome()
        {
            ViewBag.Year = new SelectList(db.ThongKe_SelectYear(), "Nam");
            return View();
        }

        public JsonResult Income_MonthChart(int year)
        {
            var result = db.Statistic_Income_Month(year).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}