using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using System.Data.Entity;
using PagedList;
using accgame.Filters;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    
    public class quanlynapController : Controller
    {
        // GET: quantri/quanlynap
        /* public ActionResult Index(int? page, string search)
         {
             ViewBag.lsnt = "active";
             int pageSize = 20;
             int pageNumber = page ?? 1;

             int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
             int level = Convert.ToInt16(Session["levelAdmin"]);
             if (level != 1 || idnd == 0)
             {
                 return Redirect("/");
             }
             ViewBag.search = search;
             //phan quyen
             using (var db = new accgameEntities())
             {

                 var data = db.NapTiens.OrderByDescending(m => m.IdNapTien).Include(m => m.NguoiDung).ToPagedList(pageNumber, pageSize);
                 if (search != null)
                 {
                      data = db.NapTiens.Where(m => m.Magd != null && (m.NguoiDung.Email.Contains(search) || m.NguoiDung.IDNguoiDung.ToString().Contains(search) || m.NguoiDung.TenNguoiDung.Contains(search)) ).OrderByDescending(m => m.IdNapTien).Include(m => m.NguoiDung).ToPagedList(pageNumber, pageSize);
                 }
                 else
                 {
                     data = db.NapTiens.Where(m => m.Magd != null).OrderByDescending(m => m.IdNapTien).Include(m => m.NguoiDung).ToPagedList(pageNumber, pageSize);
                 }
                 return View(data);
             }
         }*/

        public ActionResult Index(int? page, string searchMa, string searchND, string loai)
        {
            ViewBag.lsnt = "active";
            int pageSize = 20;
            int pageNumber = page ?? 1;

            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }


            using (var db = new accgameEntities())
            {
                var query = db.NapTiens.AsQueryable();
                if (!string.IsNullOrEmpty(loai))
                {
                    if (loai == "atm" || loai == "momo")
                    {
                        query = query.Where(m => m.Noidung.Contains(loai));
                    }
                }
                if (!string.IsNullOrEmpty(searchMa))
                {
                    query = query.Where(m => m.NguoiDung.IDNguoiDung.ToString().Contains(searchMa));
                }

                if (!string.IsNullOrEmpty(searchND))
                {
                    query = query.Where(m => m.NguoiDung.TenNguoiDung.Equals(searchND));
                }

                var count = query.Count();
                var data = query.Where(m => m.Magd != null)
                                .OrderByDescending(m => m.IdNapTien)
                                .Include(m => m.NguoiDung)
                                .ToPagedList(pageNumber, pageSize);

                int lastPage = Convert.ToInt16(Math.Ceiling(count / (double)pageSize));

                ViewBag.searchMa = searchMa;
                ViewBag.searchND = searchND;
                ViewBag.loai = loai;
                ViewBag.lastPage = lastPage;
                ViewBag.page = pageNumber;

                if (Request.UrlReferrer != null)
                {
                    TempData["PreviousUrl"] = Request.UrlReferrer.ToString();
                }

                return View(data);
            }
        }


        public ActionResult NapTheCao(int? page, string searchMa, string searchND)
        {
            ViewBag.lsntc = "active";
            int pageSize = 20;
            int pageNumber = page ?? 1;

            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            using (var db = new accgameEntities())
            {
                var query = db.NapTiens.AsQueryable();
                if (!string.IsNullOrEmpty(searchMa))
                {
                    query = query.Where(m => m.NguoiDung.IDNguoiDung.ToString().Contains(searchMa));
                }

                if (!string.IsNullOrEmpty(searchND))
                {
                    query = query.Where(m => m.NguoiDung.TenNguoiDung.Equals(searchND));
                }

                var count = query.Count();
                var data = query.Where(m => m.Magd == null)
                                .OrderByDescending(m => m.IdNapTien)
                                .Include(m => m.NguoiDung)
                                .ToPagedList(pageNumber, pageSize);

                int lastPage = Convert.ToInt16(Math.Ceiling(count / (double)pageSize));

                ViewBag.searchMa = searchMa;
                ViewBag.searchND = searchND;
                ViewBag.lastPage = lastPage;
                ViewBag.page = pageNumber;

                if (Request.UrlReferrer != null)
                {
                    TempData["PreviousUrl"] = Request.UrlReferrer.ToString();
                }

                return View(data);
            }
        }

    }
}