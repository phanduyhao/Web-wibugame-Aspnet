using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using accgame.Models;
using accgame.Mahoa;
using ImageMagick;
using accgame.Filters;
using System.Collections;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    
    public class quanlytaikhoanController : Controller
    {
        accgameEntities db = new accgameEntities();
        public ActionResult Index(int? page, string sapxep, string searchMa, string searchTK, string searchND, int? loaigame, string theloaiacc)
        {
            int pageNumber = (page ?? 1);
            int newpage = pageNumber - 1;

            ViewBag.qltk = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }

            var query = db.Accs.Where(m => m.Xoa != true);
            var listLoaiGame = db.LoaiGames.OrderBy(m => m.STT).ToList();
            var GiaCtvCollabAccVip = db.CaiDats
                           .Where(m => m.MaCaiDat == "gia_collab_acc_vip")
                           .Select(m => m.NoiDung)
                           .FirstOrDefault();
            if (!string.IsNullOrEmpty(sapxep))
            {
                if (sapxep == "1")
                {
                    query = query.Where(m => m.DaBan == true);
                }
                if (sapxep == "0")
                {
                    query = query.Where(m => m.DaBan != true);
                }
            }

            if (!string.IsNullOrEmpty(searchMa))
            {
                query = query.Where(m => m.MaTaiKhoan.Contains(searchMa));
            }

            if (!string.IsNullOrEmpty(searchTK))
            {
                query = query.Where(m => m.TaiKhoan.Contains(searchTK));
            }

            if (!string.IsNullOrEmpty(searchND))
            {
                query = query.Where(m => m.NguoiDung.TenNguoiDung.Equals(searchND));
            }

            if (loaigame.HasValue)
            {
                query = query.Where(m => m.LoaiGame == loaigame.Value);
            }

            var data = query.Include(m => m.LoaiGame1)
                            .OrderByDescending(m => m.ThoiGianBan)
                            .ThenByDescending(m => m.IDAcc)
                            .Skip(newpage * 40)
                            .Take(40)
                            .ToList();

            decimal count = query.Count();
            var listAcc = db.LoaiAccs.Where(m => m.IDLoaiAccCha == null).OrderBy(m => m.STT).ToList();

            ViewBag.searchMa = searchMa;
            ViewBag.searchTK = searchTK;
            ViewBag.searchND = searchND;
            ViewBag.loaigame = loaigame;
            ViewBag.sapxep = sapxep;
            ViewBag.theloaiacc = theloaiacc;
            int last_page = Convert.ToInt16(Math.Ceiling(count / 40));
            ViewBag.last_page = last_page;
            ViewBag.page = pageNumber;
            ViewBag.listgame = listLoaiGame;
            ViewBag.loaiacc = listAcc;
            ViewBag.GiaCtvCollabAccVip = GiaCtvCollabAccVip;
            ViewBag.phanTramSaleTet = db.CaiDats.Where(m => m.MaCaiDat == "phan_tram_sale_tet").Select(m => m.NoiDung).FirstOrDefault();
            ViewBag.AnAccVip = db.AnAccs.Where(m => m.IdUser == idnd).Select(m => m.AnAccVip).FirstOrDefault();
            ViewBag.isSaleTet = db.CaiDats.Where(m => m.MaCaiDat == "is_sale_tet").Select(m => m.NoiDung).FirstOrDefault();
            {
                TempData["PreviousUrl"] = Request.UrlReferrer.ToString();
            }
            return View(data);
        }

        [HttpPost]
        public ActionResult UpdateThongBaoMuaAcc(string thongbaomuaacc)
        {
            using (var context = new accgameEntities())
            {
                var setting = context.CaiDats.FirstOrDefault(c => c.MaCaiDat == "info_buy_acc");
                if (setting != null)
                {
                    setting.NoiDung = thongbaomuaacc;
                    context.SaveChanges();
                    return Json(new { success = true, message = "Cập nhật thành công!" });

                }
            }
            return Json(new { success = false, message = "Cập nhật thất bại!" });
        }

        public ActionResult LoadSoLuongTaiKhoan(int id)
        {


            ViewBag.qltk = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //
            var loaiAcc = db.LoaiAccs.Find(id);
            if (loaiAcc == null)
            {
                TempData["notify"] = "Không tìm thấy loại tài khoản này!";
                return RedirectToAction("LoaiAcc", new { id = id });
            }
            var countAcc = db.Accs.Where(m => m.IDLoaiAcc == id && m.DaBan != true && m.Xoa != true && m.TaiKhoanNoiBo != true).Count();
            loaiAcc.DangBan = countAcc;
            TempData["notify"] = "Đã load lại số lượng tài khoản đang có, số lượng: " + countAcc;
            db.SaveChanges();
            return RedirectToAction("LoaiAcc", new { id = id });
        }

        public ActionResult RandomRR(int? page, string sapxep, string searchMa, string searchTK, string searchND, int? loaigame, string theloaiacc)
        {
            int pageNumber = (page ?? 1);
            int newpage = pageNumber - 1;

            ViewBag.qltk = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }

            // Khởi tạo query từ cơ sở dữ liệu với điều kiện Xoa != true
            var query = db.AccRandoms.Where(m => m.Xoa != true);
            var listLoaiGame = db.LoaiGames.OrderBy(m => m.STT).ToList();
            var thongbaomuaacc = db.CaiDats
                                     .Where(m => m.MaCaiDat == "info_buy_acc")
                                     .Select(m => m.NoiDung)
                                     .FirstOrDefault();
            if (!string.IsNullOrEmpty(sapxep))
            {
                if (sapxep == "1")
                {
                    query = query.Where(m => m.DaBan == true);
                }
                if (sapxep == "0")
                {
                    query = query.Where(m => m.DaBan != true);
                }
            }

            if (!string.IsNullOrEmpty(searchMa))
            {
                query = query.Where(m => m.IDAccRandom.ToString().Contains(searchMa));
            }

            if (!string.IsNullOrEmpty(searchTK))
            {
                query = query.Where(m => m.TaiKhoan.Contains(searchTK));
            }

            if (!string.IsNullOrEmpty(searchND))
            {
                query = query.Where(m => m.NguoiDung.TenNguoiDung.Equals(searchND));
            }

            if (loaigame.HasValue)
            {
                query = query.Include(m => m.DanhMuc)
                                    .Where(m => m.DanhMuc.LoaiGame == loaigame.Value);
            }

            // Order và phân trang
            var data = query.OrderByDescending(m => m.ThoiGianBan)
                            .ThenByDescending(m => m.IDAccRandom)
                            .Skip(newpage * 40)
                            .Take(40)
                            .ToList();

            decimal count = query.Count();
            var listAcc = db.LoaiAccs.Where(m => m.Hide != true).OrderBy(m => m.STT).ToList();

            // Thiết lập các ViewBag để giữ lại giá trị tìm kiếm cho giao diện người dùng
            ViewBag.searchMa = searchMa;
            ViewBag.searchTK = searchTK;
            ViewBag.searchND = searchND;
            ViewBag.loaigame = loaigame; // Thêm dòng này để lưu trữ giá trị loại game đã chọn
            ViewBag.sapxep = sapxep;
            int last_page = Convert.ToInt16(Math.Ceiling(count / 40));
            ViewBag.last_page = last_page;
            ViewBag.page = pageNumber;
            ViewBag.listgame = listLoaiGame;
            ViewBag.loaiacc = listAcc;
            ViewBag.theloaiacc = theloaiacc;
            ViewBag.thongbaomuacc = thongbaomuaacc;

            if (Request.UrlReferrer != null)
            {
                TempData["PreviousUrl"] = Request.UrlReferrer.ToString();
            }
            return View(data);
        }
      
        public ActionResult LoaiAcc(int? page, int id, string sapxep, string searchMa, string searchTK, string searchND)
        {
            int pageNumber = (page ?? 1);
            int newpage = pageNumber - 1;

            ViewBag.qltk = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }

            // Get TenAcc for the given id
            var tenAcc = db.Accs.Where(m => m.IDLoaiAcc == id).Select(m => m.TenAcc).FirstOrDefault();
            ViewBag.TenAcc = tenAcc;

            // Khởi tạo query từ cơ sở dữ liệu với điều kiện Xoa != true
            var query = db.Accs.Where(m => m.Xoa != true && m.IDLoaiAcc == id);

            if (!string.IsNullOrEmpty(sapxep))
            {
                if (sapxep == "1")
                {
                    query = query.Where(m => m.DaBan == true);
                }
                if (sapxep == "0")
                {
                    query = query.Where(m => m.DaBan != true);
                }
            }

            if (!string.IsNullOrEmpty(searchMa))
            {
                query = query.Where(m => m.MaTaiKhoan.Contains(searchMa));
            }

            if (!string.IsNullOrEmpty(searchTK))
            {
                query = query.Where(m => m.TaiKhoan.Contains(searchTK));
            }
            if (!string.IsNullOrEmpty(searchND))
            {
                query = query.Where(m => m.NguoiDung.TenNguoiDung.Equals(searchND));
            }

            // Order và phân trang
            var data = query.Include(m => m.LoaiGame1)
                            .OrderByDescending(m => m.ThoiGianBan)
                            .ThenByDescending(m => m.IDAcc)
                            .Skip(newpage * 40)
                            .Take(40)
                            .ToList();

            decimal count = query.Count();

            // Thiết lập các ViewBag để giữ lại giá trị tìm kiếm cho giao diện người dùng
            ViewBag.searchMa = searchMa;
            ViewBag.searchTK = searchTK;
            ViewBag.sapxep = sapxep;
            int last_page = Convert.ToInt16(Math.Ceiling(count / 40));
            ViewBag.last_page = last_page;
            ViewBag.page = pageNumber;
            ViewBag.idLoaiAcc = id;
            ViewBag.searchND = searchND;
            ViewBag.phanTramSaleTet = db.CaiDats.Where(m => m.MaCaiDat == "phan_tram_sale_tet").Select(m => m.NoiDung).FirstOrDefault();
            ViewBag.isSaleTet = db.CaiDats.Where(m => m.MaCaiDat == "is_sale_tet").Select(m => m.NoiDung).FirstOrDefault();
            if (Request.UrlReferrer != null)
            {
                TempData["PreviousUrl"] = Request.UrlReferrer.ToString();
            }
            return View(data);
        }

        public ActionResult LoaiAccDacBiet(int? page, int id, string sapxep, string searchMa, string searchTK)
        {
            int pageNumber = (page ?? 1);
            int newpage = pageNumber - 1;
            var loaiAcc = db.LoaiAccs.Where(m => m.IDLoaiAcc == id).Include(m => m.LoaiGame).FirstOrDefault();

            ViewBag.qltk = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            /*            var danhmuc = db.DanhMucs.Where(m => m.Xoa != true && m.IDLoaiAcc == id).OrderByDescending(m => m.LoaiGame).ThenBy(m => m.STT).ThenByDescending(m => m.IDDanhMuc).Include(m => m.NguoiDung).Include(m => m.LoaiGame1).Skip(newpage * 40).Take(40).ToList();
            */
            var danhmuc = db.DanhMucs.Where(m => m.IDLoaiAcc == id && m.Xoa != true).OrderBy(m => m.STT).ToList();
            var listAcc = db.LoaiAccs.Where(m => m.IDLoaiAccCha == id).OrderBy(m => m.STT).ToList();

            decimal count = db.DanhMucs.Count();
            ViewBag.page = pageNumber;
            ViewBag.tenLoai = loaiAcc.TenLoaiAcc;
            ViewBag.idDanhMuc = id;
            ViewBag.loaiacc = listAcc;

            if (Request.UrlReferrer != null)
            {
                TempData["PreviousUrl"] = Request.UrlReferrer.ToString();
            }
            return View(danhmuc);
        }

        [HttpPost]
        public JsonResult SuaGia(int danhMucId, int? Gia, int? GiaCtvCollab)
        {
            var danhMuc = db.DanhMucs.Find(danhMucId);

            // Chỉ cập nhật nếu Gia không phải null và khác với giá trị hiện tại
            if (Gia.HasValue && Gia != danhMuc.Gia)
            {
                danhMuc.Gia = Gia.Value;
            }

            // Chỉ cập nhật nếu GiaCtvCollab không phải null và khác với giá trị hiện tại
            if (GiaCtvCollab.HasValue && GiaCtvCollab != danhMuc.GiaCtvCollab)
            {
                danhMuc.GiaCtvCollab = GiaCtvCollab.Value;
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            db.SaveChanges();


            TempData["SuccessSua"] = "Cập nhật giá thành công!";

            return Json(new { success = true });
        }

        public ActionResult timkiem(string search)
        {
            ViewBag.qltk = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen

            var data = db.Accs.Where(m => m.RanDom != true && m.Xoa != true && m.TaiKhoanNoiBo != true && (m.MaTaiKhoan.Contains(search) || m.TaiKhoan.Contains(search) || m.NguoiDung.TenNguoiDung.Contains(search)) && m.DaBan != true && m.TaiKhoanNoiBo != true).OrderByDescending(m => m.IDAcc).Take(40).ToList();

            return View(data);
        }
        /*public ActionResult timkiem(string search, string searchTK)
        {
            ViewBag.qltk = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            // phan quyen

            var data = db.Accs.Where(m => m.RanDom != true
                                          && m.Xoa != true
                                          && m.TaiKhoanNoiBo != true
                                          && m.DaBan != true
                                          && (m.MaTaiKhoan.Contains(search)
                                              || m.TaiKhoan.Contains(search)
                                              || m.NguoiDung.TenNguoiDung.Contains(search)
                                              || m.TaiKhoan.Contains(searchTK)))
                              .OrderByDescending(m => m.IDAcc)
                              .Take(40)
                              .ToList();

            return View(data);
        }*/

        public ActionResult themmoi(int id)
        {
            ViewBag.qltk = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }

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

            // Dữ liệu khác
            HomeModels data = new HomeModels()
            {
                listNhanVat = db.NhanVats.Where(m => m.LoaiGame == 1).ToList(),
                listVuKhi = db.VuKhis.ToList(),
            };
            // Lưu URL trang trước đó vào TempData
            if (Request.UrlReferrer != null)
            {
                TempData["PreviousUrl"] = Request.UrlReferrer.ToString();
            }
            return View(data);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult themmoi([Bind(Include = "TenAcc, LoaiGame, Server, TaiKhoan, MatKhau, Gia, GiaGoc, ChiTiet, ThongTinKhac, CapKhaiPha, AccVip, AccKhoiDau, PhanTram, IDLoaiAcc, GiaCtvCollab")] Acc acc, int[] DSNhanVat, int[] DSVuKhi, HttpPostedFileBase ImageUpload, HttpPostedFileBase[] ImageUploads)
        {
            ViewBag.qltk = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            if (acc.Gia < 0)
            {
                acc.Gia = 0;
            }
            var GiaCtvCollabAccVip = db.CaiDats
               .Where(m => m.MaCaiDat == "gia_collab_acc_vip")
               .Select(m => m.NoiDung)
               .FirstOrDefault();
            var loaiGame = db.LoaiGames.Find(acc.LoaiGame);
            var searchAcc = db.Accs.Include(m => m.LoaiGame1).OrderByDescending(m => m.IDAcc).FirstOrDefault();
            if (searchAcc == null)
            {
                acc.MaTaiKhoan = Mahoa.MyString.vietTat(loaiGame.TenLoaiGame) + 1000;
            }
            else
            {
                int ma = 1000 + searchAcc.IDAcc + 1;
                acc.MaTaiKhoan = Mahoa.MyString.vietTat(loaiGame.TenLoaiGame) + ma;
            }
            acc.NhanVat = "";
            acc.VuKhi = "";
            string stringAnhNhanVat = "";
            string anhVuKhi = "";
            string tenVuKhi = "";
            string tenNhanVat = "";
            var nhanVat = db.NhanVats.ToList();
            var vuKhi = db.VuKhis.ToList();

            if (DSNhanVat != null)
            {
                foreach (var item in DSNhanVat)
                {
                    var nhanVatItem = nhanVat.FirstOrDefault(m => m.IDNhanVat == item);
                    if (nhanVatItem != null)
                    {
                        acc.NhanVat += " - " + nhanVatItem.TenNhanVat;
                        stringAnhNhanVat += nhanVatItem.Avatar + ", ";
                        tenNhanVat += nhanVatItem.TenNhanVat + ", ";
                    }
                }
                stringAnhNhanVat = stringAnhNhanVat.TrimEnd(',', ' ');
                tenNhanVat = tenNhanVat.TrimEnd(',', ' ');
            }
            if (DSVuKhi != null)
            {
                foreach (var item in DSVuKhi)
                {
                    var vuKhiItem = vuKhi.FirstOrDefault(m => m.IDVuKhi == item);
                    if (vuKhiItem != null)
                    {
                        acc.VuKhi += " - " + vuKhiItem.TenVuKhi;
                       /* if (vuKhiItem.Sao == 5)
                        {*/
                            anhVuKhi += vuKhiItem.Avatar + ", ";
                            tenVuKhi += vuKhiItem.TenVuKhi + ", ";
                        /*}*/
                    }
                }
                anhVuKhi = anhVuKhi.TrimEnd(',', ' ');
                tenVuKhi = tenVuKhi.TrimEnd(',', ' ');
            }
            var caidat = db.CaiDats;
            var isSaleTet = caidat.Where(m => m.MaCaiDat == "is_sale_tet").Select(m => m.NoiDung).FirstOrDefault();
            var phanTramSale = caidat.Where(m => m.MaCaiDat == "phan_tram_sale_tet").Select(m => m.NoiDung).FirstOrDefault();


            var filename = "Anhacc_" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmssfff")) + ".webp";
            var savePath = Path.Combine(Server.MapPath("/Content/images/") + filename);
            //ImageUpload.SaveAs(savePath);
            using (var memoryStream = new MemoryStream())
            {

                ImageUpload.InputStream.CopyTo(memoryStream);
                byte[] fileBytes = memoryStream.ToArray();

                using (var image = new MagickImage(fileBytes))
                {
                    if (image.Width > 1000)
                    {
                        int newWidth = 1000;
                        int newHeight = (int)(image.Height * (1000.0 / image.Width));
                        image.Resize(newWidth, newHeight);
                    }
                    image.Quality = 75;
                    image.Format = MagickFormat.WebP;
                    image.Write(savePath);
                }
            }
            acc.AnhDaiDien = "/Content/images/" + filename;


            acc.IDNguoiDung = idnd;
            acc.ThoiGianDang = DateTime.Now;

            acc.IDLoaiAcc = acc.IDLoaiAcc;

            acc.AnhNhanVat = stringAnhNhanVat;
            acc.AnhVuKhi = anhVuKhi;
            acc.TenNhanVat = tenNhanVat;
            acc.TenVuKhi = tenVuKhi;
            acc.GiaCtvCollab = Convert.ToInt32(GiaCtvCollabAccVip);
            LoaiAcc loaiAcc = db.LoaiAccs.Find(acc.IDLoaiAcc);
            if (loaiAcc != null)
            {
                loaiAcc.DangBan += 1;
            }
            int phanTram = acc.PhanTram ?? 0;
            acc.PhanTram = phanTram;
            if (phanTramSale != null && isSaleTet == "1")
            {
                acc.Gia = acc.GiaGoc - (acc.GiaGoc * (Convert.ToInt32(phanTramSale) + phanTram) / 100);
            }
            else
            {
                acc.Gia = acc.GiaGoc - (acc.GiaGoc * phanTram / 100);
            }

            var checkAnAcc = db.AnAccs.Any(m => m.IdUser == idnd && m.AnAccVip == true);

            if (checkAnAcc)
            {
                acc.An = true; // Nếu điều kiện đúng, đặt cột "An" thành true
            }
            db.Accs.Add(acc);
            db.SaveChanges();
            var upAnhAcc = new Anh_Acc();
            upAnhAcc.AnhDaiDien = true;
            upAnhAcc.DuongDanAnh = "/Content/images/" + filename;
            upAnhAcc.IDNguoiDung = idnd;
            upAnhAcc.IDAcc = acc.IDAcc;
            upAnhAcc.ThoiGian = DateTime.Now;
            db.Anh_Acc.Add(upAnhAcc);
            int i = 0;
            foreach (HttpPostedFileBase imgUL in ImageUploads)
            {
                if (imgUL != null)
                {
                    Anh_Acc anhAcc = new Anh_Acc();
                    var fn = "Anhacc_" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmssfff")) + i + ".webp";
                    var SP = Path.Combine(Server.MapPath("/Content/images/") + fn);
                    //imgUL.SaveAs(SP);
                    //ImageUpload.SaveAs(savePath);
                    using (var memoryStream = new MemoryStream())
                    {
                        imgUL.InputStream.CopyTo(memoryStream);
                        byte[] fileBytesF = memoryStream.ToArray();

                        using (var image = new MagickImage(fileBytesF))
                        {
                            if (image.Width > 1000)
                            {
                                int newWidth = 1000;
                                int newHeight = (int)(image.Height * (1000.0 / image.Width));
                                image.Resize(newWidth, newHeight);
                            }
                            image.Quality = 75;
                            image.Format = MagickFormat.WebP;
                            image.Write(SP);
                        }
                    }
                    anhAcc.DuongDanAnh = "/Content/images/" + fn;
                    anhAcc.IDAcc = acc.IDAcc;
                    anhAcc.IDNguoiDung = idnd;
                    anhAcc.ThoiGian = DateTime.Now;
                    db.Anh_Acc.Add(anhAcc);
                }
                i++;
            }
            db.SaveChanges();
            TempData["SuccessThem"] = true;

            string previousUrl = TempData["PreviousUrl"] as string;
            if (!string.IsNullOrEmpty(previousUrl))
            {
                return Redirect(previousUrl);
            }
            return RedirectToAction("index");
        }

        [HttpPost]
        public JsonResult XoaAnh(int ID)
        {
            var anh = db.Anh_Acc.Find(ID); // Tìm ảnh trong database
            if (anh != null)
            {
                db.Anh_Acc.Remove(anh);  // Xóa ảnh khỏi database
                db.SaveChanges();         // Lưu thay đổi
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public ActionResult sua(int id)
        {
            ViewBag.qltk = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            HomeModels data = new HomeModels()
            {
                listNhanVat = db.NhanVats.ToList(),
                listVuKhi = db.VuKhis.ToList(),
                acc = db.Accs.Where(m => m.IDAcc == id).FirstOrDefault(),
                listAnh_Acc = db.Anh_Acc.Where(m => m.IDAcc == id && m.AnhDaiDien != true).ToList(),
                listLoaiGame = db.LoaiGames.Where(m => m.Hide != true).ToList(),
                listLoaiAcc = db.LoaiAccs.Where(m => m.Hide != true && m.DacBiet != true).ToList(),
            };
            if (Request.UrlReferrer != null)
            {
                TempData["PreviousUrl"] = Request.UrlReferrer.ToString();
            }
            return View(data);
        }
/*

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult sua(int id, [Bind(Include = "TenAcc, LoaiGame, Server, TaiKhoan, MatKhau, Gia, GiaGoc, DaBan, ChiTiet, ThongTinKhac, CapKhaiPha, AccVip, AccKhoiDau, PhanTram, IDLoaiAcc, GiaCtvCollab")] Acc acc, int[] DSNhanVat, int[] DSVuKhi, HttpPostedFileBase ImageUpload, HttpPostedFileBase[] ImageUploads)
        {
            ViewBag.qltk = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            if (acc.Gia < 0)
            {
                acc.Gia = 0;
            }
            var setAcc = db.Accs.Find(id);
            LoaiAcc loaiAc = db.LoaiAccs.Find(setAcc.IDLoaiAcc);
            if (setAcc.IDLoaiAcc != acc.IDLoaiAcc)
            {
                    if (loaiAc != null)
                    {
                        loaiAc.DangBan -= 1;
                    }
                    var newLoaiAcc = db.LoaiAccs.Where(m => m.IDLoaiAcc == acc.IDLoaiAcc).FirstOrDefault();
                    LoaiAcc setNewLoaiAcc = db.LoaiAccs.Find(newLoaiAcc.IDLoaiAcc);
                    if (setNewLoaiAcc != null)
                    {
                        setNewLoaiAcc.DangBan += 1;
                    }

                db.SaveChanges();
            }
            acc.NhanVat = "";
            acc.VuKhi = "";
            string stringAnhNhanVat = "";
            string anhVuKhi = "";
            string tenVuKhi = "";
            string tenNhanVat = "";
            var nhanVat = db.NhanVats.ToList();
            var vuKhi = db.VuKhis.ToList();

            if (DSNhanVat != null)
            {
                foreach (var item in DSNhanVat)
                {
                    var nhanVatItem = nhanVat.FirstOrDefault(m => m.IDNhanVat == item);
                    if (nhanVatItem != null)
                    {
                        acc.NhanVat += " - " + nhanVatItem.TenNhanVat;
                        stringAnhNhanVat += nhanVatItem.Avatar + ", ";
                        tenNhanVat += nhanVatItem.TenNhanVat + ", ";
                    }
                }
                stringAnhNhanVat = stringAnhNhanVat.TrimEnd(',', ' ');
                tenNhanVat = tenNhanVat.TrimEnd(',', ' ');
            }
            if (DSVuKhi != null)
            {
                foreach (var item in DSVuKhi)
                {
                    var vuKhiItem = vuKhi.FirstOrDefault(m => m.IDVuKhi == item);
                    if (vuKhiItem != null)
                    {
                        acc.VuKhi += " - " + vuKhiItem.TenVuKhi;
                        if (vuKhiItem.Sao == 5)
                        {
                            anhVuKhi += vuKhiItem.Avatar + ", ";
                            tenVuKhi += vuKhiItem.TenVuKhi + ", ";
                        }
                    }
                }
                anhVuKhi = anhVuKhi.TrimEnd(',', ' ');
                tenVuKhi = tenVuKhi.TrimEnd(',', ' ');
            }
            var caidat = db.CaiDats;

            var isSaleTet = caidat.Where(m => m.MaCaiDat == "is_sale_tet").Select(m => m.NoiDung).FirstOrDefault();
            var phanTramSale = caidat.Where(m => m.MaCaiDat == "phan_tram_sale_tet").Select(m => m.NoiDung).FirstOrDefault();
            var GiaCtvCollabAccVip = db.CaiDats
                        .Where(m => m.MaCaiDat == "gia_collab_acc_vip")
                        .Select(m => m.NoiDung)
                        .FirstOrDefault();
            setAcc.TenAcc = acc.TenAcc;
            setAcc.LoaiGame = acc.LoaiGame;
            setAcc.Server = acc.Server;
            setAcc.TaiKhoan = acc.TaiKhoan;
            setAcc.MatKhau = acc.MatKhau;
            setAcc.GiaGoc = acc.GiaGoc;
            int phanTram = acc.PhanTram ?? 0;
            setAcc.PhanTram = phanTram;

            if (phanTramSale != null && isSaleTet == "1")
            {
                setAcc.Gia = acc.GiaGoc - (acc.GiaGoc * (Convert.ToInt32(phanTramSale) + phanTram) / 100);
            }
            else
            {
                setAcc.Gia = acc.GiaGoc - (acc.GiaGoc * phanTram / 100);
            }
            setAcc.GiaCtvCollab = acc.GiaCtvCollab;
            setAcc.ChiTiet = acc.ChiTiet;
            setAcc.ThongTinKhac = acc.ThongTinKhac;
            setAcc.CapKhaiPha = acc.CapKhaiPha;
            setAcc.DaBan = acc.DaBan;
            setAcc.IDLoaiAcc = acc.IDLoaiAcc;
            setAcc.PhanTram = acc.PhanTram;
            setAcc.GiaCtvCollab = Convert.ToInt32(GiaCtvCollabAccVip);
            
            if (ImageUpload != null)
            {
                var filename = "Anhacc_" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmssfff")) + ".webp";
                var savePath = Path.Combine(Server.MapPath("/Content/images/") + filename);
                //ImageUpload.SaveAs(savePath);
                using (var memoryStream = new MemoryStream())
                {

                    ImageUpload.InputStream.CopyTo(memoryStream);
                    byte[] fileBytes = memoryStream.ToArray();

                    using (var image = new MagickImage(fileBytes))
                    {
                        if (image.Width > 1000)
                        {
                            int newWidth = 1000;
                            int newHeight = (int)(image.Height * (1000.0 / image.Width));
                            image.Resize(newWidth, newHeight);
                        }
                        image.Quality = 75;
                        image.Format = MagickFormat.WebP;
                        image.Write(savePath);
                    }
                }
                setAcc.AnhDaiDien = "/Content/images/" + filename;
                var checkAnhAcc = db.Anh_Acc.Where(m => m.IDAcc == setAcc.IDAcc && m.AnhDaiDien == true).FirstOrDefault();
                if (checkAnhAcc != null)
                {
                    string path = Server.MapPath(checkAnhAcc.DuongDanAnh);
                    if (System.IO.File.Exists(path))
                    {
                        try
                        {
                            System.IO.File.Delete(path);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    var upAnhAcc = db.Anh_Acc.Find(checkAnhAcc.IDAnh_Acc);
                    upAnhAcc.AnhDaiDien = true;
                    upAnhAcc.DuongDanAnh = "/Content/images/" + filename;
                    upAnhAcc.IDNguoiDung = idnd;
                    upAnhAcc.IDAcc = setAcc.IDAcc;

                }
                db.SaveChanges();
            }

            int i = 0;
            foreach (HttpPostedFileBase imgUL in ImageUploads)
            {
                if (imgUL != null)
                {
                    Anh_Acc anhAcc = new Anh_Acc();
                    var fn = "Anhacc_" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmssfff")) + i + ".webp";
                    var SP = Path.Combine(Server.MapPath("/Content/images/") + fn);
                    //imgUL.SaveAs(SP);
                    //ImageUpload.SaveAs(savePath);
                    using (var memoryStream = new MemoryStream())
                    {

                        imgUL.InputStream.CopyTo(memoryStream);
                        byte[] fileBytesF = memoryStream.ToArray();

                        using (var image = new MagickImage(fileBytesF))
                        {
                            if (image.Width > 1000)
                            {
                                int newWidth = 1000;
                                int newHeight = (int)(image.Height * (1000.0 / image.Width));
                                image.Resize(newWidth, newHeight);
                            }
                            image.Quality = 75;
                            image.Format = MagickFormat.WebP;
                            image.Write(SP);
                        }
                    }
                    anhAcc.DuongDanAnh = "/Content/images/" + fn;
                    anhAcc.IDAcc = setAcc.IDAcc;
                    anhAcc.IDNguoiDung = idnd;
                    anhAcc.ThoiGian = DateTime.Now;
                    db.Anh_Acc.Add(anhAcc);
                }
                i++;
            }
            db.SaveChanges();
            TempData["SuccessSua"] = true;

            string previousUrl = TempData["PreviousUrl"] as string;
            if (!string.IsNullOrEmpty(previousUrl))
            {
                return Redirect(previousUrl);
            }
            return RedirectToAction("index");
        }
*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult sua2(int id, [Bind(Include = "TenAcc, LoaiGame, Server, TaiKhoan, MatKhau, Gia, GiaGoc,DaBan, ChiTiet, ThongTinKhac, CapKhaiPha, AccVip, AccKhoiDau, PhanTram, IDLoaiAcc, GiaCtvCollab")] Acc acc, int[] DSNhanVat, int[] DSVuKhi, HttpPostedFileBase ImageUpload, HttpPostedFileBase[] ImageUploads)
        {
            ViewBag.qltk = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);

            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }

            var accDb = db.Accs.Find(id);
            if (accDb == null)
            {
                return HttpNotFound();
            }
            var caidat = db.CaiDats;

            var isSaleTet = caidat.Where(m => m.MaCaiDat == "is_sale_tet").Select(m => m.NoiDung).FirstOrDefault();
            var phanTramSale = caidat.Where(m => m.MaCaiDat == "phan_tram_sale_tet").Select(m => m.NoiDung).FirstOrDefault();
            var GiaCtvCollabAccVip = db.CaiDats
                        .Where(m => m.MaCaiDat == "gia_collab_acc_vip")
                        .Select(m => m.NoiDung)
                        .FirstOrDefault();
            // Cập nhật thông tin tài khoản
            accDb.TenAcc = acc.TenAcc;
            accDb.LoaiGame = acc.LoaiGame;
            accDb.Server = acc.Server;
            accDb.TaiKhoan = acc.TaiKhoan;
            accDb.MatKhau = acc.MatKhau;
            /*       accDb.Gia = acc.Gia < 0 ? 0 : acc.Gia;
                   accDb.GiaGoc = acc.GiaGoc;*/
            accDb.GiaGoc = acc.GiaGoc;
            accDb.ChiTiet = acc.ChiTiet;
            accDb.ThongTinKhac = acc.ThongTinKhac;
            accDb.CapKhaiPha = acc.CapKhaiPha;
            accDb.AccVip = acc.AccVip;
            accDb.AccKhoiDau = acc.AccKhoiDau;
            accDb.PhanTram = acc.PhanTram ?? 0;
            accDb.IDLoaiAcc = acc.IDLoaiAcc;
            accDb.DaBan = acc.DaBan;
            accDb.GiaCtvCollab = Convert.ToInt32(GiaCtvCollabAccVip);

            int phanTram = acc.PhanTram ?? 0;
            accDb.PhanTram = phanTram;

            if (phanTramSale != null && isSaleTet == "1")
            {
                accDb.Gia = acc.GiaGoc - (acc.GiaGoc * (Convert.ToInt32(phanTramSale) + phanTram) / 100);
            }
            else
            {
                accDb.Gia = acc.GiaGoc - (acc.GiaGoc * phanTram / 100);
            }
            // Xử lý nhân vật
            accDb.NhanVat = "";
            accDb.AnhNhanVat = "";
            accDb.TenNhanVat = "";

            if (DSNhanVat != null)
            {
                var nhanVat = db.NhanVats.Where(m => DSNhanVat.Contains(m.IDNhanVat)).ToList();
                accDb.NhanVat = string.Join(" - ", nhanVat.Select(m => m.TenNhanVat));
                accDb.AnhNhanVat = string.Join(", ", nhanVat.Select(m => m.Avatar));
                accDb.TenNhanVat = string.Join(", ", nhanVat.Select(m => m.TenNhanVat));
            }

            // Xử lý vũ khí
            accDb.VuKhi = "";
            accDb.AnhVuKhi = "";
            accDb.TenVuKhi = "";

            if (DSVuKhi != null)
            {
                var vuKhi = db.VuKhis.Where(m => DSVuKhi.Contains(m.IDVuKhi)).ToList();
                accDb.VuKhi = string.Join(" - ", vuKhi.Select(m => m.TenVuKhi));
                accDb.AnhVuKhi = string.Join(", ", vuKhi.Select(v => v.Avatar));
                accDb.TenVuKhi = string.Join(", ", vuKhi.Select(v => v.TenVuKhi));
            }

            // Xử lý hình ảnh đại diện
            if (ImageUpload != null)
            {
                var filename = "Anhacc_" + DateTime.Now.ToString("yyyyMMddhhmmssfff") + ".webp";
                var savePath = Path.Combine(Server.MapPath("/Content/images/") + filename);

                using (var memoryStream = new MemoryStream())
                {
                    ImageUpload.InputStream.CopyTo(memoryStream);
                    byte[] fileBytes = memoryStream.ToArray();

                    using (var image = new MagickImage(fileBytes))
                    {
                        if (image.Width > 1000)
                        {
                            image.Resize(1000, (int)(image.Height * (1000.0 / image.Width)));
                        }
                        image.Quality = 75;
                        image.Format = MagickFormat.WebP;
                        image.Write(savePath);
                    }
                }
                accDb.AnhDaiDien = "/Content/images/" + filename;
            }

            // Cập nhật thời gian sửa
            accDb.ThoiGianDang = DateTime.Now;

            // Lưu vào database
            db.Entry(accDb).State = EntityState.Modified;
            db.SaveChanges();

            int i = 0;
            foreach (HttpPostedFileBase imgUL in ImageUploads)
            {
                if (imgUL != null)
                {
                    Anh_Acc anhAcc = new Anh_Acc();
                    var fn = "Anhacc_" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmssfff")) + i + ".webp";
                    var SP = Path.Combine(Server.MapPath("/Content/images/") + fn);
                    //imgUL.SaveAs(SP);
                    //ImageUpload.SaveAs(savePath);
                    using (var memoryStream = new MemoryStream())
                    {

                        imgUL.InputStream.CopyTo(memoryStream);
                        byte[] fileBytesF = memoryStream.ToArray();

                        using (var image = new MagickImage(fileBytesF))
                        {
                            if (image.Width > 1000)
                            {
                                int newWidth = 1000;
                                int newHeight = (int)(image.Height * (1000.0 / image.Width));
                                image.Resize(newWidth, newHeight);
                            }
                            image.Quality = 75;
                            image.Format = MagickFormat.WebP;
                            image.Write(SP);
                        }
                    }
                    anhAcc.DuongDanAnh = "/Content/images/" + fn;
                    anhAcc.IDAcc = accDb.IDAcc;
                    anhAcc.IDNguoiDung = idnd;
                    anhAcc.ThoiGian = DateTime.Now;
                    db.Anh_Acc.Add(anhAcc);
                }
                i++;
            }
            db.SaveChanges();

            TempData["SuccessSua"] = true;

            string previousUrl = TempData["PreviousUrl"] as string;
            if (!string.IsNullOrEmpty(previousUrl))
            {
                return Redirect(previousUrl);
            }
            return RedirectToAction("index");
        }


        public ActionResult taikhoandaban(int? page, string search)
        {
            int pageNumber = (page ?? 1);
            int newpage = pageNumber - 1;

            ViewBag.qltk = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen

            var data = db.Accs.Where(m => m.Xoa != true && m.DaBan == true).Include(m => m.LoaiGame1).OrderByDescending(m => m.ThoiGianBan).ThenByDescending(m => m.IDAcc).Skip(newpage * 40).Take(40).ToList();
            decimal count = db.Accs.Where(m => m.Xoa != true && m.DaBan == true).Count();
            if (search != null)
            {
                data = db.Accs.Where(m => m.Xoa != true && m.DaBan == true && (m.TenAcc.Contains(search) || m.MaTaiKhoan.Contains(search) || m.TaiKhoan.Contains(search))).Include(m => m.LoaiGame1).OrderByDescending(m => m.ThoiGianBan).ThenByDescending(m => m.IDAcc).Skip(newpage * 40).Take(40).ToList();
                count = db.Accs.Where(m => m.Xoa != true && m.DaBan == true && (m.TenAcc.Contains(search) || m.MaTaiKhoan.Contains(search) || m.TaiKhoan.Contains(search))).Count();
            }
            ViewBag.search = search;
            int last_page = Convert.ToInt16(Math.Ceiling(count / 40));
            ViewBag.last_page = last_page;
            ViewBag.page = pageNumber;
            return View(data);
        }

        public ActionResult xoa(int id)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            var acc = db.Accs.Find(id);
            acc.Xoa = true;
            LoaiAcc loaiAcc = db.LoaiAccs.Find(acc.IDLoaiAcc);
            if (loaiAcc != null)
            {
                loaiAcc.DangBan -= 1;
            }

            db.SaveChanges();
            TempData["SuccessXoa"] = true;
            var referrerUrl = Request.UrlReferrer?.ToString();
            if (string.IsNullOrEmpty(referrerUrl))
            {
                return RedirectToAction("Index");
            }

            return Redirect(referrerUrl);
        }

        public ActionResult LamMoiTaiKhoanThuong(int id)
        {
            var oldAcc = db.Accs.FirstOrDefault(m => m.IDAcc == id);
            if (oldAcc == null)
            {
                return HttpNotFound();
            }

            var relatedAnhAccs = db.Anh_Acc.Where(a => a.IDAcc == id);
            db.Anh_Acc.RemoveRange(relatedAnhAccs);
            db.SaveChanges(); 

            var newAcc = new Acc
            {
                TenAcc = oldAcc.TenAcc,
                Server = oldAcc.Server,
                MaTaiKhoan = oldAcc.MaTaiKhoan,
                ChiTiet = oldAcc.ChiTiet,
                Gia = oldAcc.Gia,
                PhanTram = oldAcc.PhanTram,
                IDNguoiDung = oldAcc.IDNguoiDung,
                IDNguoiMua = oldAcc.IDNguoiMua,
                ThoiGianDang = DateTime.Now, 
                ThoiGianBan = null,
                DaBan = false, 
                Xoa = oldAcc.Xoa,
                AnhDaiDien = oldAcc.AnhDaiDien,
                NhanVat = oldAcc.NhanVat,
                VuKhi = oldAcc.VuKhi,
                LoaiGame = oldAcc.LoaiGame,
                TaiKhoan = oldAcc.TaiKhoan,
                MatKhau = oldAcc.MatKhau,
                ThongTinKhac = oldAcc.ThongTinKhac,
                CapKhaiPha = oldAcc.CapKhaiPha,
                TaiKhoanNoiBo = oldAcc.TaiKhoanNoiBo,
                AccVip = oldAcc.AccVip,
                AccKhoiDau = oldAcc.AccKhoiDau,
                RanDom = oldAcc.RanDom,
                MaGiamGia = oldAcc.MaGiamGia,
                SoTienGiam = oldAcc.SoTienGiam,
                AnhNhanVat = oldAcc.AnhNhanVat,
                TenNhanVat = oldAcc.TenNhanVat,
                AnhVuKhi = oldAcc.AnhVuKhi,
                TenVuKhi = oldAcc.TenVuKhi,
                IDLoaiAcc = oldAcc.IDLoaiAcc,
                IDOldAcc = oldAcc.IDOldAcc,
                GiaCtvCollab = oldAcc.GiaCtvCollab,
                GiaGoc = oldAcc.GiaGoc,
                GiaDaMua = null
            };
            var loaiAcc = db.LoaiAccs.Find(oldAcc.IDLoaiAcc);
            if (oldAcc.DaBan == true)
            {
                loaiAcc.DangBan = Convert.ToInt32(loaiAcc.DangBan) + 1;
            }
            db.Accs.Add(newAcc);
            db.SaveChanges();

            db.Accs.Remove(oldAcc);
            db.SaveChanges();
            TempData["SuccessLamMoi"] = true;

            var referrerUrl = Request.UrlReferrer?.ToString();
            if (string.IsNullOrEmpty(referrerUrl))
            {
                return RedirectToAction("Index");
            }

            return Redirect(referrerUrl);
        }

        public ActionResult LamMoiTaiKhoanRR(int id)
        {
            var oldAcc = db.AccRandoms.FirstOrDefault(m => m.IDAccRandom == id);
            if (oldAcc == null)
            {
                return HttpNotFound();
            }

            var newAcc = new AccRandom
            {
                IDDanhMuc = oldAcc.IDDanhMuc,
                MatKhau = oldAcc.MatKhau,
                TaiKhoan = oldAcc.TaiKhoan,
                MatKhauOutlook = oldAcc.MatKhauOutlook,
                MailKhoiPhuc = oldAcc.MailKhoiPhuc,
                MatKhauMailKhoiPhuc = oldAcc.MatKhauMailKhoiPhuc,
                MatKhauMail = oldAcc.MatKhauMail,
                ThongTinKhac = oldAcc.ThongTinKhac,
                IDNguoiDung = oldAcc.IDNguoiDung,
                Xoa = oldAcc.Xoa,
                DaBan = false,
                IDNguoiMua = null,
                ThoiGianBan = null,
                IDOldAcc = oldAcc.IDOldAcc,
                GiaDaMua = null
            };
            var danhmuc = db.DanhMucs.Find(oldAcc.IDDanhMuc);
            var loaiAcc = db.LoaiAccs.Find(danhmuc.IDLoaiAcc);
            if(oldAcc.DaBan == true)
            {
                danhmuc.HienCo = Convert.ToInt32(danhmuc.HienCo) + 1;
                loaiAcc.DangBan = Convert.ToInt32(loaiAcc.DangBan) + 1;
            }
            db.AccRandoms.Add(newAcc);
            db.SaveChanges();

            db.AccRandoms.Remove(oldAcc);
            db.SaveChanges();
            TempData["SuccessLamMoi"] = true;

            var referrerUrl = Request.UrlReferrer?.ToString();
            if (string.IsNullOrEmpty(referrerUrl))
            {
                return RedirectToAction("Index");
            }

            return Redirect(referrerUrl);
        }

        [HttpPost]
        public ActionResult UpdateAllGiaCtvCollab(string GiaCtvCollab)
        {
            using (var context = new accgameEntities())
            {
                var allAccounts = context.Accs.ToList();
                foreach (var account in allAccounts)
                {
                    account.GiaCtvCollab = Convert.ToInt32(GiaCtvCollab);
                }
                var setting = context.CaiDats.FirstOrDefault(c => c.MaCaiDat == "gia_collab_acc_vip");
                if (setting != null)
                {
                    setting.NoiDung = GiaCtvCollab;
                    context.SaveChanges();
                    return Json(new { success = true, message = "Cập nhật thành công!" });

                }
            }
            return Json(new { success = false, message = "Cập nhật thất bại!" });
        }

        [HttpPost]
        public JsonResult suaSTT(int danhMucId, int? STT)
        {
            var danhMuc = db.DanhMucs.Find(danhMucId);

            // Chỉ cập nhật nếu Gia không phải null và khác với giá trị hiện tại
            if (STT.HasValue && STT != danhMuc.Gia)
            {
                danhMuc.STT = STT.Value;
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            db.SaveChanges();


            TempData["SuccessSua"] = "Cập nhật giá thành công!";

            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult AnAccVip(bool AnAccVip)
        {
            int IdUser = Convert.ToInt32(Session["IDNguoiDung"]);

            using (var db = new accgameEntities()) // Thay thế bằng DbContext thực tế
            {
                // Kiểm tra xem đã có bản ghi AnAcc với IdUser chưa
                var anAcc = db.AnAccs.FirstOrDefault(a => a.IdUser == IdUser);

                if (anAcc == null)
                {
                    // Nếu chưa có, thêm mới
                    anAcc = new AnAcc
                    {
                        IdUser = IdUser,
                        AnAccVip = AnAccVip
                    };
                    db.AnAccs.Add(anAcc);
                }
                else
                {
                    // Nếu đã có, cập nhật giá trị AnAccVip
                    anAcc.AnAccVip = AnAccVip;
                }

                // Cập nhật trạng thái "An" trong bảng Acc
                var accList = db.Accs.Where(a => a.IDNguoiDung == IdUser).ToList();
                foreach (var acc in accList)
                {
                    acc.An = AnAccVip; // Nếu AnAccVip = true, thì An = true
                }

                db.SaveChanges();
            }

            return Json(new { success = true });
        }

    }
}