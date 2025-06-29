using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using accgame.Filters;
using PagedList;

namespace accgame.Controllers
{
    [CheckSessionID]
    [NotifyUserFilter]

    public class nguoidungController : Controller
    {

        private bool IsUserBlocked()
        {
            var idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            if (idnd == 0)
            {
                return true;
            }

            using (var db = new accgameEntities())
            {
                var nguoidung = db.NguoiDungs.Find(idnd);
                if (nguoidung != null && nguoidung != null && nguoidung.Block == true)
                {
                    return true;
                }
            }
            return false;
        }

        accgameEntities db = new accgameEntities();
        // GET: nguoidung
        public ActionResult hoso()
        {

            using (var db = new accgameEntities())
            {
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                Session["url"] = "/nguoidung/hoso";
                if (idnd == 0)
                {
                    return Redirect("/dangnhap");
                }
                var nguoidung = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).FirstOrDefault();
                string[] email = nguoidung.Email?.Split('@') ?? new string[0];
                string ten = string.Empty;
                if (email.Length > 0 && email[0].Length >= 3)
                {
                    ten = email[0].Substring(0, 3);
                }
                else
                {
                    ten = "";
                }
                string duoi = string.Empty;
                if (email.Length > 1)
                {
                    duoi = email[1];
                }
                else
                {
                    duoi = "***";
                }
                
                ViewBag.email = ten + "***@" + duoi;
                var CtvCollab = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).Select(m => m.CtvCollab).FirstOrDefault();
                ViewBag.CtvCollab = CtvCollab;
                var OnOffCtvCollab = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).Select(m => m.OnOffCtvCollab).FirstOrDefault();
                ViewBag.OnOffCtvCollab = OnOffCtvCollab;
                var CtvCollabCauHinh = db.CaiDats.Where(m => m.MaCaiDat == "is_ctv_collab").FirstOrDefault();
                ViewBag.CtvCollabCauHinh = CtvCollabCauHinh != null ? CtvCollabCauHinh.NoiDung : null;
                return View(nguoidung);
            }
            
        }
        [HttpPost]
        public ActionResult doiavt(string avatar)
        {
            using(var db = new accgameEntities())
            {
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                Session["url"] = "/nguoidung/hoso";
                if (idnd == 0)
                {
                    return Json(new { message = "Thay đổi AVATAR thất bại!" });
                }
                var nguoidung = db.NguoiDungs.Find(idnd);
                nguoidung.AnhDaiDien = avatar;
                db.SaveChanges();
                return Json(new { message = "Thay đổi AVATAR thành công!" });
            }
            
        }
        [HttpPost]
        public ActionResult doimk(string matkhaucu, string matkhau)
        {
            using (var db = new accgameEntities())
            {
                try
                {
                    int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                    Session["url"] = "/nguoidung/hoso";

                    // Kiểm tra IDNguoiDung từ session
                    if (idnd == 0)
                    {
                        return Json(new { message = "Thay đổi MẬT KHẨU thất bại! Hãy đăng nhập lại!" });
                    }

                    var nguoiDung = db.NguoiDungs.Find(idnd);

                    // Kiểm tra mật khẩu hiện tại có đúng không
                    if (nguoiDung.MatKhau != matkhaucu)
                    {
                        return Json(new { status = "error", errorCode = "wrong_old_password" });
                    }

                    // Kiểm tra mật khẩu mới có trùng với mật khẩu trong OldPass không
                    var oldPassExists = db.OldPasses.Any(op => op.IDNguoiDung == idnd && op.oldPassword == matkhau);
                    if (oldPassExists)
                    {
                        return Json(new { status = "error", errorCode = "old_password_reused" });
                    }

                    // Lưu mật khẩu cũ vào bảng OldPass
                    db.OldPasses.Add(new OldPass
                    {
                        IDNguoiDung = idnd,
                        oldPassword = nguoiDung.MatKhau
                    });

                    // Cập nhật mật khẩu mới cho người dùng
                    nguoiDung.MatKhau = matkhau;
                    nguoiDung.SessionID_Mobile = Guid.NewGuid().ToString();
                    nguoiDung.SessionID_Desktop = Guid.NewGuid().ToString();
                    // Lưu thay đổi
                    db.SaveChanges();

                    return Json(new { status = "success" });
                }
                catch (Exception ex)
                {
                    return Json(new { message = "Đã xảy ra lỗi: " + ex.Message });
                }
                
            }
        }


        /*        [HttpPost]
                public ActionResult doimk(string matkhaucu, string matkhau)
                {
                    using (var db = new accgameEntities())
                    {
                        int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                        if (idnd == 0)
                        {
                            return Json(new { message = "Thay đổi MẬT KHẨU thất bại! Hãy đăng nhập lại!" });
                        }

                        // Lấy thông tin người dùng hiện tại
                        var nguoidung = db.NguoiDungs.Find(idnd);
                        if (nguoidung == null)
                        {
                            return Json(new { message = "Người dùng không tồn tại!" });
                        }

                        // Kiểm tra mật khẩu cũ có khớp không
                        if (nguoidung.MatKhau != matkhaucu)
                        {
                            return Json(new { message = "Thất bại! Nhập sai mật khẩu cũ!" });
                        }

                        // Kiểm tra mật khẩu mới có trùng với mật khẩu hiện tại hoặc mật khẩu cũ không
                        var oldPasswords = db.OldPasses
                                             .Where(p => p.IDNguoiDung == idnd)
                                             .Select(p => p.oldPassword)
                                             .ToList();

                        if (nguoidung.MatKhau == matkhau || oldPasswords.Contains(matkhau))
                        {
                            return Json(new { message = "Không được đặt lại mật khẩu cũ!" });
                        }

                        // Lưu mật khẩu cũ vào bảng OldPass
                        OldPass oldPass = new OldPass
                        {
                            IDNguoiDung = idnd,
                            oldPassword = nguoidung.MatKhau // Lưu lại mật khẩu hiện tại
                        };
                        db.OldPasses.Add(oldPass);

                        // Cập nhật mật khẩu mới
                        nguoidung.MatKhau = matkhau;
                        nguoidung.SessionID = null; // Đặt lại session nếu cần

                        // Lưu thay đổi vào cơ sở dữ liệu
                        db.SaveChanges();

                        return Json(new { message = "Thay đổi MẬT KHẨU thành công!" });
                    }
                }
        */

        public ActionResult biendongsodu(int? page)
        {
            using (var db = new accgameEntities())
            {
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                Session["url"] = "/nguoidung/biendongsodu";
                if (idnd == 0)
                {
                    return Redirect("/dangnhap");
                }

                int pageSize = 20;
                int pageNumber = (page ?? 1);
                var biendongsd = db.BienDongSoDus.Where(m => m.IDNguoiDung == idnd).OrderByDescending(m => m.IDBienDongSoDu).ToPagedList(pageNumber, pageSize);
                var CtvCollab = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).Select(m => m.CtvCollab).FirstOrDefault();
                ViewBag.CtvCollab = CtvCollab;
                var OnOffCtvCollab = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).Select(m => m.OnOffCtvCollab).FirstOrDefault();
                ViewBag.OnOffCtvCollab = OnOffCtvCollab;
                var CtvCollabCauHinh = db.CaiDats.Where(m => m.MaCaiDat == "is_ctv_collab").FirstOrDefault();
                ViewBag.CtvCollabCauHinh = CtvCollabCauHinh != null ? CtvCollabCauHinh.NoiDung : null;
                return View(biendongsd);
            }

        }

        [HttpPost]
        public JsonResult ToggleCtvCollab(bool isChecked)
        {
            try
            {
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);

                // Phân quyền và cập nhật trạng thái
                NguoiDung nguoiDung = db.NguoiDungs.Find(idnd);
                if (nguoiDung != null)
                {
                    nguoiDung.OnOffCtvCollab = isChecked;
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Người dùng không tồn tại" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}