using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using System.Data.Entity;
using PagedList;
using accgame.Filters;
using System.IO;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    
    public class quanlynapgameController : Controller
    {
        // GET: quantri/quanlynapgame

        public ActionResult Index(int? page, int? rows, int? searchID, string hoanthanh, string nguoigui)
        {
            ViewBag.qlng = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }

            // Thiết lập số dòng mặc định
            int pageSize = rows ?? 10; // Mặc định là 10 dòng nếu không có giá trị được chọn
            int pageNumber = page ?? 1; // Mặc định là trang 1 nếu không có giá trị được chọn

            using (var db = new accgameEntities())
            {
                var query = db.NapGames.AsQueryable();

                // Tìm kiếm theo mã số
                if (searchID.HasValue)
                {
                    query = query.Where(m => m.IDNapGame == searchID.Value);
                }

                if (!string.IsNullOrEmpty(hoanthanh))
                {
                    if (hoanthanh == "1")
                    {
                        query = query.Where(m => m.HoanThanh == true);
                    }
                    if (hoanthanh == "0")
                    {
                        query = query.Where(m => m.HoanThanh == false);
                    }
                    if (hoanthanh == "null")
                    {
                        query = query.Where(m => m.HoanThanh == null);
                    }
                }

                if (!string.IsNullOrEmpty(nguoigui))
                {
                    query = query.Where(m => m.NguoiDung.TenNguoiDung.Contains(nguoigui));
                }

                // Pagination
                var totalItems = query.Count();
                var napgame = query.Include(n => n.NguoiDung).Include(m => m.NguoiDung1).Include(n => n.NguoiDung2)
                                   .Include(n => n.GoiNap)
                                   .OrderByDescending(m => m.IDNapGame).ToPagedList(pageNumber, pageSize);

                ViewBag.Rows = pageSize;
                ViewBag.CurrentPage = pageNumber;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                ViewBag.page = pageNumber;
                ViewBag.last_page = napgame.PageCount;
                ViewBag.SearchID = searchID;
                ViewBag.hoanthanh = hoanthanh;
                ViewBag.nguoigui = nguoigui;

                // Lấy giá trị của MaCaiDat = 'mail_nap_game'
                var mailNapGame = db.CaiDats
                    .Where(c => c.MaCaiDat == "mail_nap_game")
                    .Select(c => c.NoiDung)
                    .FirstOrDefault();

                // Kiểm tra nếu giá trị null
                if (string.IsNullOrEmpty(mailNapGame))
                {
                    ViewBag.MailNapGame = "";
                }
                else
                {
                    ViewBag.MailNapGame = mailNapGame;
                }
                return View(napgame);
            }
        }


        public ActionResult hoanthanh(int id)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Json(new { success = false, message = "Bạn không có quyền." });
            }

            using (var db = new accgameEntities())
            {
                var napgame = db.NapGames.Find(id);
                if (napgame == null)
                {
                    return Json(new { success = false, message = "Đơn hàng không tồn tại." });
                }
                napgame.HoanThanh = true;
                var setCtv = db.NguoiDungs.Find(idnd);
                if (setCtv != null)
                {
                    var biendongdsad = new BienDongSoDu();
                    biendongdsad.IDNguoiDung = setCtv.IDNguoiDung;
                    biendongdsad.SoTien = napgame.SoTien;
                    biendongdsad.TruTien = false;
                    biendongdsad.ThoiGian = DateTime.Now;
                    biendongdsad.LoiNhan = "Hoàn Thành Nạp Game";
                    biendongdsad.TienTruoc = setCtv.Tien;
                    biendongdsad.TienSau = setCtv.Tien + napgame.SoTien;

                    db.BienDongSoDus.Add(biendongdsad);

                    setCtv.Tien += napgame.SoTien;
                }
                // Tạo thông báo cho người dùng
                var nguoiDungNhanThongBao = db.NguoiDungs.Find(napgame.IDNguoiDung);
                if (nguoiDungNhanThongBao != null)
                {
                    string tenGoiNap = napgame.GoiNap?.TenGoi ?? "Không xác định";
                    string noiDungThongBao = $@"
                    <p>
                        Thông báo đơn nạp 
                        <strong>Mã số đơn #{napgame.IDNapGame}</strong> - 
                        <strong>{tenGoiNap}</strong> đã hoàn thành. 
                        <br>Vui lòng đăng nhập game để kiểm tra!
                    </p>";
                    var notify = new NotifyUser
                    {
                        IdNguoidung = nguoiDungNhanThongBao.IDNguoiDung,
                        Contents = noiDungThongBao,
                        IsRead = false,
                        IsAdminPost = false,
                        Thoigian = DateTime.Now // Nếu bảng NotifyUser có cột này
                    };

                    db.NotifyUsers.Add(notify);
                }
                db.SaveChanges();
                napgame.HoanThanh = true;
                napgame.IDNguoiDuyet = idnd;
                var nguoiDuyet = db.NguoiDungs.Find(idnd);
                string tenNguoiDuyet = nguoiDuyet?.TenNguoiDung ?? "Không xác định";
                // Cập nhật số dư (logic giống trước)
                db.SaveChanges();

                return Json(new { success = true, newStatus = "Đã hoàn thành", nguoiDuyet = tenNguoiDuyet });
            }
        }


        public ActionResult huydon(int id, int? rows)
        {
            ViewBag.qlng = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            using (var db = new accgameEntities())
            {
                var napgame = db.NapGames.Find(id);
                napgame.IDNguoiDuyet = idnd;

                if (napgame.HoanThanh == null)
                {
                    napgame.HoanThanh = false;
                    var nguoidung = db.NguoiDungs.Find(napgame.IDNguoiDung);
                    var tienTruoc = Convert.ToInt32(nguoidung.Tien);

                    nguoidung.Tien = tienTruoc + napgame.SoTien;

                    //Biến động số dư nguoi mua:
                    var biendongdsnm = new BienDongSoDu();
                    biendongdsnm.IDNguoiDung = nguoidung.IDNguoiDung;
                    biendongdsnm.SoTien = napgame.SoTien;
                    biendongdsnm.TruTien = false;
                    biendongdsnm.ThoiGian = DateTime.Now;
                    biendongdsnm.LoiNhan = "Hoàn tiền nạp game";
                    biendongdsnm.TienTruoc = tienTruoc;
                    biendongdsnm.TienSau = tienTruoc + napgame.SoTien;
                    db.BienDongSoDus.Add(biendongdsnm);
                    //
                    //Biến động số dư admin:
                    var admin = db.NguoiDungs.Where(m => m.LeverAdmin == 1).FirstOrDefault();
                    if (admin != null)
                    {
                        var biendongdsad = new BienDongSoDu();
                        biendongdsad.IDNguoiDung = admin.IDNguoiDung;
                        biendongdsad.SoTien = napgame.SoTien;
                        biendongdsad.TruTien = true;
                        biendongdsad.ThoiGian = DateTime.Now;
                        biendongdsad.LoiNhan = "Hoàn tiền nạp game";
                        biendongdsad.TienTruoc = admin.Tien;
                        biendongdsad.TienSau = admin.Tien - napgame.SoTien;

                        db.BienDongSoDus.Add(biendongdsad);

                        var setAdmin = db.NguoiDungs.Find(admin.IDNguoiDung);
                        if (setAdmin != null)
                        {
                            setAdmin.Tien -= (napgame.SoTien);
                        }
                    }


                    

                    db.SaveChanges();
                }
                var nguoiDuyet = db.NguoiDungs.Find(idnd);
                string tenNguoiDuyet = nguoiDuyet?.TenNguoiDung ?? "Không xác định";
                /* string previousUrl = TempData["PreviousUrl"] as string;
                 if (!string.IsNullOrEmpty(previousUrl))
                 {
                     return Redirect(previousUrl);
                 }
                 return RedirectToAction("Index", new { rows = rows });*/
                return Json(new { success = true, newStatus = "Đã hủy", nguoiDuyet = tenNguoiDuyet });

            }
        }

        [HttpPost]
        public JsonResult UpdateMail(string selectedMail)
        {
            try
            {
                using (var db = new accgameEntities())
                {
                    // Tìm bản ghi có MaCaiDat = 'mail_nap_game'
                    var caiDat = db.CaiDats.FirstOrDefault(c => c.MaCaiDat == "mail_nap_game");
                    if (caiDat != null)
                    {
                        // Cập nhật email mới
                        caiDat.NoiDung = selectedMail;
                        db.SaveChanges();
                        return Json(new { success = true, message = "Cập nhật email thành công!" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không tìm thấy cài đặt email." });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateNote()
        {
            using (var db = new accgameEntities())
            {
                try
                {
                    // Lấy dữ liệu JSON từ Request
                    string json = new StreamReader(Request.InputStream).ReadToEnd();
                    var request = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateNoteRequest>(json);

                    // Tìm NapGame theo ID
                    var napGame = db.NapGames.FirstOrDefault(n => n.IDNapGame == request.Id);
                    if (napGame != null)
                    {
                        // Cập nhật ghi chú và AdminEdit
                        napGame.GhiChu = request.Note;
                        napGame.AdminEdit = true;
                        db.SaveChanges();

                        return Json(new { success = true });
                    }

                    return Json(new { success = false, message = "Không tìm thấy đơn nạp!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
                }
            }
        }

        public class UpdateNoteRequest
        {
            public int Id { get; set; }
            public string Note { get; set; }
        }

        [HttpPost]
        public JsonResult SetPickHandler(int idNapGame, bool isPick)
        {
            using (var db = new accgameEntities())
            {
                try
                {
                    var currentUserID = Convert.ToInt32(Session["IDNguoiDung"]);

                    // Giả sử bạn có service hoặc db context để cập nhật trạng thái:
                    // Lấy bản ghi theo idNapGame
                    var item = db.NapGames.Find(idNapGame);
                    if (item == null)
                    {
                        return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                    }

                    if (isPick)
                    {
                        // Người dùng chọn nhận xử lý => gán IDNguoiPick
                        item.IDNguoiPick = currentUserID;
                    }
                    else
                    {
                        // Bỏ chọn => bỏ gán người xử lý
                        item.IDNguoiPick = null;
                    }

                    db.SaveChanges();

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }


        /*
                public ActionResult Daxuly(int? page)
                {
                    using (var db = new accgameEntities())
                    {
                        int pageNumber = (page ?? 1);
                        int newpage = pageNumber - 1;

                        ViewBag.qlng = "active";
                        int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                        int level = Convert.ToInt16(Session["levelAdmin"]);
                        if (level != 1 || idnd == 0)
                        {
                            return Redirect("/");
                        }
                        //phan quyen
                        var napgame = db.NapGames.Where(m => m.HoanThanh != null).Include(n => n.NguoiDung).Include(n => n.GoiNap).OrderByDescending(m => m.IDNapGame).Skip(newpage * 40).Take(40).ToList();

                        decimal count = db.NapGames.Where(m => m.HoanThanh != null).Count();
                        var last_page = Convert.ToInt16(Math.Ceiling(count / 40));
                        ViewBag.last_page = last_page;
                        ViewBag.page = pageNumber;
                        return View(napgame);
                    }
                }
            */

    }

}