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
    
    public class taikhoancuatoiController : Controller
    {
        accgameEntities db = new accgameEntities();
        // GET: quantri/taikhoancuatoi
        public ActionResult Index(int? page, string sapxep, string searchMa, string searchTK, int? loaigame, string theloaiacc)
        {
            int pageNumber = (page ?? 1);
            int newpage = pageNumber - 1;

            ViewBag.qltk = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1 && level != 2) || idnd == 0)
            {
                return Redirect("/");
            }

            var query = db.Accs.Where(m => m.Xoa != true);
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
                query = query.Where(m => m.MaTaiKhoan.Contains(searchMa));
            }

            if (!string.IsNullOrEmpty(searchTK))
            {
                query = query.Where(m => m.TaiKhoan.Contains(searchTK));
            }

            if (loaigame.HasValue)
            {
                query = query.Where(m => m.LoaiGame == loaigame.Value);
            }

            var data = query.Where(m => m.IDNguoiDung == idnd)
                            .Include(m => m.LoaiGame1)
                            .OrderByDescending(m => m.ThoiGianBan)
                            .ThenByDescending(m => m.IDAcc)
                            .Skip(newpage * 40)
                            .Take(40)
                            .ToList();

            decimal count = query.Count();
            var listAcc = db.LoaiAccs.Where(m => m.Hide != true).OrderBy(m => m.STT).ToList();

            ViewBag.searchMa = searchMa;
            ViewBag.searchTK = searchTK;
            ViewBag.loaigame = loaigame;
            ViewBag.sapxep = sapxep;
            ViewBag.theloaiacc = theloaiacc;
            int last_page = Convert.ToInt16(Math.Ceiling(count / 40));
            ViewBag.last_page = last_page;
            ViewBag.page = pageNumber;
            ViewBag.listgame = listLoaiGame;
            ViewBag.loaiacc = listAcc;
            ViewBag.thongbaomuacc = thongbaomuaacc;
            ViewBag.phanTramSaleTet = db.CaiDats.Where(m => m.MaCaiDat == "phan_tram_sale_tet").Select(m => m.NoiDung).FirstOrDefault();
            ViewBag.isSaleTet = db.CaiDats.Where(m => m.MaCaiDat == "is_sale_tet").Select(m => m.NoiDung).FirstOrDefault();
            ViewBag.AnAccVip = db.AnAccs.Where(m => m.IdUser == idnd).Select(m => m.AnAccVip).FirstOrDefault();
            ViewBag.IsAdminAn = db.AnAccs.Where(m => m.IdUser == idnd).Select(m => m.IsAdminAn).FirstOrDefault();

            if (Request.UrlReferrer != null)
            {
                TempData["PreviousUrl"] = Request.UrlReferrer.ToString();
            }
            return View(data);
        }

        public ActionResult RandomRR(int? page, string sapxep, string searchMa, string searchTK, int? loaigame, string theloaiacc)
        {
            int pageNumber = (page ?? 1);
            int newpage = pageNumber - 1;

            ViewBag.qltk = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1 && level != 2) || idnd == 0)
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

            if (loaigame.HasValue)
            {
                query = query.Include(m => m.DanhMuc)
                                    .Where(m => m.DanhMuc.LoaiGame == loaigame.Value);
            }

            // Order và phân trang
            var data = query.Where(m => m.IDNguoiDung == idnd)
                            .OrderByDescending(m => m.ThoiGianBan)
                            .ThenByDescending(m => m.IDAccRandom)
                            .Skip(newpage * 40)
                            .Take(40)
                            .ToList();

            decimal count = query.Count();
            var listAcc = db.LoaiAccs.Where(m => m.Hide != true).OrderBy(m => m.STT).ToList();

            // Thiết lập các ViewBag để giữ lại giá trị tìm kiếm cho giao diện người dùng
            ViewBag.searchMa = searchMa;
            ViewBag.searchTK = searchTK;
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

        public ActionResult LoaiAcc(int? page, int id, string sapxep, string searchMa, string searchTK)
        {
            int pageNumber = (page ?? 1);
            int newpage = pageNumber - 1;

            ViewBag.qltk = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1 && level != 2) || idnd == 0)
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

            // Order và phân trang
            var data = query.Where(m => m.IDNguoiDung == idnd).Include(m => m.LoaiGame1)
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
            var loaiAcc = db.LoaiAccs.Where(m => m.IDLoaiAcc == id).Include(m => m.LoaiGame).Where(m => m.Hide != true).FirstOrDefault();

            ViewBag.qltk = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1 && level != 2) || idnd == 0)
            {
                return Redirect("/");
            }
            /*            var danhmuc = db.DanhMucs.Where(m => m.Xoa != true && m.IDLoaiAcc == id).OrderByDescending(m => m.LoaiGame).ThenBy(m => m.STT).ThenByDescending(m => m.IDDanhMuc).Include(m => m.NguoiDung).Include(m => m.LoaiGame1).Skip(newpage * 40).Take(40).ToList();
            */
            var danhmuc = db.DanhMucs.Where(m => m.IDLoaiAcc == id && m.Xoa != true && m.IDNguoiDung == idnd).OrderByDescending(m => m.IDDanhMuc).ToList();

            decimal count = db.DanhMucs.Where(m => m.Xoa != true).Count();
            ViewBag.page = pageNumber;
            ViewBag.tenLoai = loaiAcc.TenLoaiAcc;
            ViewBag.idDanhMuc = id;
            if (Request.UrlReferrer != null)
            {
                TempData["PreviousUrl"] = Request.UrlReferrer.ToString();
            }
            return View(danhmuc);
        }


        public ActionResult timkiem(string search)
        {
            ViewBag.tkct = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1 && level != 2) || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen

            var data = db.Accs.Where(m => m.RanDom != true && m.IDNguoiDung == idnd && m.Xoa != true && m.TaiKhoanNoiBo != true && (m.MaTaiKhoan.Contains(search) || m.TaiKhoan.Contains(search) || m.NguoiDung.TenNguoiDung.Contains(search)) && m.DaBan != true && m.TaiKhoanNoiBo != true).OrderByDescending(m => m.IDAcc).Take(40).ToList();

            return View(data);
        }
        public ActionResult themmoi(int id)
        {
            ViewBag.qltk = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1 && level != 2) || idnd == 0)
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
            if (Request.UrlReferrer != null)
            {
                TempData["PreviousUrl"] = Request.UrlReferrer.ToString();
            }
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult themmoi([Bind(Include = "TenAcc, LoaiGame, Server, TaiKhoan, MatKhau, Gia, GiaGoc, ChiTiet, ThongTinKhac, CapKhaiPha, AccVip, AccKhoiDau, PhanTram, IDLoaiAcc")] Acc acc, int[] DSNhanVat, int[] DSVuKhi, HttpPostedFileBase ImageUpload, HttpPostedFileBase[] ImageUploads)
        {
            ViewBag.tkct = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1 && level != 2) || idnd == 0)
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
            var searchAcc = db.Accs.OrderByDescending(m => m.IDAcc).FirstOrDefault();
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

        public ActionResult xoaanh(int id)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1 && level != 2) || idnd == 0)
            {
                return Redirect("/");
            }

            //phan quyen
            var anh_acc = db.Anh_Acc.Find(id);
            var getAcc = db.Accs.Where(m => m.IDAcc == anh_acc.IDAcc).FirstOrDefault();
            if (getAcc == null || getAcc.IDNguoiDung != idnd)
            {
                return Json(new { Message = "Lỗi", JsonRequestBehavior.AllowGet });
            }
            string path = Server.MapPath(anh_acc.DuongDanAnh);
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
            db.Anh_Acc.Remove(anh_acc);
            db.SaveChanges();
            return Json(new { Message = "Thành công", JsonRequestBehavior.AllowGet });
        }

        public ActionResult sua(int id)
        {
            ViewBag.tkct = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1 && level != 2) || idnd == 0)
            {
                return Redirect("/");
            }
            var getAcc = db.Accs.Where(m => m.IDAcc == id).FirstOrDefault();
            if (getAcc.IDNguoiDung != idnd || getAcc.DaBan == true)
            {
                return RedirectToAction("index");
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult sua(int id, [Bind(Include = "TenAcc, LoaiGame, Server, TaiKhoan, MatKhau, Gia, GiaGoc, ChiTiet, ThongTinKhac, CapKhaiPha, AccVip, AccKhoiDau, PhanTram, IDLoaiAcc")] Acc acc, int[] DSNhanVat, int[] DSVuKhi, HttpPostedFileBase ImageUpload, HttpPostedFileBase[] ImageUploads)
        {
            ViewBag.tkct = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1 && level != 2) || idnd == 0)
            {
                return Redirect("/");
            }
            var getAcc = db.Accs.Where(m => m.IDAcc == id).FirstOrDefault();
            if (getAcc.IDNguoiDung != idnd || getAcc.DaBan == true)
            {
                return RedirectToAction("index");
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
            var caidat = db.CaiDats;

            var isSaleTet = caidat.Where(m => m.MaCaiDat == "is_sale_tet").Select(m => m.NoiDung).FirstOrDefault();
            var phanTramSale = caidat.Where(m => m.MaCaiDat == "phan_tram_sale_tet").Select(m => m.NoiDung).FirstOrDefault();
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
            setAcc.ChiTiet = acc.ChiTiet;
            setAcc.ThongTinKhac = acc.ThongTinKhac;
            setAcc.CapKhaiPha = acc.CapKhaiPha;

            setAcc.IDLoaiAcc = acc.IDLoaiAcc;
            setAcc.PhanTram = acc.PhanTram;

            // Xử lý nhân vật
            setAcc.NhanVat = "";
            setAcc.AnhNhanVat = "";
            setAcc.TenNhanVat = "";

            if (DSNhanVat != null)
            {
                var nhanVat = db.NhanVats.Where(m => DSNhanVat.Contains(m.IDNhanVat)).ToList();
                setAcc.NhanVat = string.Join(" - ", nhanVat.Select(m => m.TenNhanVat));
                setAcc.AnhNhanVat = string.Join(", ", nhanVat.Select(m => m.Avatar));
                setAcc.TenNhanVat = string.Join(", ", nhanVat.Select(m => m.TenNhanVat));
            }

            // Xử lý vũ khí
            setAcc.VuKhi = "";
            setAcc.AnhVuKhi = "";
            setAcc.TenVuKhi = "";

            if (DSVuKhi != null)
            {
                var vuKhi = db.VuKhis.Where(m => DSVuKhi.Contains(m.IDVuKhi)).ToList();
                setAcc.VuKhi = string.Join(" - ", vuKhi.Select(m => m.TenVuKhi));
                setAcc.AnhVuKhi = string.Join(", ", vuKhi.Select(v => v.Avatar));
                setAcc.TenVuKhi = string.Join(", ", vuKhi.Select(v => v.TenVuKhi));
            }
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


        public ActionResult taikhoandaban(int? page, string search)
        {
            int pageNumber = (page ?? 1);
            int newpage = pageNumber - 1;

            ViewBag.tkct = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1 && level != 2) || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen

            var data = db.Accs.Where(m => m.Xoa != true && m.DaBan == true && m.IDNguoiDung == idnd).Include(m => m.LoaiGame1).OrderByDescending(m => m.ThoiGianBan).ThenByDescending(m => m.IDAcc).Skip(newpage * 40).Take(40).ToList();
            decimal count = db.Accs.Where(m => m.Xoa != true && m.DaBan == true).Count();
            if (search != null)
            {
                data = db.Accs.Where(m => m.Xoa != true && m.DaBan == true && m.IDNguoiDung == idnd && (m.TenAcc.Contains(search) || m.MaTaiKhoan.Contains(search) || m.TaiKhoan.Contains(search))).Include(m => m.LoaiGame1).OrderByDescending(m => m.ThoiGianBan).ThenByDescending(m => m.IDAcc).Skip(newpage * 40).Take(40).ToList();
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
            if ((level != 1 && level != 2) || idnd == 0)
            {
                return Redirect("/");
            }
            var getAcc = db.Accs.Where(m => m.IDAcc == id).FirstOrDefault();
            if (getAcc.IDNguoiDung != idnd || getAcc.DaBan == true)
            {
                return RedirectToAction("index");
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
            return RedirectToAction("index");

            
        }
    }
}