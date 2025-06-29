using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using Newtonsoft.Json;
using accgame.Filters;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    
    public class quanlydanhmucController : Controller
    {

        // GET: quantri/quanlydanhmuc
        public ActionResult Index(int? page)
        {
            using (var db = new accgameEntities())
            {
                int pageNumber = (page ?? 1);
                int newpage = pageNumber - 1;

                ViewBag.qldm = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }
                //phan quyen
                var danhmuc = db.DanhMucs.Where(m => m.Xoa != true).OrderByDescending(m => m.LoaiGame).ThenBy(m => m.STT).ThenByDescending(m => m.IDDanhMuc).Include(m => m.NguoiDung).Include(m => m.LoaiGame1).Skip(newpage * 40).Take(40).ToList();
                decimal count = db.DanhMucs.Where(m => m.Xoa != true).Count();
                int last_page = Convert.ToInt16(Math.Ceiling(count / 40));
                ViewBag.last_page = last_page;
                ViewBag.page = pageNumber;
                return View(danhmuc);
            }
        }

        public ActionResult Themmoi(int id)
        {
            ViewBag.qldm = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            using (var db = new accgameEntities())
            {
                // Lấy thông tin Loại Acc và Loại Game
                var loaiAcc = db.LoaiAccs.Where(m => m.IDLoaiAcc == id).FirstOrDefault();
                if (loaiAcc == null)
                {
                    return HttpNotFound();
                }

                var tenLoaiAcc = loaiAcc.TenLoaiAcc;
                var idLoaiGame = loaiAcc.IDLoaiGame;
                var tenLoaiGame = db.LoaiGames.Where(m => m.IDLoaiGame == idLoaiGame).Select(m => m.TenLoaiGame).FirstOrDefault();

                // Thiết lập ViewBag
                ViewBag.idLoaiAcc = id;
                ViewBag.tenLoaiAcc = tenLoaiAcc;
                ViewBag.tenLoaiGame = tenLoaiGame;
                ViewBag.idLoaiGame = idLoaiGame;
                if (Request.UrlReferrer != null)
                {
                    TempData["PreviousUrl"] = Request.UrlReferrer.ToString();
                }
                return View();
            }


        }

        [HttpPost]
        [ValidateInput(false)]

        public ActionResult Themmoi(DanhMuc danhmuc, HttpPostedFileBase ImageUpload, string ImageUploadLink)
        {
            using (var db = new accgameEntities())
            {
                ViewBag.qldm = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }
                //phan quyen
                if (ImageUpload != null && ImageUpload.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ImageUpload.FileName);
                    var filePath = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                    ImageUpload.SaveAs(filePath);

                    // Lưu URL ảnh
                    danhmuc.AnhDanhMuc = "/Content/Images/" + fileName;
                }
                else
                {
                    danhmuc.AnhDanhMuc = ImageUploadLink;
                }
                danhmuc.HienCo = 0;
                db.DanhMucs.Add(danhmuc);
                db.SaveChanges();
                TempData["SuccessThem"] = true;

                string previousUrl = TempData["PreviousUrl"] as string;
                if (!string.IsNullOrEmpty(previousUrl))
                {
                    return Redirect(previousUrl);
                }
                return RedirectToAction("index");
            }

        }


        public ActionResult sua(int id)
        {
            using (var db = new accgameEntities())
            {

                ViewBag.LoaiGame = db.LoaiGames.ToList();
                ViewBag.LoaiAcc = db.LoaiAccs.Where(m => m.DacBiet == true).ToList();
                ViewBag.qldm = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }
                //phan quyen
                var danhmuc = db.DanhMucs.Where(m => m.IDDanhMuc == id).FirstOrDefault();
                if (Request.UrlReferrer != null)
                {
                    TempData["PreviousUrl"] = Request.UrlReferrer.ToString();
                }
                return View(danhmuc);
            }

        }

        /*        public ActionResult sua(int id, DanhMuc danhmuc, string ImageUpload)
                {
                    using (var db = new accgameEntities())
                    {
                        ViewBag.qldm = "active";
                        int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                        int level = Convert.ToInt16(Session["levelAdmin"]);
                        if (level != 1 || idnd == 0)
                        {
                            return Redirect("/");
                        }
                        //phan quyen

                        var setDanhMuc = db.DanhMucs.Find(id);
                        if (setDanhMuc == null)
                        {
                            return RedirectToAction("index");
                        }
                        setDanhMuc.LoaiGame = danhmuc.LoaiGame;
                        setDanhMuc.IDLoaiAcc = danhmuc.IDLoaiAcc;
                        setDanhMuc.TenDanhMuc = danhmuc.TenDanhMuc;
                        setDanhMuc.MoTa = danhmuc.MoTa;
                        setDanhMuc.DanhMucLevel = danhmuc.DanhMucLevel;
                        setDanhMuc.LuotRoll = danhmuc.LuotRoll;
                        setDanhMuc.DaBan = danhmuc.DaBan;
                        setDanhMuc.Gia = danhmuc.Gia;
                        setDanhMuc.GiaCtvCollab = danhmuc.GiaCtvCollab;
                        setDanhMuc.TaiKhoanNoiBo = danhmuc.TaiKhoanNoiBo;
                        setDanhMuc.STT = danhmuc.STT;
                        setDanhMuc.AnhDanhMuc = ImageUpload;
                        setDanhMuc.ThongBaoMuaAcc = danhmuc.ThongBaoMuaAcc;
                        db.SaveChanges();
                        TempData["SuccessSua"] = true;

                        return RedirectToAction("sua", new { id = id });
                    }
                }
        */

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult sua(int id, DanhMuc danhmuc, HttpPostedFileBase ImageUpload, string ImageUploadLink)
        {
            using (var db = new accgameEntities())
            {
                ViewBag.qldm = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Json(new { success = false, message = "Không có quyền thực hiện hành động này." });
                }

                var setDanhMuc = db.DanhMucs.Find(id);
                if (setDanhMuc == null)
                {
                    return Json(new { success = false, message = "Danh mục không tồn tại." });
                }

                // Cập nhật thông tin danh mục
                setDanhMuc.LoaiGame = danhmuc.LoaiGame;
                setDanhMuc.IDLoaiAcc = danhmuc.IDLoaiAcc;
                setDanhMuc.TenDanhMuc = danhmuc.TenDanhMuc;
                setDanhMuc.MoTa = danhmuc.MoTa;
                setDanhMuc.DanhMucLevel = danhmuc.DanhMucLevel;
                setDanhMuc.LuotRoll = danhmuc.LuotRoll;
                setDanhMuc.DaBan = danhmuc.DaBan;
                setDanhMuc.Gia = danhmuc.Gia;
                setDanhMuc.GiaCtvCollab = danhmuc.GiaCtvCollab;
                setDanhMuc.TaiKhoanNoiBo = danhmuc.TaiKhoanNoiBo;
                setDanhMuc.STT = danhmuc.STT;
                if (ImageUpload != null && ImageUpload.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ImageUpload.FileName);
                    var filePath = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                    ImageUpload.SaveAs(filePath);

                    // Lưu URL ảnh mới
                    setDanhMuc.AnhDanhMuc = "/Content/Images/" + fileName;
                }
                else if (!string.IsNullOrEmpty(ImageUploadLink))
                {
                    // Lưu URL ảnh từ liên kết
                    setDanhMuc.AnhDanhMuc = ImageUploadLink;
                }
                // Nếu không có file hoặc link ảnh, giữ nguyên ảnh cũ
                else
                {
                    // Không thay đổi giá trị của AnhDanhMuc
                }
                setDanhMuc.ThongBaoMuaAcc = danhmuc.ThongBaoMuaAcc;

                db.SaveChanges();

                // Trả về JSON khi lưu thành công
                return Json(new { success = true, message = "Chỉnh sửa thành công!" });
            }
        }
        
        public ActionResult xoa(int id)
        {
            using (var db = new accgameEntities())
            {
                ViewBag.qldm = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }
                //phan quyen
                var danhmuc = db.DanhMucs.Find(id);
                danhmuc.Xoa = true;
                db.SaveChanges();
                TempData["SuccessXoa"] = true;

                // Redirect về trang trước đó
                var referrerUrl = Request.UrlReferrer?.ToString();
                if (string.IsNullOrEmpty(referrerUrl))
                {
                    return RedirectToAction("Index");
                }

                return Redirect(referrerUrl);
            }
        }


        public ActionResult danhsachacc(int id, int? page, string sapxep, string searchMa, string searchTK, string searchND)
        {
            using (var db = new accgameEntities())
            {
                ViewBag.qldm = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }

                var danhmuc = db.DanhMucs.Find(id);
                if (danhmuc == null)
                {
                    return RedirectToAction("index");
                }

                int pageNumber = (page ?? 1);
                int newpage = pageNumber - 1;

                ViewBag.tenDanhMuc = danhmuc.TenDanhMuc;

                // Tạo câu truy vấn cơ bản
                var query = db.AccRandoms.Where(m => m.IDDanhMuc == id && m.Xoa != true);

                // Áp dụng lọc theo trạng thái
                if (!string.IsNullOrEmpty(sapxep))
                {
                    if (sapxep == "0") query = query.Where(m => m.DaBan != true);
                    if (sapxep == "1") query = query.Where(m => m.DaBan == true);
                }

                // Áp dụng tìm kiếm theo mã số
                if (!string.IsNullOrEmpty(searchMa))
                {
                    query = query.Where(m => m.IDAccRandom.ToString().Contains(searchMa));
                }

                // Áp dụng tìm kiếm theo tài khoản
                if (!string.IsNullOrEmpty(searchTK))
                {
                    query = query.Where(m => m.TaiKhoan.Contains(searchTK));
                }

                // Áp dụng tìm kiếm theo người đăng
                if (!string.IsNullOrEmpty(searchND))
                {
                    query = query.Where(m => m.NguoiDung.TenNguoiDung.Equals(searchND));
                }

                // Sắp xếp và phân trang
                var listAccRD = query.OrderByDescending(m => m.IDAccRandom).Include(m => m.NguoiDung).Skip(newpage * 40).Take(40).ToList();

                decimal count = query.Count();
                int last_page = Convert.ToInt16(Math.Ceiling(count / 40));
                ViewBag.last_page = last_page;
                ViewBag.page = pageNumber;
                ViewBag.iddanhmuc = id;
                ViewBag.sapxep = sapxep;
                ViewBag.searchMa = searchMa;
                ViewBag.searchTK = searchTK;
                ViewBag.searchND = searchND;
                if (Request.UrlReferrer != null)
                {
                    TempData["PreviousUrl"] = Request.UrlReferrer.ToString();
                }
                return View(listAccRD);
            }
        }

        public ActionResult LoadSoLuong(int id)
        {
            using (var db = new accgameEntities())
            {
                ViewBag.qldm = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }


                var danhMuc = db.DanhMucs.Find(id);
                if (danhMuc == null)
                {
                    TempData["notify"] = "Không tìm thấy danh mục";
                    return RedirectToAction("danhsachacc", new { id = id });
                }

                var countAcc = db.AccRandoms.Where(m => m.IDDanhMuc == id && m.Xoa != true && m.DaBan != true).Count();
                danhMuc.HienCo = countAcc;
                TempData["notify"] = "Đã load lại số lượng tài khoản đang có, số lượng: " + countAcc;
                db.SaveChanges();
                return RedirectToAction("danhsachacc", new { id = id });
            }
        }


        public ActionResult danhsachaccdaban(int id, int? page)
        {
            using (var db = new accgameEntities())
            {
                ViewBag.qldm = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }
                //phan quyen
                var danhmuc = db.DanhMucs.Find(id);
                if (danhmuc == null)
                {
                    return RedirectToAction("index");
                }
                int pageNumber = (page ?? 1);
                int newpage = pageNumber - 1;


                ViewBag.tenDanhMuc = danhmuc.TenDanhMuc;
                var listAccRD = db.AccRandoms.Where(m => m.IDDanhMuc == id && m.Xoa != true && m.DaBan == true).Include(m => m.NguoiDung1).OrderByDescending(m => m.IDAccRandom).Include(m => m.NguoiDung).Skip(newpage * 40).Take(40).ToList();

                decimal count = db.AccRandoms.Where(m => m.IDDanhMuc == id && m.Xoa != true && m.DaBan == true).Count();
                int last_page = Convert.ToInt16(Math.Ceiling(count / 40));
                ViewBag.last_page = last_page;
                ViewBag.page = pageNumber;
                ViewBag.iddanhmuc = id;
                return View(listAccRD);
            }

        }

        public ActionResult themacc(int id)
        {
            using (var db = new accgameEntities())
            {
                ViewBag.qldm = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }

                // Lấy thông tin loại danh mục
                var loaiDanhMuc = db.DanhMucs.Where(m => m.IDDanhMuc == id).FirstOrDefault();
                if (loaiDanhMuc == null)
                {
                    return HttpNotFound();
                }

                var tenDanhMuc = loaiDanhMuc.TenDanhMuc;
                ViewBag.loaigame = id;
                ViewBag.tenDanhMuc = tenDanhMuc;

                // Lưu URL trang trước đó vào TempData
                if (Request.UrlReferrer != null)
                {
                    TempData["PreviousUrl"] = Request.UrlReferrer.ToString();
                }

                return View();
            }
        }

        [HttpPost]
        public ActionResult themacc(AccRandom accRandom)
        {
            using (var db = new accgameEntities())
            {
                ViewBag.qldm = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }

                // phan quyen
                var danhmuc = db.DanhMucs.Find(accRandom.IDDanhMuc);
                if (danhmuc == null)
                {
                    return RedirectToAction("Index");
                }
                danhmuc.HienCo = Convert.ToInt32(danhmuc.HienCo) + 1;
                accRandom.IDNguoiDung = idnd;
                accRandom.ThoiGianDang = DateTime.Now;
                db.AccRandoms.Add(accRandom);
                if (danhmuc.IDLoaiAcc != null)
                {
                    var loaiAcc = db.LoaiAccs.Find(danhmuc.IDLoaiAcc);
                    if (loaiAcc != null)
                    {
                        loaiAcc.DangBan = Convert.ToInt32(loaiAcc.DangBan) + 1;
                    }
                }

                db.SaveChanges();
                TempData["SuccessThem"] = true;

                string previousUrl = TempData["PreviousUrl"] as string;
                if (!string.IsNullOrEmpty(previousUrl))
                {
                    return Redirect(previousUrl);
                }

                // Nếu không có URL trang trước đó, chuyển hướng về danhsachacc
                return RedirectToAction("danhsachacc", new { id = accRandom.IDDanhMuc });
            }
        }

        public ActionResult themnhieuacc(int id)
        {
            using (var db = new accgameEntities())
            {
                ViewBag.qldm = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }

                // Lấy thông tin loại danh mục
                var loaiDanhMuc = db.DanhMucs.Where(m => m.IDDanhMuc == id).FirstOrDefault();
                if (loaiDanhMuc == null)
                {
                    return HttpNotFound();
                }
                // Lấy cấu hình đã lưu trong CauHinhDangNhieuAcc
                var cauHinh = loaiDanhMuc.CauHinhDangNhieuAcc;
                if (!string.IsNullOrEmpty(cauHinh))
                {
                    // Tách chuỗi cấu hình theo dấu |
                    var cauHinhFields = cauHinh.Split('|');

                    // Lưu từng trường vào ViewBag
                    ViewBag.Field1 = cauHinhFields.Length > 0 ? cauHinhFields[0] : null;
                    ViewBag.Field2 = cauHinhFields.Length > 1 ? cauHinhFields[1] : null;
                    ViewBag.Field3 = cauHinhFields.Length > 2 ? cauHinhFields[2] : null;
                    ViewBag.Field4 = cauHinhFields.Length > 3 ? cauHinhFields[3] : null;
                    ViewBag.Field5 = cauHinhFields.Length > 4 ? cauHinhFields[4] : null;
                    ViewBag.Field6 = cauHinhFields.Length > 5 ? cauHinhFields[5] : null;
                }
                ViewBag.Cauhinh = cauHinh;
                ViewBag.loaigame = id;
                ViewBag.tenDanhMuc = loaiDanhMuc.TenDanhMuc;
                ViewBag.GhiChuAcc = loaiDanhMuc.GhiChuAcc;

                // Lưu URL trang trước đó vào TempData
                if (Request.UrlReferrer != null)
                {
                    TempData["PreviousUrl"] = Request.UrlReferrer.ToString();
                }
                return View();
            }
        }


        [HttpPost]
        public ActionResult themnhieuacc(int id, int IDDanhMuc, string thongtin)
        {
            using (var db = new accgameEntities())
            {
                ViewBag.qldm = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }

                var danhmuc = db.DanhMucs.Find(id);
                if (danhmuc == null) return HttpNotFound();

                // Lấy cấu hình từ CauHinhDangNhieuAcc
                var cauHinh = danhmuc.CauHinhDangNhieuAcc?.Split('|') ?? new string[0];

                // Phân tách từng dòng trong thongtin
                var dsacc = thongtin.Split('\n');
                foreach (var item in dsacc)
                {
                    var accrd = new AccRandom();
                    var getacc = item.Split('|');

                    // Ánh xạ các trường theo cấu hình
                    for (int i = 0; i < cauHinh.Length; i++)
                    {
                        var field = cauHinh[i].ToLower();
                        var value = i < getacc.Length ? getacc[i] : null;

                        switch (field)
                        {
                            case "taikhoan":
                                accrd.TaiKhoan = value;
                                break;
                            case "matkhau":
                                accrd.MatKhau = value;
                                break;
                            case "matkhauoutlook":
                                accrd.MatKhauOutlook = value;
                                break;
                            case "mailkhoiphuc":
                                accrd.MailKhoiPhuc = value;
                                break;
                            case "matkhaumailkhoiphuc":
                                accrd.MatKhauMailKhoiPhuc = value;
                                break;
                            case "matkhaumail":
                                accrd.MatKhauMail = value;
                                break;
                        }
                    }

                    // Các thông tin khác của AccRandom
                    accrd.IDDanhMuc = id;
                    accrd.IDNguoiDung = idnd;
                    accrd.ThoiGianDang = DateTime.Now;
                    db.AccRandoms.Add(accrd);

                    // Cập nhật số lượng hiện có trong DanhMuc
                    danhmuc.HienCo = (danhmuc.HienCo ?? 0) + 1;

                    // Cập nhật số lượng đang bán trong LoaiAcc nếu có
                    if (danhmuc.IDLoaiAcc != null)
                    {
                        var loaiAcc = db.LoaiAccs.Find(danhmuc.IDLoaiAcc);
                        if (loaiAcc != null)
                        {
                            loaiAcc.DangBan = (loaiAcc.DangBan ?? 0) + 1;
                        }
                    }
                }

                db.SaveChanges();
                TempData["SuccessThem"] = true;

                // Lấy URL trang trước đó từ TempData
                string previousUrl = TempData["PreviousUrl"] as string;
                if (!string.IsNullOrEmpty(previousUrl))
                {
                    return Redirect(previousUrl);
                }
                return RedirectToAction("danhsachacc", new { id = id });
            }
        }


        // Hàm cập nhật cấu hình của danh mục
        [HttpPost]
        public JsonResult CapNhatCauHinh(int id, string cauHinhDangNhieuAcc, string ghiChu)
        {
            try
            {
                using (var db = new accgameEntities())
                {

                    // Tìm danh mục và cập nhật cấu hình
                    var danhMuc = db.DanhMucs.Find(id);
                    if (danhMuc != null)
                    {
                        danhMuc.CauHinhDangNhieuAcc = cauHinhDangNhieuAcc;
                        danhMuc.GhiChuAcc = ghiChu;
                        db.SaveChanges();
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Danh mục không tồn tại" });
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult updateHideStatus(int id, bool hide)
        {
            using (var db = new accgameEntities())
            {
                // Tìm danh mục theo ID
                var danhMuc = db.DanhMucs.Find(id);
                if (danhMuc == null)
                {
                    return Json(new { message = "Danh mục không tồn tại!" });
                }

                // Cập nhật giá trị Hide
                danhMuc.Hide = hide;

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();

                // Trả về thông báo thành công
                return Json(new { message = "Cập nhật trạng thái thành công!" });
            }
        }
        public ActionResult suaacc(int id)
        {
            using (var db = new accgameEntities())
            {
                ViewBag.qldm = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }
                //phan quyen

                var accrd = db.AccRandoms.Find(id);
                return View(accrd);
            }

        }
        [HttpPost]
        public ActionResult suaacc(AccRandom accRandomVM)
        {
            using (var db = new accgameEntities())
            {
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Json(new { success = false, message = "Không có quyền truy cập!" });
                }

                var accRandom = db.AccRandoms.Find(accRandomVM.IDAccRandom);
                if (accRandom != null)
                {
                    accRandom.TaiKhoan = accRandomVM.TaiKhoan;
                    accRandom.MatKhau = accRandomVM.MatKhau;
                    accRandom.MatKhauOutlook = accRandomVM.MatKhauOutlook;
                    accRandom.MailKhoiPhuc = accRandomVM.MailKhoiPhuc;
                    accRandom.MatKhauMailKhoiPhuc = accRandomVM.MatKhauMailKhoiPhuc;
                    accRandom.MatKhauMail = accRandomVM.MatKhauMail;
                    accRandom.ThongTinKhac = accRandomVM.ThongTinKhac;
                    accRandom.DaBan = accRandomVM.DaBan;

                    db.Entry(accRandom).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(new { success = true, message = "Chỉnh sửa thành công!" });
                }

                return Json(new { success = false, message = "Không tìm thấy tài khoản!" });
            }
        }

        public ActionResult xoaacc(int id)
        {
            using (var db = new accgameEntities())
            {
                ViewBag.qldm = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }

                // Xử lý xóa tài khoản
                var accrd = db.AccRandoms.Find(id);
                if (accrd == null)
                {
                    return HttpNotFound();
                }

                int iddm = Convert.ToInt32(accrd.IDDanhMuc);
                var danhmuc = db.DanhMucs.Find(iddm);
                if (danhmuc != null)
                {
                    danhmuc.HienCo = Convert.ToInt32(danhmuc.HienCo) - 1;
                }
                accrd.Xoa = true;

                if (danhmuc.IDLoaiAcc != null)
                {
                    var loaiAcc = db.LoaiAccs.Find(danhmuc.IDLoaiAcc);
                    if (loaiAcc != null)
                    {
                        loaiAcc.DangBan = Convert.ToInt32(loaiAcc.DangBan) + 1;
                        db.SaveChanges();
                    }
                }
                db.SaveChanges();

                TempData["SuccessXoa"] = true;

                // Redirect về trang trước đó
                var referrerUrl = Request.UrlReferrer?.ToString();
                if (string.IsNullOrEmpty(referrerUrl))
                {
                    return RedirectToAction("Index");
                }

                return Redirect(referrerUrl);
            }
        }

    }

    public class loadDanhMuc
    {
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("data")]
        public List<dataLoadDanhMuc> data { get; set; }
    }
    public class dataLoadDanhMuc
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("product")]
        public string product { get; set; }
        [JsonProperty("type_category")]
        public string type_category { get; set; }
        [JsonProperty("sell")]
        public string sell { get; set; }
        [JsonProperty("detail")]
        public detailDanhMuc detail { get; set; }

    }
    public class detailDanhMuc
    {
        [JsonProperty("name_product")]
        public string name_product { get; set; }
        [JsonProperty("thumb")]
        public string thumb { get; set; }
        [JsonProperty("cash")]
        public string cash { get; set; }


        [JsonProperty("data")]
        public List<dataDetailDanhMuc> data { get; set; }

    }

    public class dataDetailDanhMuc
    {
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("value")]
        public string value { get; set; }


    }
}