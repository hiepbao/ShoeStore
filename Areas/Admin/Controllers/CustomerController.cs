using ShoeStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoeStore.Areas.Admin.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Admin/Customer
            ShoeStoreEntities db = new ShoeStoreEntities();
            // GET: Admin/Customer
            public ActionResult Index()
            {
                return View(db.Customer_List().ToList());
            }

            public ActionResult Details(string id)
            {
                if (id == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    var result = db.Customer_Details(id).SingleOrDefault();
                    ViewBag.Detail = result;
                    return View();
                }
            }
    }
}