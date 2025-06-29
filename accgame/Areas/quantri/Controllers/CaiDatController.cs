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
    
    public class CaiDatController : Controller
    {
        // GET: quantri/CaiDat
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TangThoiGian(string TaiKhoan, string MatKhau)
        {
            if(TaiKhoan != "haihai")
            {
                return RedirectToAction("Index");
            }
            if(MatKhau != "havani_hoyo")
            {
                return RedirectToAction("Index");
            }
            using (var db = new accgameEntities())
            {
                ViewBag.TaiKhoan = TaiKhoan;
                ViewBag.MatKhau = MatKhau;
                ViewBag.thoiHan = db.CaiDats.Where(m => m.MaCaiDat == "Thoi_Han").FirstOrDefault()?.ThoiGianHet ?? null;
            }
            return View();
        }

        [HttpPost]
        public ActionResult TangThoiGian(int thoigian, string TaiKhoan, string MatKhau)
        {
            if (TaiKhoan != "haihai")
            {
                return RedirectToAction("Index");
            }
            if (MatKhau != "havani_hoyo")
            {
                return RedirectToAction("Index");
            }
            using (var db = new accgameEntities())
            {
                ViewBag.TaiKhoan = TaiKhoan;
                ViewBag.MatKhau = MatKhau;
                
                ViewBag.loi = "Thành Công";
                var caidat = db.CaiDats.Where(m => m.MaCaiDat == "Thoi_Han").FirstOrDefault();
                if(caidat == null)
                {
                    DateTime ngayHienTai = DateTime.Now;
                    CaiDat setCaiDat = new CaiDat();
                    setCaiDat.ThoiGianHet = ngayHienTai.AddDays(thoigian);
                    setCaiDat.MaCaiDat = "Thoi_Han";
                    setCaiDat.NoiDung = "Thời Hạn";
                    db.CaiDats.Add(setCaiDat);
                    db.SaveChanges();
                }
                else
                {
                    DateTime thoiGianHet = caidat.ThoiGianHet ?? DateTime.Now;
                    var setCaiDat = db.CaiDats.Find(caidat.IDCaiDat);
                    setCaiDat.ThoiGianHet = thoiGianHet.AddDays(thoigian);
                    db.SaveChanges();
                }
                ViewBag.thoiHan = db.CaiDats.Where(m => m.MaCaiDat == "Thoi_Han").FirstOrDefault()?.ThoiGianHet ?? null;
            }
            return View();
        }

        
    }
}