using System;
using System.Web;
using System.Web.Mvc;
using accgame.Context;

namespace accgame.Filters
{
    public class CheckAccountBlockAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpCookie authCookie = filterContext.HttpContext.Request.Cookies["AuthCookie"];

            if (authCookie != null)
            {
                int userId = Convert.ToInt32(authCookie["UserID"]);

                using (var db = new accgameEntities())
                {
                    var nguoidung = db.NguoiDungs.Find(userId);

                    if (nguoidung != null)
                    {
                        // Kiểm tra tài khoản có bị khóa không
                        if (nguoidung != null && nguoidung.Block == true)
                        {
                            filterContext.Controller.TempData["BlockContent"] = nguoidung.BlockContent;
                            filterContext.Result = new RedirectResult("/Home/Blocked");
                            return;
                        }
                    }
                }
            }
            else
            {
                // Nếu không có cookie, buộc người dùng đăng nhập lại
                filterContext.Result = new RedirectResult("/dangnhap");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
