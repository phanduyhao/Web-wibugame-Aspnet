using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using accgame.Filters;

namespace accgame.Controllers
{
    [CheckSessionID]
    [NotifyUserFilter]

    public class NapGameController : Controller
    {
        // GET: NapGame
        public ActionResult Index()
        {
            ViewBag.napgame = "active";
            using (var db = new accgameEntities())
            {
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                Session["url"] = "/NapGame";
                if (idnd == 0)
                {
                    return Redirect("/dangnhap");
                }
                var nguoidung = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).FirstOrDefault();
                if (nguoidung != null && nguoidung.Block == true)
                {
                    return RedirectToAction("Blocked", "Home");

                }
                var napgame = db.NapGames.Include(m => m.GoiNap).Where(m => m.IDNguoiDung == idnd).OrderByDescending(m => m.IDNapGame).Take(20).ToList();
                return View(napgame);
            }
        }

        public ActionResult ListPackage(int id)
        {
            if (id == 1)
            {
                ViewBag.napgame1 = "active";
            }
            else if (id == 2)
            {
                ViewBag.napgame2 = "active";
            }
            else if (id == 3)
            {
                ViewBag.napgame3 = "active";
            }
            else if (id == 4)
            {
                ViewBag.napgame4 = "active";
            }
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            Session["url"] = "/NapGame/themmoi/" + id;
            using (var db = new accgameEntities())
            {

                var loaiGame = db.LoaiGames.Where(m => m.IDLoaiGame == id).FirstOrDefault();
                ViewBag.loaigame = loaiGame.TenLoaiGame;
                ViewBag.idLoaiGame = loaiGame.IDLoaiGame;
                ViewBag.NapLogin = db.GoiNaps.Include(m => m.LoaiGame1).Where(m => m.Xoa != true && m.LoaiGame == id && m.LoaiNap != 2 && m.An != true).OrderBy(m => m.SoThuTu).ToList();
                ViewBag.NapUID = db.GoiNaps.Include(m => m.LoaiGame1).Where(m => m.Xoa != true && m.LoaiGame == id && m.LoaiNap == 2 && m.An != true).OrderBy(m => m.SoThuTu).ToList();
                return View();
            }
        }

        public ActionResult themmoi(int id, int loaiGameId)
        {
            using (var db = new accgameEntities())
            {
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                Session["url"] = "/NapGame";
                if (idnd == 0)
                {
                    return Redirect("/dangnhap");
                }
                var nguoidung = db.NguoiDungs.Find(idnd);

                var caidat = db.CaiDats;
                var sdt = caidat.Where(m => m.MaCaiDat == "sdt").FirstOrDefault();
                ViewBag.sdt = sdt?.NoiDung ?? "";

                var loaiGame = db.LoaiGames.Where(m => m.IDLoaiGame == loaiGameId).FirstOrDefault();
                ViewBag.loaigame = loaiGame.TenLoaiGame;
                ViewBag.idLoaiGame = loaiGame.IDLoaiGame;
                // Lấy gói nạp đã được chọn từ id
                var selectedGoiNap = db.GoiNaps.Where(m => m.IDGoiNap == id && m.LoaiGame == loaiGameId).FirstOrDefault();

                if (selectedGoiNap == null)
                {
                    ViewBag.loi = "Không tìm thấy gói nạp!";
                    return RedirectToAction("Index", "NapGame"); // Redirect nếu không tìm thấy gói
                }
                ViewBag.tien = nguoidung.Tien;
                // Truyền gói nạp đã chọn vào View
                ViewBag.selectedGoiNap = selectedGoiNap;
                ViewBag.id = id;
                return View(); // Trả về view mà không cần lặp lại danh sách gói nạp
            }
        }

        [HttpPost]
        public ActionResult themmoi([Bind(Include = "loaigame, UID, TenDangNhap, MatKhau, Server, TenNhanVat, SoDienThoai, GhiChu, IDGoiNap, SoLuong")] NapGame napGame, int id)
        {
            using (var db = new accgameEntities())
            {
                var loaiGame = db.LoaiGames.Where(m => m.IDLoaiGame == id).FirstOrDefault();
                ViewBag.loaigame = loaiGame.TenLoaiGame;
                ViewBag.idLoaiGame = loaiGame.IDLoaiGame;

                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                Session["url"] = "/NapGame";
                if (idnd == 0)
                {
                    return Redirect("/dangnhap");
                }
                var nguoidung = db.NguoiDungs.Find(idnd);
                var tienTruoc = nguoidung.Tien;

                ViewBag.UID = napGame.UID;
                ViewBag.TenDangNhap = napGame.TenDangNhap;
                ViewBag.MatKhau = napGame.MatKhau;
                ViewBag.Server = napGame.Server;
                ViewBag.TenNhanVat = napGame.TenNhanVat;
                ViewBag.SoDienThoai = napGame.SoDienThoai;
                ViewBag.GhiChu = napGame.GhiChu;

                var getgoinap = db.GoiNaps.Where(m => m.IDGoiNap == napGame.IDGoiNap).FirstOrDefault();
                if (napGame.SoLuong == null || napGame.SoLuong < 1)
                {
                    napGame.SoLuong = 1;
                }

                var TongTienNap = getgoinap.Gia * napGame.SoLuong;

                if (getgoinap == null)
                {
                    ViewBag.loi = "Không tìm thấy gói nạp!";
                    return RedirectToAction("Index", "NapGame");
                }

                if (nguoidung.Tien == null || (decimal)nguoidung.Tien < (decimal)TongTienNap)
                {
                    ViewBag.loi = "Không đủ tiền để thực hiện giao dịch!";
                    // Load lại các dữ liệu cần thiết
                    var goinap = db.GoiNaps.Where(m => m.Xoa != true && m.LoaiGame == napGame.LoaiGame).OrderBy(m => m.SoThuTu).ToList();
                    ViewBag.goiNaps = goinap;
                    return View("Themmoi", napGame); // Load lại trang với thông báo lỗi
                }
                else
                {
                    nguoidung.Tien -= TongTienNap;
                    napGame.IDNguoiDung = idnd;
                    napGame.ThoiGianGui = DateTime.Now;
                    napGame.SoTien = TongTienNap;
                    db.NapGames.Add(napGame);

                    // Biến động số dư:
                    var biendongds = new BienDongSoDu
                    {
                        IDNguoiDung = nguoidung.IDNguoiDung,
                        SoTien = TongTienNap,
                        TruTien = true,
                        ThoiGian = DateTime.Now,
                        LoiNhan = "Nạp game",
                        TienTruoc = tienTruoc,
                        TienSau = nguoidung.Tien
                    };
                    db.BienDongSoDus.Add(biendongds);

                    // Biến động số dư admin:
                    var admin = db.NguoiDungs.Where(m => m.LeverAdmin == 1).FirstOrDefault();
                    if (admin != null)
                    {
                        var tienTruocAd = admin.Tien;

                        var biendongdsad = new BienDongSoDu
                        {
                            IDNguoiDung = admin.IDNguoiDung,
                            SoTien = TongTienNap,
                            TruTien = false,
                            ThoiGian = DateTime.Now,
                            LoiNhan = "Khách nạp game",
                            TienTruoc = tienTruocAd
                        };

                        var setAdmin = db.NguoiDungs.Find(admin.IDNguoiDung);
                        if (setAdmin != null)
                        {
                            setAdmin.Tien += TongTienNap;
                        }

                        biendongdsad.TienSau = setAdmin.Tien;
                        db.BienDongSoDus.Add(biendongdsad);
                    }

                    db.SaveChanges();
                    var mailNapGame = db.CaiDats
                     .Where(c => c.MaCaiDat == "mail_nap_game")
                     .Select(c => c.NoiDung)
                     .FirstOrDefault();
                    var emailService = new EmailService();
                    string toEmail = mailNapGame;
                    string subject = "Thông báo đơn nạp game";
                    string body = string.Format(
                        "Khách hàng {0} đã nạp ĐƠN #{1} với gói {2}, giá {3}.<br>Hãy mau hoàn thành đơn nạp!",
                        nguoidung.TenNguoiDung,
                        napGame.IDNapGame,
                        getgoinap.TenGoi,
                        (decimal)(getgoinap.Gia ?? 0)
                    );
                    emailService.Send(toEmail, subject, body);
                    return RedirectToAction("Index");
                }

                
            }
        }


        public ActionResult themmoiUID(int? id, int? loaiGameId)
        {
            using (var db = new accgameEntities())
            {
                var caidat = db.CaiDats;
                var sdt = caidat.Where(m => m.MaCaiDat == "sdt").FirstOrDefault();
                ViewBag.sdt = sdt?.NoiDung ?? "";

                var loaiGame = db.LoaiGames.Where(m => m.IDLoaiGame == loaiGameId).FirstOrDefault();
                ViewBag.loaigame = loaiGame.TenLoaiGame;
                ViewBag.idLoaiGame = loaiGame.IDLoaiGame;
                // Lấy gói nạp đã được chọn từ id
                var selectedGoiNap = db.GoiNaps.Where(m => m.IDGoiNap == id && m.LoaiGame == loaiGameId).FirstOrDefault();

                if (selectedGoiNap == null)
                {
                    ViewBag.loi = "Không tìm thấy gói nạp!";
                    return RedirectToAction("Index", "NapGame"); // Redirect nếu không tìm thấy gói
                }

                // Truyền gói nạp đã chọn vào View
                ViewBag.selectedGoiNap = selectedGoiNap;
                return View(); // Trả về view mà không cần lặp lại danh sách gói nạp
            }
        }


        [HttpPost]
        public ActionResult themmoiUID([Bind(Include = "loaigame, UID, Server, TenNhanVat, GhiChu, IDGoiNap, SoDienThoai, SoLuong")] NapGame napGame, int id)
        {
            using (var db = new accgameEntities())
            {
                var loaiGame = db.LoaiGames.Where(m => m.IDLoaiGame == id).FirstOrDefault();
                ViewBag.loaigame = loaiGame.TenLoaiGame;
                ViewBag.idLoaiGame = loaiGame.IDLoaiGame;

                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                Session["url"] = "/NapGame";
                if (idnd == 0)
                {
                    return Redirect("/dangnhap");
                }
                var nguoidung = db.NguoiDungs.Find(idnd);
                var tienTruoc = nguoidung.Tien;
                ViewBag.UID = napGame.UID;
                ViewBag.Server = napGame.Server;
                ViewBag.TenNhanVat = napGame.TenNhanVat;
                ViewBag.GhiChu = napGame.GhiChu;

                var goinap = db.GoiNaps.Where(m => m.Xoa != true && m.LoaiGame == napGame.LoaiGame).OrderBy(m => m.SoThuTu).ToList();
                var getgoinap = db.GoiNaps.Where(m => m.IDGoiNap == napGame.IDGoiNap).FirstOrDefault();
                if (napGame.SoLuong == null || napGame.SoLuong < 1)
                {
                    napGame.SoLuong = 1;
                }
                var TongTienNap = getgoinap.Gia * napGame.SoLuong;

                if (getgoinap == null)
                {
                    ViewBag.loi = "Không tìm thấy gói nạp!";
                    return View(goinap);
                }
                if (nguoidung.Tien == null || (decimal)nguoidung.Tien < (decimal)TongTienNap)
                {
                    TempData["loi"] = "Không đủ tiền để thực hiện giao dịch!";
                    return RedirectToAction("themmoiUID", new { id = id, loaiGameId = napGame.LoaiGame });

                }

                nguoidung.Tien -= TongTienNap;
                napGame.IDNguoiDung = idnd;
                napGame.ThoiGianGui = DateTime.Now;
                napGame.SoTien = TongTienNap;
                db.NapGames.Add(napGame);
                //Biến động số dư:
                var biendongds = new BienDongSoDu();
                biendongds.IDNguoiDung = nguoidung.IDNguoiDung;
                biendongds.SoTien = TongTienNap;
                biendongds.TruTien = true;
                biendongds.ThoiGian = DateTime.Now;
                biendongds.LoiNhan = "Nạp game";
                biendongds.TienTruoc = tienTruoc;
                biendongds.TienSau = nguoidung.Tien;
                db.BienDongSoDus.Add(biendongds);
                //
                //Biến động số dư admin:
                var admin = db.NguoiDungs.Where(m => m.LeverAdmin == 1).FirstOrDefault();
                if (admin != null)
                {
                    var tienTruocAd = admin.Tien;

                    var biendongdsad = new BienDongSoDu();
                    biendongdsad.IDNguoiDung = admin.IDNguoiDung;
                    biendongdsad.SoTien = TongTienNap;
                    biendongdsad.TruTien = false;
                    biendongdsad.ThoiGian = DateTime.Now;
                    biendongdsad.LoiNhan = "Khách nạp game";
                    biendongdsad.TienTruoc = tienTruocAd;

                    var setAdmin = db.NguoiDungs.Find(admin.IDNguoiDung);
                    if (setAdmin != null)
                    {
                        setAdmin.Tien += TongTienNap;
                    }

                    biendongdsad.TienSau = setAdmin.Tien;
                    db.BienDongSoDus.Add(biendongdsad);
                }

                //

                db.SaveChanges();
                var mailNapGame = db.CaiDats
                      .Where(c => c.MaCaiDat == "mail_nap_game")
                      .Select(c => c.NoiDung)
                      .FirstOrDefault();
                var emailService = new EmailService();
                string toEmail = mailNapGame;
                string subject = "Thông báo đơn nạp game";
                string body = string.Format(
                    "Khách hàng {0} đã nạp ĐƠN #{1} với gói {2}, giá {3}.<br>Hãy mau hoàn thành đơn nạp!",
                    nguoidung.TenNguoiDung,
                    napGame.IDNapGame,
                    getgoinap.TenGoi,
                    (decimal)(TongTienNap ?? 0)
                );
                emailService.Send(toEmail, subject, body);
                return RedirectToAction("Index");
            }

        }

   /*     private void SendEmail(string tenDangNhap, int idDon, decimal tongTien)
        {
            try
            {
                var fromAddress = new System.Net.Mail.MailAddress("shopaccwibugamevn@gmail.com", "Tên hệ thống");
                var toAddress = new System.Net.Mail.MailAddress("haomrvuii@gmail.com", "Admin");
                const string fromPassword = "hqxthgtyceaglxgw";
                string subject = "Thông báo đơn nạp game";
                string body = $"Khách hàng {tenDangNhap} đã nạp ĐƠN {idDon}, giá {tongTien}.\nHãy mau hoàn thành đơn nạp!";

                var smtp = new System.Net.Mail.SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new System.Net.Mail.MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                // Log lỗi (nếu cần)
                Console.WriteLine($"Lỗi khi gửi email: {ex.Message}");
            }
        }
*/
    }
}