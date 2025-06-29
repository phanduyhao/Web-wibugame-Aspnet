using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using accgame.Filters;

namespace accgame.Controllers
{
    [CheckSessionID]

    public class CayThueController : Controller
    {
        // GET: CayThue
        public ActionResult danhsach()
        {
            return View();
        }
        public ActionResult Index()
        {
            using (var db = new accgameEntities())
            {
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                Session["url"] = "/CayThue/";
                if (idnd == 0)
                {
                    return Redirect("/dangnhap");
                }
                var cayThue = db.CayThues.Include(m => m.LoaiGame1).Where(m => m.IDNguoiDung == idnd).OrderByDescending(m => m.IDCayThue).Take(20).ToList();
                return View(cayThue);
            }
        }

        public ActionResult themmoi(int id)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            Session["url"] = "/CayThue/themmoi/" + id;
            if (idnd == 0)
            {
                return Redirect("/dangnhap");
            }
            
            using (var db = new accgameEntities())
            {
                var loaiGame = db.LoaiGames.Where(m => m.IDLoaiGame == id).FirstOrDefault();
                ViewBag.loaigame = loaiGame.TenLoaiGame;
                ViewBag.idLoaiGame = loaiGame.IDLoaiGame;

                var caidat = db.CaiDats;
                var facebook = caidat.Where(m => m.MaCaiDat == "facebook").FirstOrDefault();
                ViewBag.facebook = facebook?.NoiDung ?? "";
                var goinhiemvu = db.GoiNhiemVus.Where(m => m.Xoa != true && m.LoaiGame == id).ToList();
                return View(goinhiemvu);
            }
            
        }
        [HttpPost]
        public ActionResult themmoi([Bind(Include = "IDGoiNhiemVu, UID, TenDangNhap, MatKhau, Server, TenNhanVat, SoDienThoai, GhiChu")] CayThue cayThue, int id)
        {
            using (var db = new accgameEntities())
            {
                var loaiGame = db.LoaiGames.Where(m => m.IDLoaiGame == id).FirstOrDefault();
                ViewBag.loaigame = loaiGame.TenLoaiGame;
                ViewBag.idLoaiGame = loaiGame.IDLoaiGame;

                var caidat = db.CaiDats;
                var facebook = caidat.Where(m => m.MaCaiDat == "facebook").FirstOrDefault();
                ViewBag.facebook = facebook?.NoiDung ?? "";


                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                Session["url"] = "/CayThue/themmoi";
                if (idnd == 0)
                {
                    return Redirect("/dangnhap");
                }
                var goinhiemvu = db.GoiNhiemVus.Where(m => m.Xoa != true).ToList();
                var nguoidung = db.NguoiDungs.Find(idnd);
                var tienTruoc = Convert.ToInt32(nguoidung.Tien);
                ViewBag.UID = cayThue.UID;
                ViewBag.TenDangNhap = cayThue.TenDangNhap;
                ViewBag.MatKhau = cayThue.MatKhau;
                ViewBag.Server = cayThue.Server;
                ViewBag.TenNhanVat = cayThue.TenNhanVat;
                ViewBag.SoDienThoai = cayThue.SoDienThoai;
                ViewBag.GhiChu = cayThue.GhiChu;
                int sotien = db.GoiNhiemVus.Where(m => m.IDGoiNhiemVu == cayThue.IDGoiNhiemVu).FirstOrDefault().GiaTien ?? 0;
                if (tienTruoc < sotien)
                {
                    ViewBag.loi = "Không đủ tiền để thực hiện giao dịch!";
                    return View(goinhiemvu) ;
                }
                nguoidung.Tien = tienTruoc - sotien;
                cayThue.IDNguoiDung = idnd;
                cayThue.ThoiGianGui = DateTime.Now;
                cayThue.SoTien = sotien;
                cayThue.LoaiGame = id;
                db.CayThues.Add(cayThue);
                //Biến động số dư:
                var biendongds = new BienDongSoDu();
                biendongds.IDNguoiDung = nguoidung.IDNguoiDung;
                biendongds.SoTien = sotien;
                biendongds.TruTien = true;
                biendongds.ThoiGian = DateTime.Now;
                biendongds.LoiNhan = "Cày thuê";
                biendongds.TienTruoc = tienTruoc;
                biendongds.TienSau = nguoidung.Tien;
                db.BienDongSoDus.Add(biendongds);
                //
                db.SaveChanges();
                return RedirectToAction("Index");
            }

        }
    }
}