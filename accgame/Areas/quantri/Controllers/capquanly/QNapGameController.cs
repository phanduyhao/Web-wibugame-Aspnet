using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using System.Data.Entity;
using accgame.Filters;
using PagedList;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    
    public class QNapGameController : Controller
    {
        // GET: quantri/QNapGame
/*        public ActionResult Index()
        {
            using (var db = new accgameEntities())
            {
                ViewBag.qlng = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if ((level != 1 && level != 3) || idnd == 0)
                {
                    return Redirect("/");
                }
                //phan quyen
                var napgame = db.NapGames.Where(m => m.HoanThanh == null).Include(m => m.NguoiDung1).Include(n => n.NguoiDung).Include(n => n.GoiNap).OrderByDescending(m => m.IDNapGame).ToList();
                return View(napgame);
            }
        }*/
        public ActionResult Index(int? page, int? rows, int? searchID, string hoanthanh, string nguoigui)
        {
            ViewBag.qlng = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1 && level != 3) || idnd == 0)
            {
                return Redirect("/");
            }

            // Thiết lập số dòng mặc định
            int pageSize = rows ?? 10; // Mặc định là 10 dòng nếu không có giá trị được chọn
            int pageNumber = page ?? 1; // Mặc định là trang 1 nếu không có giá trị được chọn

            using (var db = new accgameEntities())
            {
                var query = db.NapGames.AsQueryable();

                // Tìm kiếm theo mã số
                if (searchID.HasValue)
                {
                    query = query.Where(m => m.IDNapGame == searchID.Value);
                }

                if (!string.IsNullOrEmpty(hoanthanh))
                {
                    if (hoanthanh == "1")
                    {
                        query = query.Where(m => m.HoanThanh == true);
                    }
                    if (hoanthanh == "0")
                    {
                        query = query.Where(m => m.HoanThanh == false);
                    }
                    if (hoanthanh == "null")
                    {
                        query = query.Where(m => m.HoanThanh == null);
                    }
                }

                if (!string.IsNullOrEmpty(nguoigui))
                {
                    query = query.Where(m => m.NguoiDung.TenNguoiDung.Contains(nguoigui));
                }

                // Pagination
                var totalItems = query.Count();
                var napgame = query.Include(n => n.NguoiDung).Include(m => m.NguoiDung1).Include(m => m.NguoiDung2)
                                   .Include(n => n.GoiNap)
                                   .OrderByDescending(m => m.IDNapGame).ToPagedList(pageNumber, pageSize);

                ViewBag.Rows = pageSize;
                ViewBag.CurrentPage = pageNumber;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                ViewBag.page = pageNumber;
                ViewBag.last_page = napgame.PageCount;
                ViewBag.SearchID = searchID;
                ViewBag.hoanthanh = hoanthanh;
                ViewBag.nguoigui = nguoigui;
                return View(napgame);
            }
        }



/*        public ActionResult hoanthanh(int id)
        {
            ViewBag.qlng = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1 && level != 3) || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            using (var db = new accgameEntities())
            {
                var napgame = db.NapGames.Find(id);
                napgame.HoanThanh = true;
                var setCtv = db.NguoiDungs.Find(idnd);
                if (setCtv != null)
                {
                    var biendongdsad = new BienDongSoDu();
                    biendongdsad.IDNguoiDung = setCtv.IDNguoiDung;
                    biendongdsad.SoTien = napgame.SoTien;
                    biendongdsad.TruTien = false;
                    biendongdsad.ThoiGian = DateTime.Now;
                    biendongdsad.LoiNhan = "Hoàn Thành Nạp Game";
                    biendongdsad.TienTruoc = setCtv.Tien;
                    biendongdsad.TienSau = setCtv.Tien + napgame.SoTien;

                    db.BienDongSoDus.Add(biendongdsad);

                    setCtv.Tien += napgame.SoTien;
                }
                db.SaveChanges();
                return RedirectToAction("index");
            }
        }*/

        public ActionResult huydon(int id)
        {
            ViewBag.qlng = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1 && level != 3) || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            using (var db = new accgameEntities())
            {
                var napgame = db.NapGames.Find(id);
                napgame.IDNguoiDuyet = idnd;

                if (napgame.HoanThanh == null)
                {
                    napgame.HoanThanh = false;
                    napgame.IDNguoiDuyet = idnd;
                    var nguoidung = db.NguoiDungs.Find(napgame.IDNguoiDung);
                    var tienTruoc = Convert.ToInt32(nguoidung.Tien);

                    nguoidung.Tien = tienTruoc + napgame.SoTien;

                    //Biến động số dư nguoi mua:
                    var biendongdsnm = new BienDongSoDu();
                    biendongdsnm.IDNguoiDung = nguoidung.IDNguoiDung;
                    biendongdsnm.SoTien = napgame.SoTien;
                    biendongdsnm.TruTien = false;
                    biendongdsnm.ThoiGian = DateTime.Now;
                    biendongdsnm.LoiNhan = "Hoàn tiền nạp game";
                    biendongdsnm.TienTruoc = tienTruoc;
                    biendongdsnm.TienSau = tienTruoc + napgame.SoTien;
                    db.BienDongSoDus.Add(biendongdsnm);
                    //
                    //Biến động số dư admin:
                    var admin = db.NguoiDungs.Where(m => m.LeverAdmin == 1).FirstOrDefault();
                    if (admin != null)
                    {
                        var biendongdsad = new BienDongSoDu();
                        biendongdsad.IDNguoiDung = admin.IDNguoiDung;
                        biendongdsad.SoTien = napgame.SoTien;
                        biendongdsad.TruTien = true;
                        biendongdsad.ThoiGian = DateTime.Now;
                        biendongdsad.LoiNhan = "Hoàn tiền nạp game";
                        biendongdsad.TienTruoc = admin.Tien;
                        biendongdsad.TienSau = admin.Tien - napgame.SoTien;

                        db.BienDongSoDus.Add(biendongdsad);

                        var setAdmin = db.NguoiDungs.Find(admin.IDNguoiDung);
                        if (setAdmin != null)
                        {
                            setAdmin.Tien -= (napgame.SoTien);
                        }
                    }


                    

                    db.SaveChanges();
                }
                var nguoiDuyet = db.NguoiDungs.Find(idnd);
                string tenNguoiDuyet = nguoiDuyet?.TenNguoiDung ?? "Không xác định";
                return Json(new { success = true, newStatus = "Đã hủy", nguoiDuyet = tenNguoiDuyet });
            }
        }

        public ActionResult Daxuly(int? page)
        {
            using (var db = new accgameEntities())
            {
                int pageNumber = (page ?? 1);
                int newpage = pageNumber - 1;

                ViewBag.qlng = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if ((level != 1 && level != 3) || idnd == 0)
                {
                    return Redirect("/");
                }
                //phan quyen
                var napgame = db.NapGames.Where(m => m.HoanThanh != null && m.IDNguoiDuyet == idnd).Include(n => n.NguoiDung).Include(n => n.GoiNap).Include(m => m.NguoiDung1).OrderByDescending(m => m.IDNapGame).Skip(newpage * 40).Take(40).ToList();

                decimal count = db.NapGames.Where(m => m.HoanThanh != null).Count();
                var last_page = Convert.ToInt16(Math.Ceiling(count / 40));
                ViewBag.last_page = last_page;
                ViewBag.page = pageNumber;
                return View(napgame);
            }
        }
    }
}