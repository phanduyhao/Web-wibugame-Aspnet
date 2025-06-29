using accgame.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Models;
using accgame.Filters;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    
    public class quanlydoanhthuController : Controller
    {
        // GET: quantri/quanlydoanhthu
        public ActionResult Index(int? page, string search)
        {
            ViewBag.qldt = "active";
            int pageNumber = (page ?? 1);
            int newpage = pageNumber - 1;

            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1) || idnd == 0)
            {
                return Redirect("/");
            }
            var datetime = DateTime.Now;
            var ngay = datetime.Day;
            var thang = datetime.Month;
            var nam = datetime.Year;
            DateTime dauThang = Convert.ToDateTime(thang + "/01/" + nam);
            DateTime dauNgay = Convert.ToDateTime(thang.ToString("00") + "/" + ngay.ToString("00") + "/" + nam.ToString("0000"));
            //phan quyen
            using (var db = new accgameEntities())
            {
                var thongke = (from nguoiDung in db.NguoiDungs.Where(m => m.LeverAdmin == 1 || m.LeverAdmin == 2)
                                              join acc in db.Accs.Where(m => m.Xoa != true) on nguoiDung.IDNguoiDung equals acc.IDNguoiDung into accGroup
                                              join bienDongSoDu in db.BienDongSoDus.Where(m => m.TruTien != true && m.LoiNhan != "Nạp CARD" && m.LoiNhan != "Nạp MOMO" && m.LoiNhan != "Nạp ATM") on nguoiDung.IDNguoiDung equals bienDongSoDu.IDNguoiDung into bdsdGroup
                                              join cayThue in db.CayThues on nguoiDung.IDNguoiDung equals cayThue.IDNguoiNhan into ctGroup
                                              select new ThongKe
                                              {
                                                  IDNguoiDung = nguoiDung.IDNguoiDung,
                                                  TenNguoiDung = nguoiDung.TenNguoiDung,
                                                  Email = nguoiDung.Email,
                                                  DoanhThuHomNay = bdsdGroup.Where(m => m.ThoiGian > dauNgay).Sum(x => x.SoTien),
                                                  DoanhThuThang = bdsdGroup.Where(m => m.ThoiGian > dauThang).Sum(x => x.SoTien),
                                                  TongDoanhThu = bdsdGroup.Sum(x => x.SoTien),
                                                  AccBanHomNay = accGroup.Where(m => m.ThoiGianBan > dauNgay).Count(),
                                                  AccBanThangNay = accGroup.Where(m => m.ThoiGianBan > dauThang).Count(),
                                                  TongAccBan = accGroup.Where(m => m.DaBan == true).Count(),
                                                  SoAccDaDang = accGroup.Count(),
                                                  CayThueHoanThanh = ctGroup.Where(m => m.HoanThanh == true).Count(),
                                                  TyLeHoaHong = nguoiDung.Phantramhoahong,
                                              });
                CTV ctv = new CTV()
                {
                    thongKes = thongke.OrderByDescending(m => m.IDNguoiDung).Skip(newpage * 40).Take(40).ToList(),
                };
                decimal count = thongke.Count();
                if (search != null)
                {
                    ctv.thongKes = thongke.Where(m => m.IDNguoiDung.ToString().Contains(search) || m.TenNguoiDung.Contains(search) || m.Email.Contains(search)).OrderByDescending(m => m.IDNguoiDung).Skip(newpage * 40).Take(40).ToList();
                    count = thongke.Where(m => m.IDNguoiDung.ToString().Contains(search) || m.TenNguoiDung.Contains(search) || m.Email.Contains(search)).Count();
                }
                int last_page = Convert.ToInt16(Math.Ceiling(count / 40));
                ViewBag.last_page = last_page;
                ViewBag.page = pageNumber;
                return View(ctv);
            }
        }
    }
}