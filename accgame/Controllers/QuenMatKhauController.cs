using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using accgame.Context;

namespace accgame.Controllers
{
    [NotifyUserFilter]

    public class QuenMatKhauController : Controller
    {
        // GET: QuenMatKhau
        public ActionResult Index()
        {
            return View();
        }
        /*[HttpPost]
        public ActionResult Index(string Email)
        {
            ViewBag.Email = Email;
            using (var db = new accgameEntities())
            {
                var nguoidung = db.NguoiDungs.Where(x => x.Email == Email).FirstOrDefault();
                if (nguoidung == null)
                {
                    ViewBag.loi = "Không tìm thấy người dùng có email " + Email;
                    return View();
                }

                var caidat = db.CaiDats;
                var tenmien = caidat.Where(m => m.MaCaiDat == "ten_mien").FirstOrDefault();
                var getTenmien = tenmien?.NoiDung ?? "";

                string toEmail = Convert.ToString(Email);
                string subject = "Quên mật khẩu tài khoản! " + getTenmien;
                string body = "Bạn vừa gửi lệnh quên mật khẩu tài khoản tại " + getTenmien + "Tài khoản của bạn là: " + nguoidung.TenNguoiDung +
                    ", Mật khẩu của bạn là: <b>" + nguoidung.MatKhau + "</b>. Lưu ý, để đảm bảo tính bảo mật, không nên cung cấp mật khẩu cho bên thứ 3!";
                var service = new EmailService();
                bool kq = service.Send(toEmail, subject, body);
                ViewBag.loi = "Quên mật khẩu thành công, bạn hãy kiểm tra email: " + Email + " để xem mật khẩu nhé!";
                return View();
            }
        }*/
        [HttpPost]
        public ActionResult Index(string Email)
        {
            ViewBag.Email = Email;
            using (var db = new accgameEntities())
            {
                var nguoidung = db.NguoiDungs.Where(x => x.Email == Email).FirstOrDefault();

                if (nguoidung == null)
                {
                    ViewBag.loi = "Không tìm thấy người dùng với email " + Email;
                    return View();
                }

                // Kiểm tra nếu tài khoản đang bị chặn
                if (nguoidung.LockedUntil.HasValue && nguoidung.LockedUntil > DateTime.Now)
                {
                    var timeRemaining = nguoidung.LockedUntil.Value - DateTime.Now;
                    ViewBag.loi = $"Chức năng quên mật khẩu bị khóa. Vui lòng thử lại sau {timeRemaining.Minutes} phút.";
                    return View();
                }

                // Kiểm tra số lần yêu cầu trong 5 phút
                if (nguoidung.LastPasswordRequestTime.HasValue && nguoidung.LastPasswordRequestTime.Value.AddMinutes(5) > DateTime.Now)
                {
                    if (nguoidung.FailedPasswordRequests >= 3)
                    {
                        // Nếu đã đạt giới hạn 5 lần, khóa chức năng quên mật khẩu trong 2 giờ
                        nguoidung.LockedUntil = DateTime.Now.AddHours(2);
                        db.SaveChanges();
                        ViewBag.loi = "Bạn đã thử quá nhiều lần. Chức năng quên mật khẩu đã bị khóa. Vui lòng thử lại sau 2 giờ.";
                        return View();
                    }
                    else
                    {
                        // Tăng số lần yêu cầu quên mật khẩu
                        nguoidung.FailedPasswordRequests++;
                        db.SaveChanges();
                    }
                }
                else
                {
                    // Nếu quá 5 phút thì reset lại số lần yêu cầu
                    nguoidung.FailedPasswordRequests = 1;
                    nguoidung.LastPasswordRequestTime = DateTime.Now;
                    db.SaveChanges();
                }

                // Thực hiện gửi email quên mật khẩu (giống như hiện tại)
                var caidat = db.CaiDats;

                var tenmien = caidat.Where(m => m.MaCaiDat == "ten_mien").FirstOrDefault();
                var getTenmien = tenmien?.NoiDung ?? "";

                string toEmail = Convert.ToString(Email);
                string subject = "Quên mật khẩu tài khoản! " + getTenmien;
                string body = "Bạn vừa gửi lệnh quên mật khẩu tài khoản tại " + getTenmien + "Tài khoản của bạn là: " + nguoidung.TenNguoiDung +
                    ", Mật khẩu của bạn là: <b>" + nguoidung.MatKhau + "</b>. Lưu ý, để đảm bảo tính bảo mật, không nên cung cấp mật khẩu cho bên thứ 3!";
                var service = new EmailService();
                bool kq = service.Send(toEmail, subject, body);
                ViewBag.loi = "Quên mật khẩu thành công. Kiểm tra email để nhận mật khẩu!";
                return View();
            }
        }

    }
}