using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using accgame.Filters;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    
    public class quanlycodeController : Controller
    {
        // GET: quantri/quanlycode
        public ActionResult Index()
        {
            ViewBag.qlcode = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            using (var db = new accgameEntities())
            {
                var loaicode = db.LoaiCodes.Where(m => m.Xoa != true).Include(m => m.LoaiGame1).OrderBy(m => m.LoaiGame).ThenByDescending(m => m.IDLoaiCode).ToList();
                return View(loaicode);
            }
        }

        public ActionResult themmoi()
        {
            ViewBag.qlcode = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            using (var db = new accgameEntities()){

                ViewBag.LoaiGame = db.LoaiGames.ToList();
                return View();
            }

        }

        [HttpPost]
        public ActionResult Themmoi(LoaiCode loaiCode)
        {
            using (var db = new accgameEntities())
            {
                ViewBag.qlcode = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }
                //phan quyen
                db.LoaiCodes.Add(loaiCode);
                db.SaveChanges();
                return RedirectToAction("index");
            }

        }

        public ActionResult sua(int id)
        {
            using (var db = new accgameEntities())
            {
                ViewBag.qlcode = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }
                //phan quyen
                var loaicode = db.LoaiCodes.Where(m => m.IDLoaiCode == id).FirstOrDefault();
                ViewBag.LoaiGame = db.LoaiGames.ToList();
                return View(loaicode);
            }
        }

        [HttpPost]
        public ActionResult sua(int id, LoaiCode loaiCode)
        {
            using (var db = new accgameEntities())
            {
                ViewBag.qlcode = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }
                //phan quyen

                var setloaicode = db.LoaiCodes.Find(id);
                if (setloaicode == null)
                {
                    return RedirectToAction("index");
                }
                setloaicode.TenLoaiCode = loaiCode.TenLoaiCode;
                setloaicode.Gia = loaiCode.Gia;
                setloaicode.CodeNoiBo = loaiCode.CodeNoiBo;
                setloaicode.LoaiGame = loaiCode.LoaiGame;
                db.SaveChanges();
                return RedirectToAction("index");
            }
        }

        public ActionResult xoa(int id)
        {
            using (var db = new accgameEntities())
            {
                ViewBag.qlcode = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }
                //phan quyen
                var loaicode = db.LoaiCodes.Find(id);
                loaicode.Xoa = true;
                db.SaveChanges();
                return RedirectToAction("index"); 
            }
        }

        public ActionResult danhsach(int id, int? page)
        {
            ViewBag.qlcode = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }

            //phan quyen
            ViewBag.idloaicode = id;
            int pageNumber = (page ?? 1);
            int newpage = pageNumber - 1;
            
            using (var db = new accgameEntities())
            {
                ViewBag.tenloaicode = db.LoaiCodes.Where(m => m.IDLoaiCode == id).FirstOrDefault().TenLoaiCode;
                var listCode = db.Codes.Where(m => m.IDLoaiCode == id && m.Xoa != true && m.DaBan != true).OrderByDescending(m => m.IDLoaiCode).Skip(newpage * 40).Take(40).ToList();

                decimal count = db.Codes.Where(m => m.IDLoaiCode == id && m.Xoa != true && m.DaBan != true).Count();
                int last_page = Convert.ToInt16(Math.Ceiling(count / 40));
                ViewBag.last_page = last_page;
                ViewBag.page = pageNumber;
                ViewBag.iddanhmuc = id;
                return View(listCode);
            }
        }

        public ActionResult themcode(int id)
        {
            ViewBag.qlcode = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            ViewBag.loaicode = id;
            return View();
        }

        [HttpPost]
        public ActionResult themcode(int id, string thongtin)
        {
            ViewBag.qlcode = "active";
            using (var db = new accgameEntities())
            {
                var dscode = thongtin.Split('\n');
                foreach (var item in dscode)
                {
                    var code = new Code();
                    code.MaCode = item;
                    code.IDLoaiCode = id;
                    db.Codes.Add(code);
                }
                db.SaveChanges();
                ViewBag.loaicode = id;
                ViewBag.thongbao = "Thêm mới thành công!";
                return View();
            }
        }

        public ActionResult xoacode(int id)
        {
            using (var db = new accgameEntities())
            {
                ViewBag.qlcode = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }
                //phan quyen
                var code = db.Codes.Find(id);
                int idloaicode = Convert.ToInt32(code.IDLoaiCode);
                code.Xoa = true;
                db.SaveChanges();
                return RedirectToAction("danhsach", new {id = idloaicode });
            }
        }

        public ActionResult danhsachdaban(int id, int? page)
        {
            ViewBag.qlcode = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }

            //phan quyen
            ViewBag.idloaicode = id;
            int pageNumber = (page ?? 1);
            int newpage = pageNumber - 1;
            using (var db = new accgameEntities())
            {
                ViewBag.tenloaicode = db.LoaiCodes.Where(m => m.IDLoaiCode == id).FirstOrDefault().TenLoaiCode;
                var listCode = db.Codes.Where(m => m.IDLoaiCode == id && m.Xoa != true && m.DaBan == true).Include(m => m.NguoiDung).OrderByDescending(m => m.IDCode).Skip(newpage * 40).Take(40).ToList();

                decimal count = db.Codes.Where(m => m.IDLoaiCode == id && m.Xoa != true && m.DaBan == true).Count();
                int last_page = Convert.ToInt16(Math.Ceiling(count / 40));
                ViewBag.last_page = last_page;
                ViewBag.page = pageNumber;
                ViewBag.iddanhmuc = id;
                return View(listCode);
            }
        }
    }
}