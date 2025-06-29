using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;

namespace accgame.Controllers
{
    public class dangkyController : Controller
    {
        // GET: dangky
        public ActionResult Index()
        {
            using (var db = new accgameEntities())
            {
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                if (idnd != 0)
                {
                    return Redirect("/");
                }
                return View();
            }

        }
        [HttpPost]
        public ActionResult Index(string TenNguoiDung, string MatKhau, string Email, string NhapLaiMatKhau)
        {
            using (var db = new accgameEntities())
            {
                TenNguoiDung = TenNguoiDung.ToLower();
                var searchTen = db.NguoiDungs.Where(m => m.TenNguoiDung == TenNguoiDung).FirstOrDefault();
                ViewBag.TenNguoiDung = TenNguoiDung;
                ViewBag.NhapLaiMatKhau = NhapLaiMatKhau;
                ViewBag.MatKhau = MatKhau;
                ViewBag.Email = Email;
                if (searchTen != null)
                {
                    ViewBag.loi = "Tên đăng nhập đã tồn tại!";
                    return View();
                }
                var searchEmail = db.NguoiDungs.Where(m => m.Email == Email).FirstOrDefault();
                if (searchEmail != null)
                {
                    ViewBag.loi = "Email đã tồn tại!";
                    return View();
                }
                if (TenNguoiDung.Length >= 15)
                {
                    ViewBag.loi = "Tên đăng nhập không vượt quá 14 ký tự!";
                    return View();
                }
                if (TenNguoiDung.Length < 6)
                {
                    ViewBag.loi = "Tên đăng nhập không được dưới 6 ký tự!";
                    return View();
                }
                NguoiDung setNguoiDung = new NguoiDung();
                setNguoiDung.TenNguoiDung = TenNguoiDung;
                setNguoiDung.MatKhau = MatKhau;
                setNguoiDung.Email = Email;
                setNguoiDung.Tien = 0;
                setNguoiDung.NgayThamGia = DateTime.Now;
                setNguoiDung.LeverAdmin = 0;
                db.NguoiDungs.Add(setNguoiDung);
                db.SaveChanges();
                return Redirect("/dangnhap");
            }
                
        }
    }
}