using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using accgame.Context;

namespace accgame.Controllers
{
    public class AccountController : Controller
    {
        private readonly GoogleApiService _googleApiService = new GoogleApiService();

        public ActionResult Login()
        {
            var authUrl = _googleApiService.GetAuthorizationUrl();
            return Redirect(authUrl);
        }

        public async Task<ActionResult> OAuth2Callback(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                // Trao đổi mã code lấy access token
                var accessToken = await _googleApiService.ExchangeCodeForTokenAsync(code);

                // Lấy thông tin người dùng
                var userInfo = await _googleApiService.GetUserInfoAsync(accessToken);

                // Xử lý thông tin người dùng
                using (var db = new accgameEntities())
                {
                    var nguoiDung = db.NguoiDungs.Where(m => m.TenNguoiDung == userInfo.Email).FirstOrDefault();
                    if (nguoiDung != null)
                    {
                        Session["IDNguoiDung"] = nguoiDung.IDNguoiDung;
                        Session["levelAdmin"] = Convert.ToInt16(nguoiDung.LeverAdmin);
                        HttpCookie cookieV = new HttpCookie("UserLogin");
                        cookieV["UserName"] = nguoiDung.TenNguoiDung;
                        cookieV["Password"] = nguoiDung.MatKhau;
                        cookieV.Expires = DateTime.Now.AddDays(30);
                        Response.Cookies.Add(cookieV);
                        string oldUrlV = Session["url"].ToString();
                        return Redirect(oldUrlV);
                    }
                    else
                    {
                        var newNguoiDUng = new NguoiDung();
                        newNguoiDUng.TenNguoiDung = userInfo.Email;
                        newNguoiDUng.MatKhau = Mahoa.MyString.md5(DateTime.Now.ToString("ddMMyyyyhhmmssff"));
                        newNguoiDUng.Email = userInfo.Email;
                        newNguoiDUng.Tien = 0;
                        newNguoiDUng.NgayThamGia = DateTime.Now;
                        newNguoiDUng.LeverAdmin = 0;
                        newNguoiDUng.TienNapThang = 0;
                        newNguoiDUng.TienNap = 0;
                        newNguoiDUng.GoogleAccount = true;

                        db.NguoiDungs.Add(newNguoiDUng);
                        db.SaveChanges();
                        Session["IDNguoiDung"] = newNguoiDUng.IDNguoiDung;
                        Session["levelAdmin"] = Convert.ToInt16(newNguoiDUng.LeverAdmin);
                        Session["SessionID"] = Session.SessionID;
                        newNguoiDUng.SessionID = Session.SessionID;
                        HttpCookie cookieV = new HttpCookie("UserLogin");
                        cookieV["UserName"] = newNguoiDUng.TenNguoiDung;
                        cookieV["Password"] = newNguoiDUng.MatKhau;
                        cookieV.Expires = DateTime.Now.AddDays(30);

                        HttpCookie authCookie = new HttpCookie("AuthCookie");
                        authCookie["UserID"] = newNguoiDUng.IDNguoiDung.ToString();
                        authCookie["SessionID"] = newNguoiDUng.SessionID;
                        authCookie.Expires = DateTime.Now.AddDays(30); // Lưu cookie trong 30 ngày
                        Response.Cookies.Add(authCookie);
                        Response.Cookies.Add(cookieV);
                        string oldUrlV = Session["url"].ToString();
                        return Redirect(oldUrlV);
                    }
                }

               
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }
    }
}
