using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.UI;
using accgame.Context;
using accgame.Filters;
using PagedList;

namespace accgame.Areas.quantri.Controllers
{
    public class quanlythongbaoController : Controller
    {
        private accgameEntities db = new accgameEntities();

        // GET: quantri/quanlynguoidung
        public ActionResult Index()
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            ViewBag.thongbao = "active";

            //phan quyen
            ViewBag.thongbaoadmin = db.NotifyUsers.Where(m => m.IsAdminPost == true).OrderByDescending(m => m.IdNotify).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)] // Cho phép HTML input cho phương thức này
        public ActionResult PostThongBao(string contents)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);

            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }

            using (var db = new accgameEntities())
            {
                if (!string.IsNullOrEmpty(contents))
                {
                    var notify = new NotifyUser
                    {
                        IdNguoidung = null,
                        IdAdmin = idnd,
                        IsAdminPost = true,
                        Contents = contents,
                        IsRead = false,
                        Thoigian = DateTime.Now
                    };

                    db.NotifyUsers.Add(notify);
                    db.SaveChanges();
                    TempData["success"] = "Thông báo đã được đăng thành công.";
                }
                else
                {
                    TempData["error"] = "Nội dung thông báo không được để trống.";
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteNotification(int id)
        {
            try
            {
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);

                if (level != 1 || idnd == 0)
                {
                    return Json(new { success = false, message = "Bạn không có quyền xóa thông báo này!" });
                }

                // Tìm thông báo cần xóa
                var notify = db.NotifyUsers.Find(id);
                if (notify == null || notify.IsAdminPost != true)
                {
                    return Json(new { success = false, message = "Thông báo không tồn tại hoặc không phải do admin đăng!" });
                }

                // Xóa các bản ghi liên quan trong bảng AdminNotifyRead
                var relatedReads = db.AdminNotifyReads.Where(r => r.IdNotifyUser == id).ToList();
                if (relatedReads.Any())
                {
                    db.AdminNotifyReads.RemoveRange(relatedReads);
                }

                // Xóa bản ghi trong bảng NotifyUsers
                db.NotifyUsers.Remove(notify);
                db.SaveChanges();

                return Json(new { success = true, message = "Thông báo và dữ liệu liên quan đã được xóa thành công!" });
            }
            catch (Exception ex)
            {
                string errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = "Có lỗi xảy ra: " + errorMessage });
            }
        }


    }


}