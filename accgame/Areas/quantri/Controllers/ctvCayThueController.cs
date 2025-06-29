using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using System.Data.Entity;
using accgame.Filters;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    
    public class ctvCayThueController : Controller
    {
        // GET: quantri/ctvCayThue
        public ActionResult Index()
        {
            ViewBag.ctvct = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (idnd == 0 || (level != 1 && level != 2))
            {
                return Redirect("/");
            }
            //phan quyen
            using (var db = new accgameEntities())
            {
                var caythue = db.CayThues.Include(m => m.LoaiGame1).Include(m => m.NguoiDung).Include(m => m.GoiNhiemVu).Include(m => m.NguoiDung1).Where(m => m.HoanThanh == null).ToList();
                return View(caythue);
            }
        }

        public ActionResult dondanhan()
        {
            ViewBag.ctvct = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (idnd == 0 || (level != 1 && level != 2))
            {
                return Redirect("/");
            }
            //phan quyen
            using (var db = new accgameEntities())
            {
                var caythue = db.CayThues.Include(m => m.NguoiDung).Include(m => m.GoiNhiemVu).Include(m => m.NguoiDung1).Where(m => m.IDNguoiNhan == idnd).OrderByDescending(m => m.IDCayThue).ToList();
                return View(caythue);
            }
        }
        public ActionResult nhandon(int id)
        {
            ViewBag.ctvct = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (idnd == 0 || (level != 1 && level != 2))
            {
                return Redirect("/");
            }
            using (var db = new accgameEntities())
            {
                var caythue = db.CayThues.Find(id);
                if (caythue.IDNguoiNhan == null && caythue.HoanThanh != true)
                {
                    caythue.IDNguoiNhan = idnd;
                    db.SaveChanges();
                }
                return RedirectToAction("dondanhan");
            }
        }

        public ActionResult hoanthanh(int id)
        {
            ViewBag.ctvct = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (idnd == 0 || (level != 1 && level != 2))
            {
                return Redirect("/");
            }
            using (var db = new accgameEntities())
            {
                var caythue = db.CayThues.Find(id);
                if (caythue.HoanThanh != null)
                {
                    return RedirectToAction("dondanhan");
                }
                if (caythue.IDNguoiNhan == idnd)
                {
                    caythue.HoanThanh = true;
                    caythue.ThoiGianHoanThanh = DateTime.Now;
                    var nguoidung = db.NguoiDungs.Find(idnd);
                    var tienTruoc = Convert.ToInt32(nguoidung.Tien);
                    int congtien = Convert.ToInt32(Convert.ToDouble(caythue.GoiNhiemVu.GiaTien) * nguoidung.Phantramhoahong/100.0);
                    nguoidung.Tien = tienTruoc + congtien;

                    //Biến động số dư:
                    var biendongds = new BienDongSoDu();
                    biendongds.IDNguoiDung = nguoidung.IDNguoiDung;
                    biendongds.SoTien = congtien;
                    biendongds.TruTien = false;
                    biendongds.LoiNhan = "CTV cày thuê";
                    biendongds.ThoiGian = DateTime.Now;
                    biendongds.TienTruoc = tienTruoc;
                    biendongds.TienSau = tienTruoc + congtien;
                    db.BienDongSoDus.Add(biendongds);
                    //
                    //Biến động số dư admin:
                    var admin = db.NguoiDungs.Where(m => m.LeverAdmin == 1).FirstOrDefault();
                    if(admin != null)
                    {
                        var tienAdmin = Convert.ToInt32(admin.Tien);
                        var biendongdsad = new BienDongSoDu();
                        biendongdsad.IDNguoiDung = admin.IDNguoiDung;
                        biendongdsad.SoTien = caythue.GoiNhiemVu.GiaTien - congtien;
                        biendongdsad.TruTien = false;
                        biendongdsad.LoiNhan = "Hoa hồng cày thuê";
                        biendongdsad.ThoiGian = DateTime.Now;
                        biendongdsad.TienTruoc = tienAdmin;
                        biendongdsad.TienSau = tienAdmin + (caythue.GoiNhiemVu.GiaTien - congtien);
                        db.BienDongSoDus.Add(biendongdsad);
                    }
                    //
                    db.SaveChanges();
                }
                return RedirectToAction("dondanhan");
            }
        }
        public ActionResult huydon(int id)
        {
            ViewBag.ctvct = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (idnd == 0 || (level != 1 && level != 2))
            {
                return Redirect("/");
            }
            using (var db = new accgameEntities())
            {
                var caythue = db.CayThues.Find(id);
                if(caythue.IDNguoiNhan == idnd && caythue.HoanThanh == null)
                {
                    caythue.IDNguoiNhan = null;
                    db.SaveChanges();
                }
                return RedirectToAction("dondanhan");
            }
        }
    }
}