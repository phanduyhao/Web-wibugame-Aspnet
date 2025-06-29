using System;
using System.Linq;
using System.Web.Mvc;
using accgame.Context;

namespace accgame.Controllers
{
    public class NotifyController : Controller
    {
        [HttpPost]
        public ActionResult MarkAllNotificationsAsRead()
        {
            using (var db = new accgameEntities())
            {
                int userId = Convert.ToInt32(Session["IDNguoiDung"]);
                if (userId == 0)
                {
                    return Json(new { success = false, message = "User not logged in." });
                }

                // Cập nhật IsRead cho tất cả thông báo cá nhân của người dùng
                var notifications = db.NotifyUsers.Where(n => n.IdNguoidung == userId && n.IsRead != true).ToList();
                foreach (var notify in notifications)
                {
                    notify.IsRead = true;
                }

                // Xử lý thông báo admin
                var adminNotifications = db.NotifyUsers
                    .Where(n => n.IsAdminPost == true)
                    .ToList();

                foreach (var adminNotify in adminNotifications)
                {
                    // Kiểm tra xem đã có bản ghi trong AdminNotifyReads chưa
                    bool alreadyExists = db.AdminNotifyReads
                        .Any(r => r.IdNotifyUser == adminNotify.IdNotify && r.IdNguoidung == userId);

                    // Nếu chưa có thì thêm mới
                    if (!alreadyExists)
                    {
                        db.AdminNotifyReads.Add(new AdminNotifyRead
                        {
                            IdNotifyUser = adminNotify.IdNotify,
                            IdNguoidung = userId
                        });
                    }
                }

                // Lưu thay đổi
                db.SaveChanges();

                return Json(new { success = true });
            }
        }
    }
}
