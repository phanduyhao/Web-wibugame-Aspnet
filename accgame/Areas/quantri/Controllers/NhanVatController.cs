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
    
    public class NhanVatController : Controller
    {

        // GET: quantri/NhanVat
        public ActionResult Index(string loaigame)
        {
            using (var db = new accgameEntities())
            {
                ViewBag.nhanvat = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }
                var dsnhanvat = db.NhanVats.Include(m => m.LoaiGame1).OrderByDescending(m => m.IDNhanVat).ToList();
                ViewBag.loaiGame = db.LoaiGames.ToList();
                ViewBag.idLoaiGame = Convert.ToInt16(loaigame);
                //phan quyen
                if (loaigame == null || loaigame == "")
                {
                    return View(dsnhanvat);
                }
                int intloaigame = Convert.ToInt16(loaigame);
                dsnhanvat = dsnhanvat.Where(m => m.LoaiGame == intloaigame).OrderByDescending(m => m.IDNhanVat).ToList();
                if (Request.UrlReferrer != null)
                {
                    TempData["PreviousUrl"] = Request.UrlReferrer.ToString();
                }
                return View(dsnhanvat);
            }

        }
        [HttpPost]
        public ActionResult Index(string loaigame, string TenNhanVat, string Avatar, int Sao)
        {
            using (var db = new accgameEntities())
            {
                ViewBag.nhanvat = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }
                var dsnhanvat = db.NhanVats.OrderByDescending(m => m.IDNhanVat).ToList();
                //phan quyen
                if (loaigame == null || loaigame == "")
                {
                    return View(dsnhanvat);
                }
                NhanVat nhanvat = new NhanVat();
                nhanvat.TenNhanVat = TenNhanVat;
                nhanvat.Avatar = Avatar;
                nhanvat.Sao = Sao;

                int intloaigame = Convert.ToInt16(loaigame);
                nhanvat.LoaiGame = intloaigame;
                db.NhanVats.Add(nhanvat);
                db.SaveChanges();
                if (intloaigame == 1)
                {
                    ViewBag.Honkai = "selected";
                }
                if (intloaigame == 2)
                {
                    ViewBag.Genshin = "selected";
                }
                dsnhanvat = dsnhanvat.Where(m => m.LoaiGame == intloaigame).OrderByDescending(m => m.IDNhanVat).ToList();
                TempData["SuccessThem"] = true;

                return Redirect("/quantri/nhanvat?loaigame=" + loaigame);
            }
        }

        public ActionResult sua (int id)
        {
            using (var db = new accgameEntities())
            {
                var nhanVat = db.NhanVats.Where(m => m.IDNhanVat == id).FirstOrDefault();
                ViewBag.loaiGame = db.LoaiGames.Where(m => m.Hide != true).ToList();
                return View(nhanVat);
            }
        }
        [HttpPost]
        public ActionResult sua(NhanVat nhanVat)
        {
            using (var db = new accgameEntities())
            {
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }
                //phan quyen
                
                NhanVat setNV = db.NhanVats.Find(nhanVat.IDNhanVat);
                setNV.Avatar = nhanVat.Avatar;
                setNV.TenNhanVat = nhanVat.TenNhanVat;
                setNV.Sao = nhanVat.Sao;
                setNV.Avatar = nhanVat.Avatar;
                db.SaveChanges();
                ViewBag.notify = "Chỉnh sửa thành công!";
                ViewBag.loaiGame = db.LoaiGames.Where(m => m.Hide != true).ToList();
                TempData["SuccessSua"] = true;

                string previousUrl = TempData["PreviousUrl"] as string;
                if (!string.IsNullOrEmpty(previousUrl))
                {
                    return Redirect(previousUrl);
                }
                return View(setNV);
            }
        }

        public ActionResult xoa(int id)
        {
            using (var db = new accgameEntities())
            {
                ViewBag.nhanvat = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }
                //phan quyen
                var setnhanvat = db.NhanVats.Find(id);
                db.NhanVats.Remove(setnhanvat);
                db.SaveChanges();
                TempData["SuccessXoa"] = true;

                string previousUrl = TempData["PreviousUrl"] as string;
                if (!string.IsNullOrEmpty(previousUrl))
                {
                    return Redirect(previousUrl);
                }
                return RedirectToAction("index");
            }

        }
    }
}