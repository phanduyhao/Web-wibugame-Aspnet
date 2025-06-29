using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using accgame.Filters;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    
    public class loadController : Controller
    {
        // GET: quantri/load
        public ActionResult loadNVHK()
        {
            using (var db = new accgameEntities())
            {
                var nhanvat = db.NhanVats.Where(m => m.LoaiGame == 1).ToList();
                return PartialView("_loadNV", nhanvat);
            }
        }

        public ActionResult loadNV(int id)
        {
            using (var db = new accgameEntities())
            {
                var nhanvat = db.NhanVats.Where(m => m.LoaiGame == id).ToList();
                return PartialView("_loadNV", nhanvat);
            }
        }
        public ActionResult loadVK(int id)
        {
            using (var db = new accgameEntities())
            {
                var vukhi = db.VuKhis.Where(m => m.LoaiGame == id).ToList();
                return PartialView("_loadVK", vukhi);
            }
        }

        public ActionResult loadNVGS()
        {
            using (var db = new accgameEntities())
            {
                var nhanvat = db.NhanVats.Where(m => m.LoaiGame == 2).ToList();
                return PartialView("_loadNV", nhanvat);
            }

        }

        public ActionResult loadVKHK()
        {
            using(var db = new accgameEntities())
            {
                var vukhi = db.VuKhis.Where(m => m.LoaiGame == 1).ToList();
                return PartialView("_loadVK", vukhi);
            }
        }

        public ActionResult loadVKGS()
        {
            using (var db = new accgameEntities())
            {
                var vuKhis = db.VuKhis.Where(m => m.LoaiGame == 2).ToList();
                return PartialView("_loadVK", vuKhis);
            }

        }

        // API lấy danh sách vũ khí của tài khoản
        public JsonResult GetDanhSachVuKhi(int idAcc)
        {
            using (var db = new accgameEntities())
            {
                var acc = db.Accs.Where(m => m.IDAcc == idAcc).FirstOrDefault();
                if (acc == null) return Json(new { success = false });


                if (string.IsNullOrEmpty(acc.VuKhi))
                {
                    return Json(new { success = true, data = new List<string>() }, JsonRequestBehavior.AllowGet);
                }

                var danhSachVuKhi = acc.VuKhi.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(s => s.Trim())
                                        .Where(s => !string.IsNullOrEmpty(s))
                                        .ToList();


                return Json(new { success = true, data = danhSachVuKhi }, JsonRequestBehavior.AllowGet);
            }
        }


        // API lấy danh sách nhân vật của tài khoản
        public JsonResult GetDanhSachNhanVat(int idAcc)
        {
            using (var db = new accgameEntities())
            {
                var acc = db.Accs.Where(m => m.IDAcc == idAcc).FirstOrDefault();
                if (acc == null) return Json(new { success = false });


                if (string.IsNullOrEmpty(acc.NhanVat))
                {
                    return Json(new { success = true, data = new List<string>() }, JsonRequestBehavior.AllowGet);
                }

                var danhSachNhanVat = acc.NhanVat.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries)
                                           .Select(s => s.Trim())
                                           .Where(s => !string.IsNullOrEmpty(s))
                                           .ToList();


                return Json(new { success = true, data = danhSachNhanVat }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}