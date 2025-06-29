using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using accgame.Models;
using accgame.Filters;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    
    public class TrangChuController : Controller
    {
        // GET: quantri/TrangChu
        public ActionResult Index()
        {
            ViewBag.dashboard = "active";

            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }

            using (var db = new accgameEntities())
            {
                var now = DateTime.Now;
                DateTime dauThang = new DateTime(now.Year, now.Month, 1);
                DateTime dauNgay = now.Date;

                // Query thông minh: không ToList sớm
                var nguoiDungs = db.NguoiDungs.AsQueryable();
                var naptien = db.NapTiens.AsQueryable();
                var napgame = db.NapGames.AsQueryable();
                var accRandoms = db.AccRandoms.Where(m => m.Xoa != true);
                var accs = db.Accs.Where(m => m.Xoa != true);
                var cayThue = db.CayThues.AsQueryable();
                var bienDongSoDu = db.BienDongSoDus.Where(m =>
                    (m.NguoiDung.LeverAdmin == 1 || m.NguoiDung.LeverAdmin == 2) &&
                    m.TruTien != true &&
                    m.LoiNhan != "Nạp CARD" && m.LoiNhan != "Nạp MOMO" && m.LoiNhan != "Nạp ATM"
                );

                // Đếm & Sum trực tiếp bằng LINQ to Entities
                var thanhVienQuery = nguoiDungs.Where(m => m.LeverAdmin != 1 && m.LeverAdmin != 2);
                var ctvQuery = nguoiDungs.Where(m => m.LeverAdmin == 2);

                DashModelss data = new DashModelss()
                {
                    thanhVien = thanhVienQuery.Count(),
                    tienThanhVien = thanhVienQuery.Sum(m => (decimal?)m.Tien) ?? 0m,
                    tienCTV = ctvQuery.Sum(m => (decimal?)m.Tien) ?? 0m,
                    tongTienDaNap = nguoiDungs.Sum(m => (decimal?)m.TienNap) ?? 0m,

                    bankNap = naptien.Count(m => m.Magd != null),
                    theNap = naptien.Count(m => m.Magd == null),
                    taiKhoanGame = accRandoms.Count() + accs.Count(),
                    taiKhoanGameDaBan = accRandoms.Count(m => m.DaBan == true) + accs.Count(m => m.DaBan == true),
                    tongDonNap = napgame.Count(),
                    tongDonNapHoanThanh = napgame.Count(m => m.HoanThanh == true),
                    tongDonCayThue = cayThue.Count(),
                    tongDonCTHoanThanh = cayThue.Count(m => m.HoanThanh == true),

                    doanhThuHomNay = Convert.ToInt32(bienDongSoDu.Where(m => m.ThoiGian >= dauNgay).Sum(m => (decimal?)m.SoTien) ?? 0),
                    tienNapHomNay = Convert.ToInt32(naptien.Where(m => m.thoigian >= dauNgay).Sum(m => (decimal?)m.SoTien) ?? 0),
                    napBankHomNay = Convert.ToInt32(naptien.Where(m => m.thoigian >= dauNgay && m.Magd != null).Sum(m => (decimal?)m.SoTien) ?? 0),
                    napTheHomNay = Convert.ToInt32(naptien.Where(m => m.thoigian >= dauNgay && m.Magd == null).Sum(m => (decimal?)m.SoTien) ?? 0),
                    taiKhoanDaDang = accs.Count(m => m.ThoiGianDang > dauNgay),
                    donBanNickHomNay = accs.Count(m => m.ThoiGianBan > dauNgay),

                    doanhThuThang = Convert.ToInt32(bienDongSoDu.Where(m => m.ThoiGian >= dauThang).Sum(m => (decimal?)m.SoTien) ?? 0),
                    tienNapThang = Convert.ToInt32(naptien.Where(m => m.thoigian >= dauThang).Sum(m => (decimal?)m.SoTien) ?? 0),
                    napBankThang = Convert.ToInt32(naptien.Where(m => m.thoigian >= dauThang && m.Magd != null).Sum(m => (decimal?)m.SoTien) ?? 0),
                    napTheThang = Convert.ToInt32(naptien.Where(m => m.thoigian >= dauThang && m.Magd == null).Sum(m => (decimal?)m.SoTien) ?? 0),
                    taiKhoanDaDangThang = accs.Count(m => m.ThoiGianDang > dauThang),
                    donBanNickThang = accs.Count(m => m.ThoiGianBan > dauThang),
                };

                // Chỉ truy vấn Cài Đặt 1 lần
                var thoiHanCaiDat = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "Thoi_Han");
                DateTime? thoiHan = thoiHanCaiDat?.ThoiGianHet;

                ViewBag.thoiHan = thoiHan;
                ViewBag.saphet = (thoiHan.HasValue ? (thoiHan.Value - now).Days : 0);
                ViewBag.ngayHet = thoiHan;

                return View(data);
            }
        }


    }
}