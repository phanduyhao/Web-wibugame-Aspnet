using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using PagedList;
using accgame.Filters;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    
    public class giaithuongvongquayController : Controller
    {
        // GET: quantri/giaithuongvongquay
        public ActionResult Index(int? page)
        {
            int pageSize = 30;
            int pageNumber = (page ?? 1);
            ViewBag.giaithuongvongquay = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            using (var db = new accgameEntities())
            {
                var giaithuong = db.GiaiThuongVongQuays.Where(m => m.DaBan != true).OrderByDescending(m => m.IDGiaiThuongVongQuay).ToPagedList(pageNumber, pageSize);
                return View(giaithuong);
            }
        }

        public ActionResult loaigiai(int id, int? page)
        {
            ViewBag.id = id;
            int pageSize = 30;
            int pageNumber = (page ?? 1);
            ViewBag.giaithuongvongquay = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            if(id == 1)
            {
                ViewBag.loaiGiai = "CHAR 5* + WP";
            }
            else if (id == 2)
            {
                ViewBag.loaiGiai = "CHAR LIMITED";
            }
            else if (id == 3)
            {
                ViewBag.loaiGiai = "ONE CHAR 5*";
            }
            else if (id == 4)
            {
                ViewBag.loaiGiai = ">2 CHAR 5*";
            }
            else if (id == 5)
            {
                ViewBag.loaiGiai = "ACC REROLL";
            }
            else if (id == 6)
            {
                ViewBag.loaiGiai = "ACC REROLL VIP";
            }
            using (var db = new accgameEntities())
            {
                var giaithuong = db.GiaiThuongVongQuays.Where(m => m.DaBan != true && m.LoaiGiai == id).OrderByDescending(m => m.IDGiaiThuongVongQuay).ToPagedList(pageNumber, pageSize);
                return View(giaithuong);
            }
        }

        public ActionResult taomoi()
        {
            ViewBag.giaithuongvongquay = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            return View();
        }

        [HttpPost]
        public ActionResult taomoi(string thongtintaikhoan, int LoaiGiai)
        {
            ViewBag.giaithuongvongquay = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            using (var db = new accgameEntities())
            {
                var dstk = thongtintaikhoan.Split('\n');
                foreach(var item in dstk)
                {
                    var giaithuong = new GiaiThuongVongQuay();
                    giaithuong.LoaiGiai = LoaiGiai;
                    giaithuong.ThongTin = item;
                    db.GiaiThuongVongQuays.Add(giaithuong);
                }
                db.SaveChanges();
                return RedirectToAction("loaigiai", new { id = LoaiGiai });
            }
        }

        public ActionResult edit(int id)
        {
            ViewBag.giaithuongvongquay = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            using (var db = new accgameEntities())
            {
                var gt = db.GiaiThuongVongQuays.Find(id);
                return View(gt);
            }
        }
        [HttpPost]
        public ActionResult edit(GiaiThuongVongQuay giaiThuongVongQuay)
        {
            ViewBag.giaithuongvongquay = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            using (var db = new accgameEntities())
            {
                var giaithuong = db.GiaiThuongVongQuays.Find(giaiThuongVongQuay.IDGiaiThuongVongQuay);
                giaithuong.ThongTin = giaiThuongVongQuay.ThongTin;
                giaithuong.LoaiGiai = giaiThuongVongQuay.LoaiGiai;
                return RedirectToAction("edit", new {id = giaiThuongVongQuay.IDGiaiThuongVongQuay});
            }
        }

        public ActionResult delete(int id)
        {
            ViewBag.giaithuongvongquay = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            using (var db = new accgameEntities())
            {
                var giaithuong = db.GiaiThuongVongQuays.Find(id);
                int loaigiai = giaithuong.LoaiGiai ?? 0;
                db.GiaiThuongVongQuays.Remove(giaithuong);
                db.SaveChanges();
                return RedirectToAction("loaigiai", new {id = loaigiai});
            }
        }
    }
}