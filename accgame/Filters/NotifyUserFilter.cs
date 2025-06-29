using System;
using System.Linq;
using System.Web.Mvc;
using accgame.Context;

public class NotifyUserFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        using (var db = new accgameEntities())
        {
            int idnd = Convert.ToInt32(filterContext.HttpContext.Session["IDNguoiDung"] ?? "0");
            if (idnd > 0)
            {
                // Lấy danh sách thông báo
                var notifyList = db.NotifyUsers
                                   .Where(m => m.IdNguoidung == idnd || m.IsAdminPost == true)
                                   .OrderByDescending(m => m.IdNotify)
                                   .ToList();

                // Đếm số lượng thông báo chưa đọc
                int unreadCount = db.NotifyUsers
                     .Where(m =>
                         // Thông báo cá nhân chưa đọc
                         (m.IdNguoidung == idnd && m.IsRead != true) ||

                         // Thông báo từ Admin chưa được đánh dấu là đã đọc
                         (m.IsAdminPost == true &&
                          !db.AdminNotifyReads.Any(r => r.IdNotifyUser == m.IdNotify && r.IdNguoidung == idnd))
                     )
                     .Count();

                // Lưu vào ViewBag
                filterContext.Controller.ViewBag.NotifyUser = notifyList;
                filterContext.Controller.ViewBag.UnreadNotifyCount = unreadCount;
            }
        }

        base.OnActionExecuting(filterContext);
    }
}
