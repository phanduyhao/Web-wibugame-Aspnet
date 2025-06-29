using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using accgame.Models;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Net.Http;
using System.Data.Entity;

namespace accgame.Controllers
{
    public class HomeController : Controller
    {
        [NotifyUserFilter]

        public ActionResult Index()
        {
            using (var db = new accgameEntities())
            {
                ViewBag.trangchu = "active";

                var caidat = db.CaiDats;

                var tieuDe = caidat.Where(m => m.MaCaiDat == "tieude_trangchu").FirstOrDefault();
                var noiDung = caidat.Where(m => m.MaCaiDat == "noidung_trangchu").FirstOrDefault();
                var tieuDeTop = caidat.Where(m => m.MaCaiDat == "tieude_top_trangchu").FirstOrDefault();
                var noiDungTop = caidat.Where(m => m.MaCaiDat == "noidung_top_trangchu").FirstOrDefault();

                var danhmuc = db.DanhMucs.Where(m => m.TaiKhoanNoiBo != true && m.Xoa != true && m.Hide != true);

                ViewBag.TieuDeTrangChu = tieuDe?.NoiDung ?? "";
                ViewBag.NoiDungTrangChu = noiDung?.NoiDung ?? "";
                ViewBag.TieuDeTopTrangChu = tieuDeTop?.NoiDung ?? "";
                ViewBag.NoiDungTopTrangChu = noiDungTop?.NoiDung ?? "";

                var homeTitle = caidat.Where(m => m.MaCaiDat == "home_title").FirstOrDefault();
                ViewBag.Title = homeTitle?.NoiDung ?? "";
                var bannerHome = caidat.Where(m => m.MaCaiDat == "banner_home").Select(m => m.NoiDung).FirstOrDefault();
                ViewBag.bannerHome = bannerHome;

                var napGame = caidat.Where(m => m.MaCaiDat == "nap_game").FirstOrDefault();
                var cayThue = caidat.Where(m => m.MaCaiDat == "cay_thue").FirstOrDefault();
                ViewBag.napGame = napGame?.NoiDung ?? "";
                ViewBag.cayThue = cayThue?.NoiDung ?? "";

                var listLoaiGame = db.LoaiGames.Where(m => m.Hide != true).OrderBy(m => m.STT).ToList();
                var listAcc = new List<Acc>(); // Khởi tạo listAcc là một danh sách mới
                Dictionary<int, int> dangBanDictionary = new Dictionary<int, int>();
                Dictionary<int, int> daBanDictionary = new Dictionary<int, int>();
                foreach (var item in listLoaiGame)
                {
                    listAcc.AddRange(db.Accs.Where(m => m.LoaiGame == item.IDLoaiGame && m.DaBan != true && m.Xoa != true && m.An != true)
                                             .OrderByDescending(m => m.IDAcc)
                                             .Take(4)
                                             .ToList());
                    var dangBan = db.LoaiAccs.Where(m => m.IDLoaiGame == item.IDLoaiGame && m.Hide != true).Sum(m => m.DangBan) ?? 0;
                    var daBan = db.LoaiAccs.Where(m => m.IDLoaiGame == item.IDLoaiGame && m.Hide != true).Sum(m => m.DaBan) ?? 0;

                    // Lưu số lượng Đang Bán và Đã Bán vào dictionary
                    dangBanDictionary[item.IDLoaiGame] = dangBan;
                    daBanDictionary[item.IDLoaiGame] = daBan;
                }
                ViewBag.DangBanDictionary = dangBanDictionary;
                ViewBag.DaBanDictionary = daBanDictionary;
                ViewBag.showDichVu = listLoaiGame.Where(m => m.DichVuNapGame == true || m.DichVuCayThue == true || m.DichVuCode == true).ToList().Count() > 0 ? true : false;
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);

                HomeModels data = new HomeModels()
                {
                    listNguoiDung = db.NguoiDungs.OrderByDescending(m => m.TienNapThang).ThenByDescending(m => m.IDNguoiDung).Take(10).ToList(),
                    listLoaiGame = listLoaiGame,
                    listAcc = listAcc,
                    listLoaiAcc = db.LoaiAccs.Where(m => m.Hide != true && m.IDLoaiAccCha == null).ToList(),
                };
                var CtvCollab = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).Select(m => m.CtvCollab).FirstOrDefault();
                ViewBag.CtvCollab = CtvCollab;
                var OnOffCtvCollab = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).Select(m => m.OnOffCtvCollab).FirstOrDefault();
                ViewBag.OnOffCtvCollab = OnOffCtvCollab;
                var CtvCollabCauHinh = db.CaiDats.Where(m => m.MaCaiDat == "is_ctv_collab").FirstOrDefault();
                ViewBag.CtvCollabCauHinh = CtvCollabCauHinh != null ? CtvCollabCauHinh.NoiDung : null;
                ViewBag.isSaleTet = caidat.Where(m => m.MaCaiDat == "is_sale_tet").Select(m => m.NoiDung).FirstOrDefault();
                ViewBag.phanTramTet = int.TryParse(caidat.Where(m => m.MaCaiDat == "phan_tram_sale_tet")
                                                        .Select(m => m.NoiDung)
                                                        .FirstOrDefault(), out var phanTramTet)
                                      ? phanTramTet
                                      : 0;
                var recentNotifications = db.NotifyUsers
                    .Where(n => n.Contents.Contains("mua")) // Lọc các thông báo có chữ "mua"
                    .OrderByDescending(n => n.Thoigian)     // Sắp xếp theo thời gian giảm dần (mới nhất trước)
                    .Take(5)                                // Lấy 5 thông báo gần nhất
                    .ToList();
                List<string> notificationMessages = new List<string>();
                
                foreach (var notify in recentNotifications)
                {
                    var user = db.NguoiDungs.Find(notify.IdNguoidung); // Lấy thông tin người dùng từ ID
                    string userName = user != null ? user.TenNguoiDung : "Khách hàng"; // Nếu không có tên, dùng "Khách hàng"

                    // Tính thời gian cách đây bao lâu
                    TimeSpan timeDiff = DateTime.Now - (notify.Thoigian ?? DateTime.MinValue);
                    string timeAgo = "";
                    if (timeDiff.TotalMinutes < 60)
                        timeAgo = $"{(int)timeDiff.TotalMinutes} phút trước";
                    else if (timeDiff.TotalHours < 24)
                        timeAgo = $"{(int)timeDiff.TotalHours} giờ trước";
                    else
                        timeAgo = $"{(int)timeDiff.TotalDays} ngày trước";
                    Match match = Regex.Match(notify.Contents, @"<strong>(.*?)</strong>");

                    string maAcc = match.Groups[1].Value;  // Lấy phần GI2250 từ strong tag
                    string message = "";
                    var acc = db.Accs.FirstOrDefault(a => a.MaTaiKhoan == maAcc);
                    var taiKhoan = db.AccRandoms.FirstOrDefault(t => t.TaiKhoan == maAcc);
                    if (userName.Length < 6)
                        userName = userName.Substring(0, Math.Min(userName.Length, 10)) + "**";
                    else
                        userName = userName.Substring(0, Math.Min(userName.Length, 10)) + "****";

                    if (acc != null)
                    {
                        message = $"Khách hàng {userName} vừa mua Acc Vip với giá {acc.GiaDaMua:N0} VNĐ - {timeAgo}";
                    }
                    else if (taiKhoan != null)
                    {
                        message = $"Khách hàng {userName} vừa mua Acc Reroll/Starter với giá {taiKhoan.GiaDaMua:N0} VNĐ - {timeAgo}";
                    }
                    notificationMessages.Add(message);
                }

                // Đưa vào ViewBag để hiển thị trên trang Home
                ViewBag.RecentNotifications = notificationMessages;

                return View(data);
            }

        }

        public ActionResult Blocked()
        {
            // Lấy IDNguoiDung từ session
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);

            using (var db = new accgameEntities())
            {
                // Truy vấn cơ sở dữ liệu để lấy thông tin người dùng bị khóa
                var nguoidung = db.NguoiDungs.Find(idnd);

                if (nguoidung != null && nguoidung.Block == true)
                {
                    // Truyền nội dung khóa tài khoản ra View
                    ViewBag.BlockContent = nguoidung.BlockContent;
                }
            }
            return View();
        }
        



        public ActionResult renderTenMien()
        {
            using (var db = new accgameEntities())
            {
                var caidat = db.CaiDats;
                var tenmien = caidat.Where(m => m.MaCaiDat == "ten_mien").FirstOrDefault();
                var getTenmien = tenmien?.NoiDung ?? "";
                return Content(getTenmien);
            }

        }

        public ActionResult renderLogo()
        {
            using (var db = new accgameEntities())
            {
                var caidat = db.CaiDats;
                var logo = caidat.Where(m => m.MaCaiDat == "logo").FirstOrDefault();
                var getLogo = logo?.NoiDung ?? "";
                return Content(getLogo);
            }

        }

        public ActionResult render()
        {
            using (var db = new accgameEntities())
            {
                HttpCookie cookie = Request.Cookies["UserLogin"];
                if (cookie != null && cookie["UserName"] != null && cookie["Password"] != null && Convert.ToInt32(Session["IDNguoiDung"]) == 0 )
                {
                    string username = cookie["UserName"].ToString();
                    string password = cookie["Password"].ToString();
                    var user = db.NguoiDungs.Where(m => m.TenNguoiDung == username && m.MatKhau == password).FirstOrDefault();
                    if (user != null)
                    {
                        Session["IDNguoiDung"] = user.IDNguoiDung;
                        Session["levelAdmin"] = user.LeverAdmin;
                    }
                }
            }
            return PartialView("_renderTop");
        }

        public ActionResult renderGame()
        {
            using (var db = new accgameEntities())
            {
                var listGame = db.LoaiGames.Where(m => m.Hide != true).OrderBy(m => m.STT).ToList();
                return View(listGame);
            }
        }

        public ActionResult renderMoney()
        {
            var idNguoiDung = Convert.ToInt32(Session["IDNguoiDung"]);
            using (var db = new accgameEntities())
            {
                ViewBag.tienNguoiDung = "";
                var user = db.NguoiDungs.Where(m => m.IDNguoiDung == idNguoiDung).FirstOrDefault();
                
                return View(user);
            }
            
        }

        public ActionResult renderFooter()
        {
            using (var db = new accgameEntities())
            {
                var caidat = db.CaiDats;
                var listGame = db.LoaiGames.ToList();

                var sdt = caidat.Where(m => m.MaCaiDat == "sdt").FirstOrDefault();
                var tenmien = caidat.Where(m => m.MaCaiDat == "ten_mien").FirstOrDefault();
                var facebook = caidat.Where(m => m.MaCaiDat == "facebook").FirstOrDefault();

                var thuAcc = caidat.Where(m => m.MaCaiDat == "thu_acc").FirstOrDefault();
                var checkUt = caidat.Where(m => m.MaCaiDat == "check_ut").FirstOrDefault();
                var hoTro = caidat.Where(m => m.MaCaiDat == "ho_tro").FirstOrDefault();



                ViewBag.sdt = sdt?.NoiDung ?? "";
                ViewBag.tenmien = tenmien?.NoiDung ?? "";
                ViewBag.facebook = facebook?.NoiDung ?? "";

                ViewBag.thuAcc = thuAcc?.NoiDung ?? "";
                ViewBag.checkUt = checkUt?.NoiDung ?? "";
                ViewBag.hoTro = hoTro?.NoiDung ?? "";

                var footerDetail = caidat.Where(m => m.MaCaiDat == "footer_detail").FirstOrDefault();
                ViewBag.footerDetail = footerDetail?.NoiDung ?? "";
                return View(listGame);
            }
            
        }
    }
}