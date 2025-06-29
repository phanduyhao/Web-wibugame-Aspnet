using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using PagedList;
using accgame.Filters;
using System.Data.Entity;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    
    public class quanlyruttienctvController : Controller
    {
        // GET: quantri/quanlyruttienctv
        public ActionResult Index(int? page, int? rows, int? searchID, string trangthai)
        {
            ViewBag.qlrt = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }

            // Thiết lập số dòng mặc định
            int pageSize = rows ?? 10; // Mặc định là 10 dòng nếu không có giá trị được chọn
            int pageNumber = page ?? 1; // Mặc định là trang 1 nếu không có giá trị được chọn

            using (var db = new accgameEntities())
            {
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

                var danhsach = query.Include(r => r.NguoiDung).OrderByDescending(m => m.IDRutTien)
                    .ToPagedList(pageNumber, pageSize); // Phân trang

                ViewBag.Rows = pageSize;
                ViewBag.SearchID = searchID;
                ViewBag.trangthai = trangthai;
                ViewBag.page = pageNumber;
                ViewBag.last_page = danhsach.PageCount;

                return View(danhsach);
            }
        }

        /*        public ActionResult thanhtoan(int id, int? rows)
                {
                    ViewBag.qlrt = "active";
                    int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                    int level = Convert.ToInt16(Session["levelAdmin"]);
                    if (level != 1 || idnd == 0)
                    {
                        return Redirect("/");
                    }
                    //phan quyen
                    using (var db = new accgameEntities())
                    {
                        var ruttien = db.RutTiens.Find(id);
                        ruttien.TrangThai = true;
                        db.SaveChanges();
                        string previousUrl = TempData["PreviousUrl"] as string;
                        if (!string.IsNullOrEmpty(previousUrl))
                        {
                            return Redirect(previousUrl);
                        }
                        return RedirectToAction("Index", new { rows = rows });
                    }
                }

                public ActionResult khongthanhtoan(int id, int? rows)
                {
                    ViewBag.qlrt = "active";
                    int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                    int level = Convert.ToInt16(Session["levelAdmin"]);
                    if (level != 1 || idnd == 0)
                    {
                        return Redirect("/");
                    }
                    //phan quyen
                    using (var db = new accgameEntities())
                    {
                        var ruttien = db.RutTiens.Find(id);
                        if(ruttien.TrangThai == null)
                        {
                            ruttien.TrangThai = false;
                            var setnguoidung = db.NguoiDungs.Find(ruttien.IDNguoiDung);
                            setnguoidung.Tien = setnguoidung.Tien + ruttien.SoTienRut;
                            db.SaveChanges();
                        }
                        string previousUrl = TempData["PreviousUrl"] as string;
                        if (!string.IsNullOrEmpty(previousUrl))
                        {
                            return Redirect(previousUrl);
                        }
                        return RedirectToAction("Index", new { rows = rows });
                    }
                }
            */


        [HttpPost]
        public JsonResult ThanhToan(int id)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            using (var db = new accgameEntities())
            {
                var ruttien = db.RutTiens.Find(id);
                if (ruttien == null)
                {
                    return Json(new { success = false, message = "Record not found" });
                }

                ruttien.TrangThai = true;
                db.SaveChanges();

                return Json(new { success = true, newStatus = "Hoàn thành" });
            }
        }

        [HttpPost]
        public JsonResult KhongThanhToan(int id)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            using (var db = new accgameEntities())
            {
                var ruttien = db.RutTiens.Find(id);
                if (ruttien == null || ruttien.TrangThai != null)
                {
                    return Json(new { success = false, message = "Record not found or already processed" });
                }

                ruttien.TrangThai = false;
                var user = db.NguoiDungs.Find(ruttien.IDNguoiDung);
                if (user != null)
                {
                    user.Tien += ruttien.SoTienRut;
                }
                db.SaveChanges();

                return Json(new { success = true, newStatus = "Đã hủy" });
            }
        }


    }
}