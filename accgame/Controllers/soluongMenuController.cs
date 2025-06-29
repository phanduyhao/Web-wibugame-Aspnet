using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using accgame.Models;
namespace accgame.Controllers
{
    public class soluongMenuController : Controller
    {
        // GET: /soluongMenu
        public ActionResult rutien()
        {
            using (var db = new accgameEntities())
            {
                ViewBag.soluongmn = db.RutTiens.Where(m => m.TrangThai == null).ToList().Count();
                return View();
            }
        }

        public ActionResult adminCongTien()
        {
            using (var db = new accgameEntities())
            {
                ViewBag.soluongmn = db.AdminCongTiens.Where(m => m.Status != true && m.HuyDon != true).ToList().Count();
                return View();
            }
        }

        public ActionResult napgame()
        {
            using (var db = new accgameEntities())
            {
                ViewBag.soluongng = db.NapGames.Where(m => m.HoanThanh == null).ToList().Count();
                return View();
            }
        }

        public ActionResult ctvCayThue()
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (idnd == 0 || (level != 1 && level != 2))
            {
                return Content("");
            }
            using (var db = new accgameEntities())
            {
                ViewBag.soluongct = db.CayThues.Where(m => m.HoanThanh == null).ToList().Count();
                return View();
            }
        }
    }
}