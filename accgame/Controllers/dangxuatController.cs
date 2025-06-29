using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace accgame.Controllers
{
    public class dangxuatController : Controller
    {
        // GET: dangxuat
        /* public ActionResult Index()
         {
             HttpCookie cookie = Request.Cookies["UserLogin"];
             if (cookie != null)
             {
                 cookie["UserName"] = null;
                 cookie["Password"] = null;
                 Response.SetCookie(cookie);
             }

             Session["IDNguoiDung"] = 0;
             Session["levelAdmin"] = 0;
             Session.Clear();

             if (Request.Cookies["AuthCookie"] != null)
             {
                 HttpCookie authCookie = new HttpCookie("AuthCookie");
                 authCookie.Expires = DateTime.Now.AddDays(-1);
                 Response.Cookies.Add(authCookie);
             }

             return RedirectToAction("Index", "dangnhap");

         }*/
        public ActionResult Index()
        {
            // Xóa cookie UserLogin nếu nó tồn tại
            HttpCookie cookie = Request.Cookies["UserLogin"];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1); // Đặt thời gian hết hạn của cookie về quá khứ
                Response.Cookies.Add(cookie); // Cập nhật cookie đã sửa đổi vào Response
            }

            // Xóa session người dùng
            Session["IDNguoiDung"] = 0;
            Session["levelAdmin"] = 0;
            Session.Clear();

            // Xóa AuthCookie nếu tồn tại
            if (Request.Cookies["AuthCookie"] != null)
            {
                HttpCookie authCookie = new HttpCookie("AuthCookie");
                authCookie.Expires = DateTime.Now.AddDays(-1); // Đặt thời gian hết hạn về quá khứ
                Response.Cookies.Add(authCookie); // Cập nhật cookie đã sửa đổi vào Response
            }

            // Chuyển hướng người dùng về trang đăng nhập
            return RedirectToAction("Index", "dangnhap");
        }
    }
}