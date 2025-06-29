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
    
    public class VuKhiController : Controller
    {
        accgameEntities db = new accgameEntities();
        // GET: quantri/VuKhi
        public ActionResult Index(string loaigame)
        {

            ViewBag.vukhi = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            var dsvukhi = db.VuKhis.Include(m => m.LoaiGame1).OrderByDescending(m => m.LoaiGame).ThenByDescending(m => m.IDVuKhi).ToList();
            int intloaigame = Convert.ToInt16(loaigame);
            ViewBag.loaiGame = db.LoaiGames.ToList();
            ViewBag.idLoaiGame = intloaigame;
            //phan quyen
            if (loaigame == null || loaigame == "")
            {
                return View(dsvukhi);
            }
            
            dsvukhi = dsvukhi.Where(m => m.LoaiGame == intloaigame).OrderByDescending(m => m.IDVuKhi).ToList();
            if (Request.UrlReferrer != null)
            {
                TempData["PreviousUrl"] = Request.UrlReferrer.ToString();
            }
            return View(dsvukhi);
        }

        [HttpPost]
        public ActionResult Index(string loaigame, string TenVuKhi, string Avatar, int Sao)
        {

            ViewBag.vukhi = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            var dsvukhi = db.VuKhis.OrderByDescending(m => m.IDVuKhi).ToList();
            //phan quyen
            if (loaigame == null || loaigame == "")
            {
                return View(dsvukhi);
            }
            VuKhi vukhi = new VuKhi();
            vukhi.TenVuKhi = TenVuKhi;

            int intloaigame = Convert.ToInt16(loaigame);
            vukhi.LoaiGame = intloaigame;
            vukhi.Avatar = Avatar;
            vukhi.Sao = Sao;
            db.VuKhis.Add(vukhi);
            db.SaveChanges();
            if (intloaigame == 1)
            {
                ViewBag.Honkai = "selected";
            }
            if (intloaigame == 2)
            {
                ViewBag.Genshin = "selected";
            }
            dsvukhi = dsvukhi.Where(m => m.LoaiGame == intloaigame).OrderByDescending(m => m.IDVuKhi).ToList();
            TempData["SuccessThem"] = true;

            return Redirect("/quantri/vukhi?loaigame=" + loaigame);
        }

        public ActionResult sua(int id)
        {
            using (var db = new accgameEntities())
            {
                var vuKhi = db.VuKhis.Where(m => m.IDVuKhi == id).FirstOrDefault();
                ViewBag.loaiGame = db.LoaiGames.Where(m => m.Hide != true).ToList();
                return View(vuKhi);
            }
        }
        [HttpPost]
        public ActionResult sua(VuKhi vuKhi)
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
                VuKhi setVK = db.VuKhis.Find(vuKhi.IDVuKhi);
                var loaigame = setVK.LoaiGame;
                setVK.Avatar = vuKhi.Avatar;
                setVK.TenVuKhi = vuKhi.TenVuKhi;
                setVK.Sao = vuKhi.Sao;
                setVK.Avatar = vuKhi.Avatar;
                db.SaveChanges();
                ViewBag.notify = "Chỉnh sửa thành công!";
                TempData["SuccessSua"] = true;
                return Redirect("/quantri/vukhi?loaigame=" + loaigame);
            }
        }

        public ActionResult xoa(int id)
        {
            ViewBag.nhanvat = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            var setvukhi = db.VuKhis.Find(id);
            db.VuKhis.Remove(setvukhi);
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