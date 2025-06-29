using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using accgame.Mahoa;
using ImageMagick;
using accgame.Filters;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]

    public class CauHinhController : Controller
    {
        // GET: quantri/CauHinh
        public ActionResult Index()
        {
            ViewBag.cauhinh = "active";
            using (var db = new accgameEntities())
            {
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }
                //phan quyen


                var caidat = db.CaiDats;

                var tieuDe = caidat.Where(m => m.MaCaiDat == "tieude_trangchu").FirstOrDefault();
                var noiDung = caidat.Where(m => m.MaCaiDat == "noidung_trangchu").FirstOrDefault();
                var tieuDeTop = caidat.Where(m => m.MaCaiDat == "tieude_top_trangchu").FirstOrDefault();
                var noiDungTop = caidat.Where(m => m.MaCaiDat == "noidung_top_trangchu").FirstOrDefault();

                var ptNapThe = caidat.Where(m => m.MaCaiDat == "phan_tram_nap_the").FirstOrDefault();

                ViewBag.TieuDeTrangChu = tieuDe?.NoiDung ?? "";
                ViewBag.NoiDungTrangChu = noiDung?.NoiDung ?? "";
                ViewBag.TieuDeTopTrangChu = tieuDeTop?.NoiDung ?? "";
                ViewBag.NoiDungTopTrangChu = noiDungTop?.NoiDung ?? "";
                ViewBag.ptNapThe = ptNapThe?.NoiDung ?? "80";

                var facebook = caidat.Where(m => m.MaCaiDat == "facebook").FirstOrDefault();
                var hoTro = caidat.Where(m => m.MaCaiDat == "ho_tro").FirstOrDefault();
                var thuAcc = caidat.Where(m => m.MaCaiDat == "thu_acc").FirstOrDefault();
                var checkUt = caidat.Where(m => m.MaCaiDat == "check_ut").FirstOrDefault();
                var sdt = caidat.Where(m => m.MaCaiDat == "sdt").FirstOrDefault();

                var stk = caidat.Where(m => m.MaCaiDat == "stk").FirstOrDefault();
                var chuNh = caidat.Where(m => m.MaCaiDat == "chu_tk_ngan_hang").FirstOrDefault();
                var nganhang = caidat.Where(m => m.MaCaiDat == "ngan_hang").FirstOrDefault();

                var momo = caidat.Where(m => m.MaCaiDat == "momo").FirstOrDefault();
                var chummm = caidat.Where(m => m.MaCaiDat == "chu_tk_momo").FirstOrDefault();
                var token_mm = caidat.Where(m => m.MaCaiDat == "token_mm").FirstOrDefault();

                var parnerId = caidat.Where(m => m.MaCaiDat == "parner_id").FirstOrDefault();
                var parnerKey = caidat.Where(m => m.MaCaiDat == "parner_key").FirstOrDefault();
                var key_web = caidat.Where(m => m.MaCaiDat == "key_web").FirstOrDefault();

                var tenmien = caidat.Where(m => m.MaCaiDat == "ten_mien").FirstOrDefault();

                var napGame = caidat.Where(m => m.MaCaiDat == "nap_game").FirstOrDefault();
                var cayThue = caidat.Where(m => m.MaCaiDat == "cay_thue").FirstOrDefault();

                var logo = caidat.Where(m => m.MaCaiDat == "logo").FirstOrDefault();
                var bannerHome = caidat.Where(m => m.MaCaiDat == "banner_home").FirstOrDefault();
                var footerDetail = caidat.Where(m => m.MaCaiDat == "footer_detail").FirstOrDefault();
                var ctvCollab = caidat.Where(m => m.MaCaiDat == "is_ctv_collab").FirstOrDefault();
                var isSaleTet = caidat.Where(m => m.MaCaiDat == "is_sale_tet").FirstOrDefault();
                var timeSaleTet = caidat.Where(m => m.MaCaiDat == "time_sale_tet").FirstOrDefault();
                var phanTramSale = caidat.Where(m => m.MaCaiDat == "phan_tram_sale_tet").FirstOrDefault();

                var thong_bao_napthecao = caidat.Where(m => m.MaCaiDat == "thong_bao_napthecao").FirstOrDefault();
                var is_thong_bao_napthecao = caidat.Where(m => m.MaCaiDat == "is_thong_bao_napthecao").FirstOrDefault();
                ViewBag.napGame = napGame?.NoiDung ?? "";
                ViewBag.cayThue = cayThue?.NoiDung ?? "";

                if (key_web == null)
                {
                    CaiDat setCaiDat = new CaiDat();
                    setCaiDat.MaCaiDat = "key_web";
                    Random random = new Random();
                    setCaiDat.NoiDung = Mahoa.MyString.md5(Convert.ToString(random.Next(0, 100001)));
                    db.CaiDats.Add(setCaiDat);
                    db.SaveChanges();
                    key_web = setCaiDat;
                }

                ViewBag.facebook = facebook?.NoiDung ?? "";
                ViewBag.thuAcc = thuAcc?.NoiDung ?? "";
                ViewBag.checkUt = checkUt?.NoiDung ?? "";
                ViewBag.sdt = sdt?.NoiDung ?? "";
                ViewBag.hoTro = hoTro?.NoiDung ?? "";

                ViewBag.stk = stk?.NoiDung ?? "";
                ViewBag.chuNh = chuNh?.NoiDung ?? "";
                ViewBag.nganhang = nganhang?.NoiDung ?? "";

                ViewBag.parnerId = parnerId?.NoiDung ?? "";
                ViewBag.parnerKey = parnerKey?.NoiDung ?? "";
                ViewBag.key_web = key_web?.NoiDung ?? "";

                ViewBag.tenmien = tenmien?.NoiDung ?? "";

                ViewBag.momo = momo?.NoiDung ?? "";
                ViewBag.chummm = chummm?.NoiDung ?? "";
                ViewBag.token_mm = token_mm?.NoiDung ?? "";

                ViewBag.logo = logo?.NoiDung ?? "";
                ViewBag.bannerHome = bannerHome?.NoiDung ?? "";
                ViewBag.footerDetail = footerDetail?.NoiDung ?? "";
                ViewBag.ctvCollab = ctvCollab?.NoiDung ?? "";
                ViewBag.isSaleTet = isSaleTet?.NoiDung ?? "";
                ViewBag.timeSaleTet = timeSaleTet?.NoiDung ?? "";
                ViewBag.phanTramSale = phanTramSale?.NoiDung ?? "";

                ViewBag.thongBaoNapTheCao = thong_bao_napthecao?.NoiDung ?? "";
                ViewBag.isThongBaoNapTheCao = is_thong_bao_napthecao?.NoiDung ?? "";
                var homeTitle = caidat.Where(m => m.MaCaiDat == "home_title").FirstOrDefault();
                ViewBag.homeTitle = homeTitle?.NoiDung ?? "";


                return View();
            }



        }

        // Bật / Tắt Sale Tết
        [HttpPost]
        public ActionResult IsSaleTet(bool isSaleTet)
        {
            Console.WriteLine("isSaleTet: " + isSaleTet); // Ghi log
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện chức năng này!" });
            }
            using (var db = new accgameEntities())
            {
                var saleTetSetting = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "is_sale_tet");
                if (saleTetSetting != null)
                {
                    saleTetSetting.NoiDung = isSaleTet ? "1" : "0";
                }
                else
                {
                    db.CaiDats.Add(new CaiDat
                    {
                        MaCaiDat = "is_sale_tet",
                        NoiDung = isSaleTet ? "1" : "0"
                    });
                }
                var accs = db.Accs.Where(a => a.DaBan != true && a.Xoa != true).ToList();

                // Áp dụng giảm giá theo phần trăm vừa set
                foreach (var acc in accs)
                {
                    if (acc.GiaGoc.HasValue)
                    {
                        // Tính giá giảm
                        var giaGoc = acc.GiaGoc ?? acc.Gia.Value; // Dùng giá gốc nếu có
                        if (isSaleTet)
                        {
                            var phanTramSaleSetting = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "phan_tram_sale_tet");
                            var phanTramSale = phanTramSaleSetting != null ? Convert.ToInt32(phanTramSaleSetting.NoiDung) : 0;
                            acc.Gia = giaGoc - (giaGoc * (phanTramSale + acc.PhanTram) / 100);

                        }
                        else
                        {
                            acc.Gia = giaGoc - (giaGoc * acc.PhanTram / 100);
                        }

                    }
                }

                db.SaveChanges();
                return Json(new { success = true, message = "Cấu hình Sale Tết đã được cập nhật!" });
            }
        }

        // Cài đặt số giờ chạy ngược
        [HttpPost]
        public ActionResult TimerSale(decimal timeSaleTet)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện chức năng này!" });
            }

            using (var db = new accgameEntities())
            {
                var startTime = DateTime.Now; // Thời gian hiện tại
                var endTime = startTime.AddHours((double)timeSaleTet); // Thời gian kết thúc

                var startSetting = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "time_sale_start");
                var endSetting = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "time_sale_end");

                // Tìm cấu hình time_sale_tet
                var timerSetting = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "time_sale_tet");
                if (timerSetting != null)
                {
                    // Cập nhật giá trị nếu đã tồn tại
                    timerSetting.NoiDung = timeSaleTet.ToString();
                }
                else
                {
                    // Tạo mới nếu chưa có
                    db.CaiDats.Add(new CaiDat
                    {
                        MaCaiDat = "time_sale_tet",
                        NoiDung = timeSaleTet.ToString()
                    });
                }
                // Lưu thời gian bắt đầu
                if (startSetting != null)
                {
                    startSetting.NoiDung = startTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    db.CaiDats.Add(new CaiDat
                    {
                        MaCaiDat = "time_sale_start",
                        NoiDung = startTime.ToString("yyyy-MM-dd HH:mm:ss")
                    });
                }

                // Lưu thời gian kết thúc
                if (endSetting != null)
                {
                    endSetting.NoiDung = endTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    db.CaiDats.Add(new CaiDat
                    {
                        MaCaiDat = "time_sale_end",
                        NoiDung = endTime.ToString("yyyy-MM-dd HH:mm:ss")
                    });
                }

                db.SaveChanges();
                return Json(new { success = true, message = "Thời gian Sale Tết đã được cập nhật!" });

            }
        }

        //  lấy thời gian còn lại
        [HttpGet]
        public ActionResult GetRemainingTime()
        {
            using (var db = new accgameEntities())
            {
                var endSetting = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "time_sale_end");
                if (endSetting == null || string.IsNullOrEmpty(endSetting.NoiDung))
                {
                    return Json(new { success = false, remainingTime = 0 }, JsonRequestBehavior.AllowGet);
                }

                DateTime endTime = DateTime.Parse(endSetting.NoiDung);
                TimeSpan remainingTime = endTime - DateTime.Now;

                // Nếu thời gian đã hết, trả về 0
                if (remainingTime.TotalSeconds <= 0)
                {
                    return Json(new { success = true, remainingTime = 0 }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = true, remainingTime = remainingTime.TotalSeconds }, JsonRequestBehavior.AllowGet);
            }
        }

        // Tự động tắt Sale Tết khi hết giờ
        [HttpPost]
        public ActionResult StopSaleTet()
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện chức năng này!" });
            }

            using (var db = new accgameEntities())
            {
                // Tìm và cập nhật cấu hình is_sale_tet
                var saleTetSetting = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "is_sale_tet");
                if (saleTetSetting != null)
                {
                    saleTetSetting.NoiDung = "0"; // Tắt Sale Tết
                    var accs = db.Accs.Where(a => a.DaBan != true && a.Xoa != true).ToList();

                    // Áp dụng giảm giá theo phần trăm vừa set
                    foreach (var acc in accs)
                    {
                        if (acc.GiaGoc.HasValue || acc.Gia.HasValue)
                        {
                            var giaGoc = acc.GiaGoc ?? acc.Gia.Value; // Dùng giá gốc nếu có, nếu không thì dùng giá thường
                            var giaMoi = giaGoc - (giaGoc * (acc.PhanTram ?? 0) / 100); // Đảm bảo acc.PhanTram không bị null

                            acc.Gia = (int)giaMoi;
                        }

                    }
                    db.SaveChanges();
                    return Json(new { success = true, message = "Sale Tết đã được cập nhật thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Không tìm thấy cấu hình Sale Tết." });
                }
            }
        }

        // Function để cập nhật phần trăm Sale Tết
        [HttpPost]
        public ActionResult PercentSale(int phanTramSale)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện chức năng này!" });
            }

            using (var db = new accgameEntities())
            {
                var percentSaleSetting = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "phan_tram_sale_tet");
                if (percentSaleSetting != null)
                {
                    percentSaleSetting.NoiDung = phanTramSale.ToString();
                }
                else
                {
                    db.CaiDats.Add(new CaiDat
                    {
                        MaCaiDat = "phan_tram_sale_tet",
                        NoiDung = phanTramSale.ToString()
                    });
                }
                var accs = db.Accs.Where(a => a.DaBan != true && a.Xoa != true).ToList();

                // Áp dụng giảm giá theo phần trăm vừa set
                foreach (var acc in accs)
                {
                    if (acc.GiaGoc.HasValue || acc.Gia.HasValue)
                    {
                        var giaGoc = acc.GiaGoc ?? acc.Gia.Value; // Dùng giá gốc nếu có, nếu không thì dùng giá thường
                        var giaMoi = giaGoc - (giaGoc * (acc.PhanTram ?? 0) / 100); // Đảm bảo acc.PhanTram không bị null

                        acc.Gia = (int)giaMoi;
                    }

                }

                db.SaveChanges();
                return Json(new { success = true, message = "Phần trăm Sale Tết đã được cập nhật!" });
            }
        }

        public ActionResult infoPage(HttpPostedFileBase logo, HttpPostedFileBase banner_home, string footer_detail, string home_title)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }

            using (var db = new accgameEntities())
            {
                // Handle logo upload
                if (logo != null)
                {
                    var logoFileName = "Logo" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmssfff"));
                    var logoExtension = Path.GetExtension(logo.FileName).ToLower();

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
                    if (allowedExtensions.Contains(logoExtension))
                    {
                        logoFileName += logoExtension;
                    }
                    else
                    {
                        logoFileName += ".webp";
                        logoExtension = ".webp";
                    }

                    var logoSavePath = Path.Combine(Server.MapPath("/Content/images/"), logoFileName);
                    if (logoExtension != ".gif") // Handle non-GIF images
                    {
                        using (var logoMemoryStream = new MemoryStream())
                        {
                            logo.InputStream.CopyTo(logoMemoryStream);
                            byte[] logoFileBytes = logoMemoryStream.ToArray();
                            using (var logoImage = new MagickImage(logoFileBytes))
                            {
                                if (logoImage.Width > 1000)
                                {
                                    int newWidth = 1000;
                                    int newHeight = (int)(logoImage.Height * (1000.0 / logoImage.Width));
                                    logoImage.Resize(newWidth, newHeight);
                                }
                                logoImage.Quality = 75;
                                logoImage.Format = GetImageFormat(logoExtension);
                                logoImage.Write(logoSavePath);
                            }
                        }
                    }
                    else // Handle GIFs without changing
                    {
                        logo.SaveAs(logoSavePath);
                    }

                    var getLogo = db.CaiDats.Where(m => m.MaCaiDat == "logo").FirstOrDefault();
                    if (getLogo == null)
                    {
                        var setLogo = new CaiDat { MaCaiDat = "logo", NoiDung = "/Content/images/" + logoFileName };
                        db.CaiDats.Add(setLogo);
                    }
                    else
                    {
                        var setLogo = db.CaiDats.Find(getLogo.IDCaiDat);
                        setLogo.NoiDung = "/Content/images/" + logoFileName;
                    }
                }

                // Handle banner_home upload
                if (banner_home != null)
                {
                    var bannerFileName = "BannerHome" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmssfff"));
                    var bannerExtension = Path.GetExtension(banner_home.FileName).ToLower();

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    if (allowedExtensions.Contains(bannerExtension))
                    {
                        bannerFileName += bannerExtension;
                    }
                    else
                    {
                        bannerFileName += ".webp";
                        bannerExtension = ".webp";
                    }

                    var bannerSavePath = Path.Combine(Server.MapPath("/Content/images/"), bannerFileName);
                    if (bannerExtension != ".gif") // Handle non-GIF images
                    {
                        using (var bannerMemoryStream = new MemoryStream())
                        {
                            banner_home.InputStream.CopyTo(bannerMemoryStream);
                            byte[] bannerFileBytes = bannerMemoryStream.ToArray();
                            using (var bannerImage = new MagickImage(bannerFileBytes))
                            {
                                if (bannerImage.Width > 1000)
                                {
                                    int newWidth = 1000;
                                    int newHeight = (int)(bannerImage.Height * (1000.0 / bannerImage.Width));
                                    bannerImage.Resize(newWidth, newHeight);
                                }
                                bannerImage.Quality = 75;
                                bannerImage.Format = GetImageFormat(bannerExtension);
                                bannerImage.Write(bannerSavePath);
                            }
                        }
                    }
                    else // Handle GIFs without changing
                    {
                        banner_home.SaveAs(bannerSavePath);
                    }

                    var getBanner = db.CaiDats.Where(m => m.MaCaiDat == "banner_home").FirstOrDefault();
                    if (getBanner == null)
                    {
                        var setBanner = new CaiDat { MaCaiDat = "banner_home", NoiDung = "/Content/images/" + bannerFileName };
                        db.CaiDats.Add(setBanner);
                    }
                    else
                    {
                        var setBanner = db.CaiDats.Find(getBanner.IDCaiDat);
                        setBanner.NoiDung = "/Content/images/" + bannerFileName;
                    }
                }

                // Update other settings
                var getFd = db.CaiDats.Where(m => m.MaCaiDat == "footer_detail").FirstOrDefault();
                if (getFd == null)
                {
                    var setFd = new CaiDat { MaCaiDat = "footer_detail", NoiDung = footer_detail };
                    db.CaiDats.Add(setFd);
                }
                else
                {
                    var setFd = db.CaiDats.Find(getFd.IDCaiDat);
                    setFd.NoiDung = footer_detail;
                }

                var getHt = db.CaiDats.Where(m => m.MaCaiDat == "home_title").FirstOrDefault();
                if (getHt == null)
                {
                    var setHt = new CaiDat { MaCaiDat = "home_title", NoiDung = home_title };
                    db.CaiDats.Add(setHt);
                }
                else
                {
                    var setHt = db.CaiDats.Find(getHt.IDCaiDat);
                    setHt.NoiDung = home_title;
                }

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        private MagickFormat GetImageFormat(string extension)
        {
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    return MagickFormat.Jpeg;
                case ".png":
                    return MagickFormat.Png;
                case ".gif":
                    return MagickFormat.Gif;
                case ".webp":
                    return MagickFormat.WebP;
                default:
                    return MagickFormat.Jpeg; // Default to JPEG if the extension is unknown
            }
        }

        [HttpPost]
        public ActionResult isThongBaoNapTheCao(bool isThongBaoNapTheCao)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện chức năng này!" });
            }
            using (var db = new accgameEntities())
            {
                var saleTetSetting = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "is_thong_bao_napthecao");
                if (saleTetSetting != null)
                {
                    saleTetSetting.NoiDung = isThongBaoNapTheCao ? "1" : "0";
                }
                else
                {
                    db.CaiDats.Add(new CaiDat
                    {
                        MaCaiDat = "is_sale_tet",
                        NoiDung = isThongBaoNapTheCao ? "1" : "0"
                    });
                }
                var accs = db.Accs.Where(a => a.DaBan != true && a.Xoa != true).ToList();


                db.SaveChanges();
                return Json(new { success = true, message = "Cấu hình Sale Tết đã được cập nhật!" });
            }
        }


        [ValidateInput(false)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult thongBaoNapTheCao(string noidung)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            using (var db = new accgameEntities())
            {
                try
                {

                    var getNoiDung = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "thong_bao_napthecao");
                    if (getNoiDung == null)
                    {
                        var setNoiDung = new CaiDat();
                        setNoiDung.MaCaiDat = "thong_bao_napthecao";
                        setNoiDung.NoiDung = noidung;
                        db.CaiDats.Add(setNoiDung);
                    }
                    else
                    {
                        getNoiDung.NoiDung = noidung;
                    }

                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    // Lặp qua tất cả các lỗi trong DbEntityValidationException và hiển thị chúng
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            return Content(validationError.PropertyName + "ss: " +
                                validationError.ErrorMessage);
                        }
                    }
                    // Xử lý lỗi theo cách phù hợp với ứng dụng của bạn, có thể là hiển thị thông báo cho người dùng hoặc ghi log lỗi.
                }
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult ctvCollab(string isctvcollab)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            using (var db = new accgameEntities())
            {
                var getCtv = db.CaiDats.Where(m => m.MaCaiDat == "is_ctv_collab").FirstOrDefault();

                var setPt = db.CaiDats.Find(getCtv.IDCaiDat);
                setPt.NoiDung = isctvcollab == "true" ? "active" : null;
                setPt.ThoiGianCapNhat = DateTime.Now; // Cập nhật thời gian hiện tại

                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult ptnapthe(string pt)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            using (var db = new accgameEntities())
            {
                var getPt = db.CaiDats.Where(m => m.MaCaiDat == "phan_tram_nap_the").FirstOrDefault();
                if (getPt == null)
                {
                    var setPt = new CaiDat();
                    setPt.MaCaiDat = "phan_tram_nap_the";
                    setPt.NoiDung = pt;
                    db.CaiDats.Add(setPt);
                    db.SaveChanges();
                }
                else
                {
                    var setPt = db.CaiDats.Find(getPt.IDCaiDat);
                    setPt.NoiDung = pt;
                    db.SaveChanges();
                }

            }
            return RedirectToAction("Index");
        }
        [ValidateInput(false)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Home(string tieude, string noidung)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            using (var db = new accgameEntities())
            {
                try
                {
                    var getTieuDe = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "tieude_trangchu");
                    if (getTieuDe == null)
                    {
                        var setTieuDe = new CaiDat();
                        setTieuDe.MaCaiDat = "tieude_trangchu";
                        setTieuDe.NoiDung = tieude;
                        db.CaiDats.Add(setTieuDe);
                    }
                    else
                    {
                        getTieuDe.NoiDung = tieude;
                    }
                    var getNoiDung = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "noidung_trangchu");
                    if (getNoiDung == null)
                    {
                        var setNoiDung = new CaiDat();
                        setNoiDung.MaCaiDat = "noidung_trangchu";
                        setNoiDung.NoiDung = noidung;
                        db.CaiDats.Add(setNoiDung);
                    }
                    else
                    {
                        getNoiDung.NoiDung = noidung;
                    }

                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    // Lặp qua tất cả các lỗi trong DbEntityValidationException và hiển thị chúng
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            return Content(validationError.PropertyName + "ss: " +
                                validationError.ErrorMessage);
                        }
                    }
                    // Xử lý lỗi theo cách phù hợp với ứng dụng của bạn, có thể là hiển thị thông báo cho người dùng hoặc ghi log lỗi.
                }
            }
            return RedirectToAction("Index");
        }

        [ValidateInput(false)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TopHome(string tieude, string noidung)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            using (var db = new accgameEntities())
            {
                try
                {
                    var getTieuDe = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "tieude_top_trangchu");
                    if (getTieuDe == null)
                    {
                        var setTieuDe = new CaiDat();
                        setTieuDe.MaCaiDat = "tieude_top_trangchu";
                        setTieuDe.NoiDung = tieude;
                        db.CaiDats.Add(setTieuDe);
                    }
                    else
                    {
                        getTieuDe.NoiDung = tieude;
                    }

                    var getNoiDung = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "noidung_top_trangchu");
                    if (getNoiDung == null)
                    {
                        var setNoiDung = new CaiDat();
                        setNoiDung.MaCaiDat = "noidung_top_trangchu";
                        setNoiDung.NoiDung = noidung;
                        db.CaiDats.Add(setNoiDung);
                    }
                    else
                    {
                        getNoiDung.NoiDung = noidung;
                    }

                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    // Lặp qua tất cả các lỗi trong DbEntityValidationException và hiển thị chúng
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            return Content(validationError.PropertyName + "ss: " +
                                validationError.ErrorMessage);
                        }
                    }
                    // Xử lý lỗi theo cách phù hợp với ứng dụng của bạn, có thể là hiển thị thông báo cho người dùng hoặc ghi log lỗi.
                }
            }
            return RedirectToAction("Index");
        }

        [ValidateInput(false)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult social(string facebook, string thu_acc, string check_ut, string sdt, string ho_tro)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            using (var db = new accgameEntities())
            {
                try
                {
                    // Kiểm tra xem dữ liệu đầu vào có rỗng không trước khi lưu vào cơ sở dữ liệu
                    if (!string.IsNullOrEmpty(facebook))
                    {
                        var getFacebook = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "facebook");
                        if (getFacebook == null)
                        {
                            var setFacebook = new CaiDat();
                            setFacebook.MaCaiDat = "facebook";
                            setFacebook.NoiDung = facebook;
                            db.CaiDats.Add(setFacebook);
                        }
                        else
                        {
                            getFacebook.NoiDung = facebook;
                        }
                    }

                    if (!string.IsNullOrEmpty(thu_acc))
                    {
                        var getThuAcc = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "thu_acc");
                        if (getThuAcc == null)
                        {
                            var setThuAcc = new CaiDat();
                            setThuAcc.MaCaiDat = "thu_acc";
                            setThuAcc.NoiDung = thu_acc;
                            db.CaiDats.Add(setThuAcc);
                        }
                        else
                        {
                            getThuAcc.NoiDung = thu_acc;
                        }
                    }

                    if (!string.IsNullOrEmpty(check_ut))
                    {
                        var getCheckUt = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "check_ut");
                        if (getCheckUt == null)
                        {
                            var setCheckUt = new CaiDat();
                            setCheckUt.MaCaiDat = "check_ut";
                            setCheckUt.NoiDung = check_ut;
                            db.CaiDats.Add(setCheckUt);
                        }
                        else
                        {
                            getCheckUt.NoiDung = check_ut;
                        }
                    }

                    if (!string.IsNullOrEmpty(sdt))
                    {
                        var getSdt = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "sdt");
                        if (getSdt == null)
                        {
                            var setSdt = new CaiDat();
                            setSdt.MaCaiDat = "sdt";
                            setSdt.NoiDung = sdt;
                            db.CaiDats.Add(setSdt);
                        }
                        else
                        {
                            getSdt.NoiDung = sdt;
                        }
                    }

                    if (!string.IsNullOrEmpty(facebook))
                    {
                        var getHoTro = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "ho_tro");
                        if (getHoTro == null)
                        {
                            var setHoTro = new CaiDat();
                            setHoTro.MaCaiDat = "ho_tro";
                            setHoTro.NoiDung = ho_tro;
                            db.CaiDats.Add(setHoTro);
                        }
                        else
                        {
                            getHoTro.NoiDung = ho_tro;
                        }
                    }
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    // Lặp qua tất cả các lỗi trong DbEntityValidationException và hiển thị chúng
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            return Content(validationError.PropertyName + "ss: " +
                                validationError.ErrorMessage);
                        }
                    }
                    // Xử lý lỗi theo cách phù hợp với ứng dụng của bạn, có thể là hiển thị thông báo cho người dùng hoặc ghi log lỗi.
                }
            }
            return RedirectToAction("Index");
        }

        [ValidateInput(false)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult nap(string stk, string chu_tk_ngan_hang, string ngan_hang, string momo, string chu_tk_momo, string parner_id, string parner_key, string token_mm)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            using (var db = new accgameEntities())
            {
                try
                {
                    // Kiểm tra xem dữ liệu đầu vào có rỗng không trước khi lưu vào cơ sở dữ liệu
                    var getStk = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "stk");
                    if (getStk == null)
                    {
                        var setStk = new CaiDat();
                        setStk.MaCaiDat = "stk";
                        setStk.NoiDung = stk;
                        db.CaiDats.Add(setStk);
                    }
                    else
                    {
                        getStk.NoiDung = stk;
                    }

                    var getChutknh = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "chu_tk_ngan_hang");
                    if (getChutknh == null)
                    {
                        var setChutknh = new CaiDat();
                        setChutknh.MaCaiDat = "chu_tk_ngan_hang";
                        setChutknh.NoiDung = chu_tk_ngan_hang;
                        db.CaiDats.Add(setChutknh);
                    }
                    else
                    {
                        getChutknh.NoiDung = chu_tk_ngan_hang;
                    }

                    var getNganHang = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "ngan_hang");
                    if (getNganHang == null)
                    {
                        var setNganHang = new CaiDat();
                        setNganHang.MaCaiDat = "ngan_hang";
                        setNganHang.NoiDung = ngan_hang;
                        db.CaiDats.Add(setNganHang);
                    }
                    else
                    {
                        getNganHang.NoiDung = ngan_hang;
                    }

                    var getMomo = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "momo");
                    if (getMomo == null)
                    {
                        var setMomo = new CaiDat();
                        setMomo.MaCaiDat = "momo";
                        setMomo.NoiDung = momo;
                        db.CaiDats.Add(setMomo);
                    }
                    else
                    {
                        getMomo.NoiDung = momo;
                    }

                    var getChutkmomo = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "chu_tk_momo");
                    if (getChutkmomo == null)
                    {
                        var setChutkmomo = new CaiDat();
                        setChutkmomo.MaCaiDat = "chu_tk_momo";
                        setChutkmomo.NoiDung = chu_tk_momo;
                        db.CaiDats.Add(setChutkmomo);
                    }
                    else
                    {
                        getChutkmomo.NoiDung = chu_tk_momo;
                    }

                    var getPnid = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "parner_id");
                    if (getPnid == null)
                    {
                        var setPnid = new CaiDat();
                        setPnid.MaCaiDat = "parner_id";
                        setPnid.NoiDung = parner_id;
                        db.CaiDats.Add(setPnid);
                    }
                    else
                    {
                        getPnid.NoiDung = parner_id;
                    }

                    var getKey = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "parner_key");
                    if (getKey == null)
                    {
                        var setKey = new CaiDat();
                        setKey.MaCaiDat = "parner_key";
                        setKey.NoiDung = parner_key;
                        db.CaiDats.Add(setKey);
                    }
                    else
                    {
                        getKey.NoiDung = parner_key;
                    }

                    var getTokenMM = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "token_mm");
                    if (getTokenMM == null)
                    {
                        var setTokenMM = new CaiDat();
                        setTokenMM.MaCaiDat = "token_mm";
                        setTokenMM.NoiDung = token_mm;
                        db.CaiDats.Add(setTokenMM);
                    }
                    else
                    {
                        getTokenMM.NoiDung = token_mm;
                    }
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    // Lặp qua tất cả các lỗi trong DbEntityValidationException và hiển thị chúng
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            return Content(validationError.PropertyName + "ss: " +
                                validationError.ErrorMessage);
                        }
                    }
                    // Xử lý lỗi theo cách phù hợp với ứng dụng của bạn, có thể là hiển thị thông báo cho người dùng hoặc ghi log lỗi.
                }
            }
            return RedirectToAction("Index");
        }

        [ValidateInput(false)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult dichvu(string nap_game, string cay_thue)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            using (var db = new accgameEntities())
            {
                try
                {
                    // Kiểm tra xem dữ liệu đầu vào có rỗng không trước khi lưu vào cơ sở dữ liệu
                    var getNapGame = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "nap_game");
                    if (getNapGame == null)
                    {
                        var setNapGame = new CaiDat();
                        setNapGame.MaCaiDat = "nap_game";
                        setNapGame.NoiDung = nap_game;
                        db.CaiDats.Add(setNapGame);
                    }
                    else
                    {
                        getNapGame.NoiDung = nap_game;
                    }
                    var getCayThue = db.CaiDats.FirstOrDefault(m => m.MaCaiDat == "cay_thue");
                    if (getCayThue == null)
                    {
                        var setCayThue = new CaiDat();
                        setCayThue.MaCaiDat = "cay_thue";
                        setCayThue.NoiDung = cay_thue;
                        db.CaiDats.Add(setCayThue);
                    }
                    else
                    {
                        getCayThue.NoiDung = cay_thue;
                    }
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    // Lặp qua tất cả các lỗi trong DbEntityValidationException và hiển thị chúng
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            return Content(validationError.PropertyName + "ss: " +
                                validationError.ErrorMessage);
                        }
                    }
                    // Xử lý lỗi theo cách phù hợp với ứng dụng của bạn, có thể là hiển thị thông báo cho người dùng hoặc ghi log lỗi.
                }
            }
            return RedirectToAction("Index");
        }
    }
}