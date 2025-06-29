using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using accgame.Models;
using PagedList;
using accgame.Filters;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]

    
    public class QNguoiDungController : Controller
    {
        private accgameEntities db = new accgameEntities();

        // GET: quantri/QNguoiDung
        public ActionResult Index(int? page, string search, int? ctv)
        {
            ViewBag.qnd = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1 && level != 3) || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            int pageNumber = (page ?? 1);
            int newpage = pageNumber - 1;
            decimal count;
            int last_page;
            ViewBag.search = search;
            ViewBag.ctvt = ctv;

            if (search == null && (ctv == null || ctv != 1))
            {
                ViewBag.nguoidung = "active";
                count = db.NguoiDungs.Count();
                last_page = Convert.ToInt16(Math.Ceiling(count / 40));
                ViewBag.last_page = last_page;
                ViewBag.page = pageNumber;
                return View(db.NguoiDungs.OrderByDescending(m => m.Tien).Skip(newpage * 40).Take(40).ToList());
            }
            if (ctv == 1)
            {
                ViewBag.ctv = "active";
                count = db.NguoiDungs.Where(m => m.LeverAdmin != 0).Count();
                last_page = Convert.ToInt16(Math.Ceiling(count / 40));
                ViewBag.last_page = last_page;
                ViewBag.page = pageNumber;
                return View(db.NguoiDungs.Where(m => m.LeverAdmin != 0).OrderByDescending(m => m.Tien).Skip(newpage * 40).Take(40).ToList());
            }

            count = db.NguoiDungs.Where(m => m.TenNguoiDung.Contains(search) || m.Email.Contains(search)).Count();
            last_page = Convert.ToInt16(Math.Ceiling(count / 40));
            ViewBag.last_page = last_page;
            ViewBag.page = pageNumber;
            ViewBag.nguoidung = "active";
            
            return View(db.NguoiDungs.Where(m => m.TenNguoiDung.Contains(search) || m.Email.Contains(search) || m.IDNguoiDung.ToString().Contains(search)).OrderByDescending(m => m.Tien).Skip(newpage * 40).Take(40).ToList());
        }

        public ActionResult DanhSachCong(int? page, string search)
        {
            ViewBag.qnd = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1 && level != 3) || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            ViewBag.search = search;
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            var data = db.AdminCongTiens.OrderByDescending(m => m.IdAdminCongTien).Include(m => m.NguoiDung).Include(m => m.NguoiDung1).ToPagedList(pageNumber, pageSize);

            if (search != null & search != "")
            {
                data = db.AdminCongTiens.Where(m => m.Lydo.Contains(search) || m.NguoiDung.TenNguoiDung.Contains(search) || m.NguoiDung1.TenNguoiDung.Contains(search)).OrderByDescending(m => m.IdAdminCongTien).Include(m => m.NguoiDung).Include(m => m.NguoiDung1).ToPagedList(pageNumber, pageSize);

            }
            return View(data);

        }



        // GET: quantri/quanlynguoidung/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.qnd = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1 && level != 3) || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NguoiDung nguoiDung = db.NguoiDungs.Find(id);
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }
            var accMua = db.Accs.Where(m => m.IDNguoiMua == id).ToList().Count();
            ViewBag.accMua = accMua;
            var accRdMua = db.AccRandoms.Where(m => m.IDNguoiMua == id).ToList().Count();
            ViewBag.accRdMua = accRdMua;
            var LuotCayThua = db.CayThues.Where(m => m.IDNguoiDung == id).ToList().Count();
            ViewBag.LuotCayThua = LuotCayThua;
            var LuotNapGame = db.NapGames.Where(m => m.IDNguoiDung == id).ToList().Count();
            ViewBag.LuotNapGame = LuotNapGame;


            return View(nguoiDung);
        }


        /*
                [HttpPost]
                public ActionResult AdminCongTien(int id, int sotien, string lido, bool congtien)
                {
                    int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                    int level = Convert.ToInt16(Session["levelAdmin"]);
                    if ((level != 1 && level != 3) || idnd == 0)
                    {
                        return Redirect("/");
                    }
                    //phan quyen
                    var nguoidung = db.NguoiDungs.Find(id);
                    if (nguoidung == null)
                    {
                        return Json(new { message = "Người dùng không tồn tại" });
                    }
                    var layThoiGian = DateTime.Now;
                    var tienNguoiDung = Convert.ToInt32(nguoidung.Tien);

                    var adminCongTien = new AdminCongTien();
                    adminCongTien.SoTien = sotien;
                    adminCongTien.IDNguoiCong = idnd;
                    adminCongTien.IDNguoiDung = id;
                    adminCongTien.Lydo = lido;
                    adminCongTien.ThoiGian = layThoiGian;
                    adminCongTien.CongTien = congtien;
                    adminCongTien.SoDuTruocCong = tienNguoiDung;
                    if (congtien == true)
                    {
                        adminCongTien.SoDuSauCong = tienNguoiDung + sotien;
                    }
                    else
                    {
                        adminCongTien.SoDuSauCong = tienNguoiDung - sotien;
                    }
                    db.AdminCongTiens.Add(adminCongTien);
                    db.SaveChanges();
                    return Json(new { message = "success" });
                }
        */

        [HttpPost]
        public ActionResult AdminCongTien(int id, int sotien, string lido, bool congtien)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1 && level != 3) || idnd == 0)
            {
                return Json(new { message = "unauthorized" }); // Trả về JSON với thông báo không được phép
            }

            var nguoidung = db.NguoiDungs.Find(id);
            if (nguoidung == null)
            {
                return Json(new { message = "Người dùng không tồn tại" }); // Trả về JSON với thông báo lỗi
            }

            var layThoiGian = DateTime.Now;
            var tienNguoiDung = Convert.ToInt32(nguoidung.Tien);

            var adminCongTien = new AdminCongTien
            {
                SoTien = sotien,
                IDNguoiCong = idnd,
                IDNguoiDung = id,
                Lydo = lido,
                ThoiGian = layThoiGian,
                CongTien = congtien,
                SoDuTruocCong = tienNguoiDung,
                SoDuSauCong = congtien ? tienNguoiDung + sotien : tienNguoiDung - sotien
            };

            db.AdminCongTiens.Add(adminCongTien);
            db.SaveChanges();

            return Json(new { message = "success" }); // Trả về JSON với thông báo thành công
        }


        public ActionResult BienDongSoDu(int id, int start = 0, int length = 10, string search = "", int draw = 0, string sortColumn = "IDBienDongSoDu", string sortDirection = "asc")
        {
            IQueryable<BienDongSoDu> bienDongSoDuQuery = db.BienDongSoDus.Where(m => m.IDNguoiDung == id);

            if (!string.IsNullOrEmpty(search))
            {
                bienDongSoDuQuery = bienDongSoDuQuery.Where(m => m.LoiNhan.Contains(search));
            }

            // Default sort order
            bienDongSoDuQuery = sortDirection == "asc" ? bienDongSoDuQuery.OrderByDescending(m => m.IDBienDongSoDu) : bienDongSoDuQuery.OrderByDescending(m => m.IDBienDongSoDu);

            int totalRecords = bienDongSoDuQuery.Count();
            int recordsFiltered = totalRecords; // Update this if there's actual filtering logic

            // Ensure sorting before Skip and Take
            if (!string.IsNullOrEmpty(sortColumn))
            {
                bienDongSoDuQuery = sortDirection == "asc"
                    ? bienDongSoDuQuery.OrderByDescending(m => m.IDBienDongSoDu)
                    : bienDongSoDuQuery.OrderByDescending(m => m.IDBienDongSoDu);
            }

            var data = bienDongSoDuQuery.Select(m => new
            {
                IDBienDongSoDu = m.IDBienDongSoDu,
                SoTien = m.SoTien,
                LoiNhan = m.LoiNhan,
                TienTruoc = m.TienTruoc,
                TienSau = m.TienSau,
                ThoiGian = m.ThoiGian,
                Tien = m.NguoiDung.Tien
            }).Skip(start).Take(length).ToList();

            var result = new
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = recordsFiltered,
                data = data
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


    }
}
