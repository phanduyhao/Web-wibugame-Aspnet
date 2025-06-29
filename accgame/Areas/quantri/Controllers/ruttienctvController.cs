using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using PagedList;
using accgame.Filters;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    
    public class ruttienctvController : Controller
    {
        accgameEntities db = new accgameEntities();
        // GET: quantri/ruttienctv
        public ActionResult Index()
        {
            ViewBag.ruttienctv = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (idnd == 0 || (level != 1 && level != 2 && level != 3))
            {
                return Redirect("/");
            }
            ViewBag.hoahong = Convert.ToInt32(db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).FirstOrDefault().Tien);
            //phan quyen
            if (Request.UrlReferrer != null)
            {
                TempData["PreviousUrl"] = Request.UrlReferrer.ToString();
            }
            return View();
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = "SoTienRut, NganHang, NguoiNhan, SoTaiKhoan")] RutTien rutTien)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (idnd == 0 || (level != 1 && level != 2 && level != 3))
            {
                return Redirect("/");
            }
            //phan quyen
            rutTien.IDNguoiDung = idnd;
            var nguoidung = db.NguoiDungs.Find(idnd);
            var tienNguoiDung = Convert.ToInt32(nguoidung.Tien);
            if(Convert.ToInt32(rutTien.SoTienRut) < 100000)
            {
                ViewBag.loi = "Số tiền rút phải lớn hơn 100.000!";
                return View();
            }
            if(Convert.ToInt32(tienNguoiDung) < rutTien.SoTienRut)
            {
                ViewBag.loi = "không đủ số dư để rút!";
                return View();
            }
            rutTien.ThoiGian = DateTime.Now;
            
            nguoidung.Tien = tienNguoiDung - rutTien.SoTienRut;
            db.RutTiens.Add(rutTien);
            //Biến động số dư:
            var biendongds = new BienDongSoDu();
            biendongds.IDNguoiDung = nguoidung.IDNguoiDung;
            biendongds.SoTien = rutTien.SoTienRut;
            biendongds.TruTien = true;
            biendongds.LoiNhan = "Rút tiền CTV";
            biendongds.ThoiGian = DateTime.Now;
            biendongds.TienTruoc = tienNguoiDung;
            biendongds.TienSau = tienNguoiDung - rutTien.SoTienRut;
            db.BienDongSoDus.Add(biendongds);
            //

            db.SaveChanges();
            string previousUrl = TempData["PreviousUrl"] as string;
            if (!string.IsNullOrEmpty(previousUrl))
            {
                return Redirect(previousUrl);
            }
            return RedirectToAction("DanhSach");
        }


        /*        public ActionResult DanhSach()
                {
                    ViewBag.ruttienctv = "active";
                    int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                    int level = Convert.ToInt16(Session["levelAdmin"]);
                    if (idnd == 0 || (level != 1 && level != 2 && level != 3))
                    {
                        return Redirect("/");
                    }
                    //phan quyen
                    var danhsach = db.RutTiens.Where(m => m.IDNguoiDung == idnd).OrderByDescending(m => m.IDRutTien).ToList();
                    ViewBag.hoahong = Convert.ToInt32(db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).FirstOrDefault().Tien);
                    ViewBag.phantramhoahong = Convert.ToInt32(db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).FirstOrDefault().Phantramhoahong);
                    return View(danhsach);
                }*/


        public ActionResult DanhSach(int? page, int? rows, int? searchID, string trangthai)
        {
            ViewBag.ruttienctv = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (idnd == 0 || (level != 1 && level != 2 && level != 3))
            {
                return Redirect("/");
            }

            // Thiết lập số dòng mặc định
            int pageSize = rows ?? 10; // Mặc định là 10 dòng nếu không có giá trị được chọn
            int pageNumber = page ?? 1; // Mặc định là trang 1 nếu không có giá trị được chọn

            var query = db.RutTiens.AsQueryable();

            // Tìm kiếm theo mã số
            if (searchID.HasValue)
            {
                query = query.Where(m => m.IDRutTien == searchID.Value);
            }

            if (!string.IsNullOrEmpty(trangthai))
            {
                if (trangthai == "1")
                {
                    query = query.Where(m => m.TrangThai == true);
                }
                if (trangthai == "0")
                {
                    query = query.Where(m => m.TrangThai == false);
                }
                if (trangthai == "null")
                {
                    query = query.Where(m => m.TrangThai == null);
                }
            }

            var danhsach = query.Where(m => m.IDNguoiDung == idnd)
                .OrderByDescending(m => m.IDRutTien)
                .ToPagedList(pageNumber, pageSize); // Phân trang

            ViewBag.Rows = pageSize;
            ViewBag.SearchID = searchID;
            ViewBag.trangthai = trangthai;
            ViewBag.hoahong = Convert.ToInt32(db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).FirstOrDefault().Tien);
            ViewBag.phantramhoahong = Convert.ToInt32(db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).FirstOrDefault().Phantramhoahong);
            ViewBag.page = pageNumber;
            ViewBag.last_page = danhsach.PageCount;

            return View(danhsach);
        }
    
    
    }
}