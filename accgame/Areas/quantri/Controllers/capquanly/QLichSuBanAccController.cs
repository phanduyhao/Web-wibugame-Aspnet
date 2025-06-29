using accgame.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using accgame.Filters;

namespace accgame.Areas.quantri.Controllers.capquanly
{
    [CheckSessionID]
    
    public class QLichSuBanAccController : Controller
    {
        // GET: quantri/QLichSuBanAcc
        /*        public ActionResult taikhoandaban(int? page, string search)
                {
                    using (var db = new accgameEntities())
                    {
                        int pageNumber = (page ?? 1);
                        int newpage = pageNumber - 1;

                        ViewBag.qltk = "active";
                        int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                        int level = Convert.ToInt16(Session["levelAdmin"]);
                        if ((level != 1 && level != 3) || idnd == 0)
                        {
                            return Redirect("/");
                        }
                        //phan quyen

                        var data = db.Accs.Where(m => m.Xoa != true && m.DaBan == true).Include(m => m.LoaiGame1).Include(m => m.NguoiDung).Include(m => m.NguoiDung1).OrderByDescending(m => m.ThoiGianBan).ThenByDescending(m => m.IDAcc).Skip(newpage * 40).Take(40).ToList();
                        decimal count = db.Accs.Where(m => m.Xoa != true && m.DaBan == true).Count();
                        if (search != null)
                        {
                            data = db.Accs.Where(m => m.Xoa != true && m.DaBan == true && (m.TenAcc.Contains(search) || m.MaTaiKhoan.Contains(search) || m.TaiKhoan.Contains(search))).Include(m => m.NguoiDung1).Include(m => m.LoaiGame1).OrderByDescending(m => m.ThoiGianBan).ThenByDescending(m => m.IDAcc).Skip(newpage * 40).Take(40).ToList();
                            count = db.Accs.Where(m => m.Xoa != true && m.DaBan == true && (m.TenAcc.Contains(search) || m.MaTaiKhoan.Contains(search) || m.TaiKhoan.Contains(search))).Count();
                        }
                        ViewBag.search = search;
                        int last_page = Convert.ToInt16(Math.Ceiling(count / 40));
                        ViewBag.last_page = last_page;
                        ViewBag.page = pageNumber;
                        return View(data);
                    }

                }

                public ActionResult Index(int? page, string search)
                {
                    using (var db = new accgameEntities())
                    {
                        int pageNumber = (page ?? 1);
                        int newpage = pageNumber - 1;

                        ViewBag.qltk = "active";
                        int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                        int level = Convert.ToInt16(Session["levelAdmin"]);
                        if ((level != 1 && level != 3) || idnd == 0)
                        {
                            return Redirect("/");
                        }
                        //phan quyen

                        var data = db.Accs.Where(m => m.Xoa != true && m.DaBan != true).Include(m => m.LoaiGame1).Include(m => m.NguoiDung).Include(m => m.NguoiDung1).OrderByDescending(m => m.ThoiGianBan).ThenByDescending(m => m.IDAcc).Skip(newpage * 40).Take(40).ToList();
                        decimal count = db.Accs.Where(m => m.Xoa != true && m.DaBan != true).Count();
                        if (search != null)
                        {
                            data = db.Accs.Where(m => m.Xoa != true && m.DaBan != true && (m.TenAcc.Contains(search) || m.MaTaiKhoan.Contains(search) || m.TaiKhoan.Contains(search))).Include(m => m.NguoiDung1).Include(m => m.LoaiGame1).OrderByDescending(m => m.ThoiGianBan).ThenByDescending(m => m.IDAcc).Skip(newpage * 40).Take(40).ToList();
                            count = db.Accs.Where(m => m.Xoa != true && m.DaBan != true && (m.TenAcc.Contains(search) || m.MaTaiKhoan.Contains(search) || m.TaiKhoan.Contains(search))).Count();
                        }
                        ViewBag.search = search;
                        int last_page = Convert.ToInt16(Math.Ceiling(count / 40));
                        ViewBag.last_page = last_page;
                        ViewBag.page = pageNumber;
                        return View(data);
                    }

                }
            */
        accgameEntities db = new accgameEntities();

        public ActionResult AccVip(int? page, string sapxep, string searchNM, string searchMa, string searchTK, string searchND, int? loaigame, string theloaiacc)
        {
            using (var db = new accgameEntities())
            {
                int pageNumber = (page ?? 1);
                int newpage = pageNumber - 1;

                ViewBag.qltk = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if ((level != 1 && level != 3) || idnd == 0)
                {
                    return Redirect("/");
                }
                var query = db.Accs.Where(m => m.Xoa != true)
                   .Include(m => m.NguoiDung1).Include(m => m.NguoiDung).Include(m => m.LoaiAcc);
                var listLoaiGame = db.LoaiGames.OrderBy(m => m.STT).ToList();
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

                if (!string.IsNullOrEmpty(searchNM))
                {
                    query = query.Where(m => m.NguoiDung1.TenNguoiDung.Contains(searchNM)); // Filter by the buyer's name
                }

                var data = query.OrderByDescending(m => m.ThoiGianBan)
                                .ThenByDescending(m => m.IDAcc)
                                .Skip(newpage * 40)
                                .Take(40)
                                .ToList();

                decimal count = query.Count();
                var listAcc = db.LoaiAccs.Where(m => m.Hide != true).OrderBy(m => m.STT).ToList();

                ViewBag.searchMa = searchMa;
                ViewBag.searchTK = searchTK;
                ViewBag.searchND = searchND;
                ViewBag.searchNM = searchNM;
                ViewBag.loaigame = loaigame;
                ViewBag.sapxep = sapxep;
                ViewBag.theloaiacc = theloaiacc;
                int last_page = Convert.ToInt16(Math.Ceiling(count / 40));
                ViewBag.last_page = last_page;
                ViewBag.page = pageNumber;
                ViewBag.listgame = listLoaiGame;
                ViewBag.loaiacc = listAcc;
                ViewBag.LevelAdmin = level;

                return View(data);
            }
        }
        public ActionResult RandomRR(int? page, string sapxep, string searchNM, string searchMa, string searchTK, string searchND, int? loaigame, string theloaiacc)
        {
            int pageNumber = (page ?? 1);
            int newpage = pageNumber - 1;

            ViewBag.qltk = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1 && level != 3) || idnd == 0)
            {
                return Redirect("/");
            }
            // Khởi tạo query từ cơ sở dữ liệu với điều kiện Xoa != true
            var query = db.AccRandoms.Where(m => m.Xoa != true).Include(m => m.NguoiDung1);
            var listLoaiGame = db.LoaiGames.OrderBy(m => m.STT).ToList();
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
            if (!string.IsNullOrEmpty(searchNM))
            {
                query = query.Where(m => m.NguoiDung1.TenNguoiDung.Contains(searchNM)); // Filter by the buyer's name
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
            ViewBag.searchNM = searchNM;
            ViewBag.sapxep = sapxep;
            int last_page = Convert.ToInt16(Math.Ceiling(count / 40));
            ViewBag.last_page = last_page;
            ViewBag.page = pageNumber;
            ViewBag.listgame = listLoaiGame;
            ViewBag.loaiacc = listAcc;
            ViewBag.theloaiacc = theloaiacc;
            ViewBag.LevelAdmin = level;

            if (Request.UrlReferrer != null)
            {
                TempData["PreviousUrl"] = Request.UrlReferrer.ToString();
            }
            return View(data);
        }


    }
}