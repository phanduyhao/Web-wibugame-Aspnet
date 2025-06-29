using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using Newtonsoft.Json;

namespace accgame.Controllers
{
    public class dangnhapController : Controller
    {
        // GET: dangnhap
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string TenNguoiDung, string MatKhau)
        {
            using (var db = new accgameEntities())
            {
                TenNguoiDung = TenNguoiDung.ToLower();
                ViewBag.TenNguoiDung = TenNguoiDung;
                ViewBag.MatKhau = MatKhau;

                var nguoidung = db.NguoiDungs
                    .Where(m => m.TenNguoiDung == TenNguoiDung)
                    .FirstOrDefault();

                if (nguoidung == null)
                {
                    // Kiểm tra API xem tài khoản có tồn tại không
                    var apiUrl = HttpContext.Application["url_api"]?.ToString();
                    string content = "";
                    string url = $"{apiUrl}api/check-login.php?username={TenNguoiDung}&password={MatKhau}";

                    try
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                        request.AutomaticDecompression = DecompressionMethods.GZip;

                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        using (Stream responeStream = response.GetResponseStream())
                        using (StreamReader streamReader = new StreamReader(responeStream))
                        {
                            content = streamReader.ReadToEnd();
                        }

                        User dataJson = JsonConvert.DeserializeObject<User>(content);
                        if (dataJson != null && dataJson.status == "success")
                        {
                            // Lưu thông tin người dùng từ API vào cơ sở dữ liệu
                            nguoidung = new NguoiDung
                            {
                                TenNguoiDung = TenNguoiDung,
                                MatKhau = MatKhau,
                                Tien = Convert.ToInt64(dataJson.data.cash),
                                Email = dataJson.data.email,
                                LeverAdmin = dataJson.data.type == "2" ? 1 : (dataJson.data.type == "4" ? 2 : 0),
                                NgayThamGia = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt32(dataJson.data.created_at)).DateTime
                            };

                            db.NguoiDungs.Add(nguoidung);
                            db.SaveChanges();

                            Session["IDNguoiDung"] = nguoidung.IDNguoiDung;
                            Session["levelAdmin"] = nguoidung.LeverAdmin;
                            SetUserCookie(nguoidung);

                            string oldUrl = Session["url"]?.ToString() ?? "/"; // Default to home page if URL is null
                            return Redirect(oldUrl);
                        }
                        else
                        {
                            ViewBag.loi = "Tài khoản hoặc mật khẩu không đúng!";
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.loi = "Tài khoản hoặc mật khẩu không đúng!";
                    }
                }
                else
                {
                    // Check if the account is locked due to too many failed login attempts
                    if (nguoidung.LockoutEndTime.HasValue && nguoidung.LockoutEndTime.Value > DateTime.Now)
                    {
                        ViewBag.loi = "Tài khoản của bạn đã bị khóa. Vui lòng thử lại sau " +
                                      (nguoidung.LockoutEndTime.Value - DateTime.Now).Minutes + " phút.";
                        return View();
                    }

                    // Verify the password
                    if (nguoidung.MatKhau != MatKhau)
                    {
                        nguoidung.FailedLoginAttempts += 1;

                        // If the user has failed login attempts more than 5 times, lock the account for 30 minutes
                        if (nguoidung.FailedLoginAttempts >= 5)
                        {
                            nguoidung.LockoutEndTime = DateTime.Now.AddMinutes(30);
                            ViewBag.loi = "Bạn đã nhập sai mật khẩu quá nhiều lần. Tài khoản của bạn bị khóa trong 30 phút.";
                        }
                        else
                        {
                            ViewBag.loi = $"Tài khoản hoặc mật khẩu không đúng! Bạn còn {5 - nguoidung.FailedLoginAttempts} lần thử.";
                        }

                        db.SaveChanges();
                        return View();
                    }

                    // Kiểm tra nếu tài khoản bị khóa
                    if (nguoidung != null && nguoidung.Block == true)
                    {
                        // Lưu IDNguoiDung vào session
                        Session["IDNguoiDung"] = nguoidung.IDNguoiDung;
                        nguoidung.FailedLoginAttempts = 0;
                        nguoidung.LockoutEndTime = null;
                        Session["SessionID"] = Session.SessionID;
                        nguoidung.SessionID = Session.SessionID;
                        TempData["BlockContent"] = nguoidung.BlockContent ?? "Tài khoản của bạn đã bị khóa.";
                        return RedirectToAction("Blocked", "Home");
                    }
                    // If login is successful, reset the failed login attempts and lockout time
                    nguoidung.FailedLoginAttempts = 0;
                    nguoidung.LockoutEndTime = null;
                    Session["IDNguoiDung"] = nguoidung.IDNguoiDung;
                    Session["SessionID"] = Session.SessionID;
                    /*                    nguoidung.SessionID = Session.SessionID;
                    */
                    string userAgent = Request.UserAgent.ToLower();
                    bool isMobile = userAgent.Contains("android") || userAgent.Contains("iphone");

                    // Tạo một SessionID mới cho phiên đăng nhập
                    string newSessionID = Guid.NewGuid().ToString();

                    // Kiểm tra nền tảng và cập nhật SessionID cho đúng nền tảng
                    if (isMobile)
                    {
                        // Nếu có một SessionID khác trên điện thoại, đăng xuất phiên cũ
                        if (nguoidung.SessionID_Mobile != null && nguoidung.SessionID_Mobile != newSessionID)
                        {
                            nguoidung.SessionID_Desktop = null; // Đăng xuất cả hai phiên
                        }
                        nguoidung.SessionID_Mobile = newSessionID;
                    }
                    else
                    {
                        // Nếu có một SessionID khác trên máy tính, đăng xuất phiên cũ
                        if (nguoidung.SessionID_Desktop != null && nguoidung.SessionID_Desktop != newSessionID)
                        {
                            nguoidung.SessionID_Mobile = null; // Đăng xuất cả hai phiên
                        }
                        nguoidung.SessionID_Desktop = newSessionID;
                    }
                    nguoidung.SessionID = newSessionID;
                    db.SaveChanges();
                    Session["levelAdmin"] = nguoidung.LeverAdmin;
                    SetUserCookie(nguoidung);

                    string oldUrl = Session["url"]?.ToString() ?? "/";
                    return Redirect(oldUrl);
                }

                return View();
            }
        }

        private void SetUserCookie(NguoiDung nguoidung)
        {
            HttpCookie cookie = new HttpCookie("UserLogin");
            cookie["UserName"] = nguoidung.TenNguoiDung;
            cookie["Password"] = nguoidung.MatKhau;
            cookie.Expires = DateTime.Now.AddDays(30);
            HttpCookie authCookie = new HttpCookie("AuthCookie");
            authCookie["UserID"] = nguoidung.IDNguoiDung.ToString();
            authCookie["SessionID"] = nguoidung.SessionID;
            authCookie.Expires = DateTime.Now.AddDays(30); // Lưu cookie trong 30 ngày
            Response.Cookies.Add(authCookie);
        
            Response.Cookies.Add(cookie);
        }
    }

    public class User
    {
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("user")]
        public Data data { get; set; }
    }

    public class Data
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("username")]
        public string username { get; set; }
        [JsonProperty("password")]
        public string password { get; set; }
        [JsonProperty("email")]
        public string email { get; set; }
        [JsonProperty("cash")]
        public string cash { get; set; }
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("created_at")]
        public string created_at { get; set; }
    }
}
