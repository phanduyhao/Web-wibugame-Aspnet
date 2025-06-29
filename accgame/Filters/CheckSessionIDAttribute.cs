using System;
using System.Web;
using System.Web.Mvc;
using accgame.Context;

public class CheckSessionIDAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        HttpCookie authCookie = filterContext.HttpContext.Request.Cookies["AuthCookie"];
        if (authCookie != null)
        {
            string cookieValue = authCookie.Value;
            var cookieData = cookieValue.Split('&');
            var userID = cookieData[0].Split('=')[1];
            var sessionID = cookieData[1].Split('=')[1];

            using (var db = new accgameEntities())
            {
                int idnd = Convert.ToInt32(userID);
                var nguoiDung = db.NguoiDungs.Find(idnd);
                if (nguoiDung != null)
                {
                    // Kiểm tra nếu SessionID không khớp
                    string userAgent = filterContext.HttpContext.Request.UserAgent.ToLower();
                    bool isMobile = userAgent.Contains("android") || userAgent.Contains("iphone");

                    // Kiểm tra SessionID theo nền tảng
                    if ((isMobile && nguoiDung.SessionID_Mobile != sessionID) || (!isMobile && nguoiDung.SessionID_Desktop != sessionID))
                    {
                        // Nếu SessionID không khớp thì đăng xuất người dùng
                        filterContext.Result = new RedirectResult("/dangxuat");
                        return;
                    }
                }
            }
        }

        base.OnActionExecuting(filterContext);
    }
}
