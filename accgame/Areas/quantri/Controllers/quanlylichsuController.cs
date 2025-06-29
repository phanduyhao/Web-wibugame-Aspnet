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

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    
    public class quanlylichsuController : Controller
    {
        accgameEntities db = new accgameEntities();
        public ActionResult LsMuaAcc(int? page, string searchMa, string searchTK, string searchND, string searchNM, int? loaigame, string theloaiacc)
        {
            int pageNumber = (page ?? 1);
            int newpage = pageNumber - 1;

            ViewBag.qlls = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }

            var query = db.Accs.Where(m => m.Xoa != true && m.DaBan == true)
                               .Include(m => m.NguoiDung1); // Include the buyer's details using NguoiDung1

            var listLoaiGame = db.LoaiGames.OrderBy(m => m.STT).ToList();

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
                query = query.Where(m => m.NguoiDung.TenNguoiDung.Equals(searchND)); // Filter by the buyer's name
            }

            if (!string.IsNullOrEmpty(searchNM))
            {
                query = query.Where(m => m.NguoiDung1.TenNguoiDung.Contains(searchNM)); // Filter by the buyer's name
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
            var listAcc = db.LoaiAccs.Where(m => m.Hide != true).OrderBy(m => m.STT).ToList();

            ViewBag.searchMa = searchMa;
            ViewBag.searchTK = searchTK;
            ViewBag.searchND = searchND;
            ViewBag.searchNM = searchNM;
            ViewBag.loaigame = loaigame;
            ViewBag.theloaiacc = theloaiacc;
            int last_page = Convert.ToInt16(Math.Ceiling(count / 40));
            ViewBag.last_page = last_page;
            ViewBag.page = pageNumber;
            ViewBag.listgame = listLoaiGame;
            ViewBag.loaiacc = listAcc;
            if (Request.UrlReferrer != null)
            {
                TempData["PreviousUrl"] = Request.UrlReferrer.ToString();
            }
            return View(data);
        }

        public ActionResult LsMuaAccRandom(int? page, string searchMa, string searchTK, string searchND, string searchNM, int? loaigame, string theloaiacc)
        {
            int pageNumber = (page ?? 1);
            int newpage = pageNumber - 1;

            ViewBag.qlls = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }

            var query = db.AccRandoms.Where(m => m.Xoa != true && m.DaBan == true)
                               .Include(m => m.NguoiDung1); // Include the buyer's details using NguoiDung1

            var listLoaiGame = db.LoaiGames.OrderBy(m => m.STT).ToList();

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
                query = query.Where(m => m.NguoiDung.TenNguoiDung.Equals(searchND)); // Filter by the buyer's name
            }

            if (!string.IsNullOrEmpty(searchNM))
            {
                query = query.Where(m => m.NguoiDung1.TenNguoiDung.Contains(searchNM)); // Filter by the buyer's name
            }

            if (loaigame.HasValue)
            {
                query = query.Where(m => m.DanhMuc.LoaiGame == loaigame.Value);
            }

            var data = query.OrderByDescending(m => m.ThoiGianBan)
                            .ThenByDescending(m => m.IDAccRandom)
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
            ViewBag.theloaiacc = theloaiacc;
            int last_page = Convert.ToInt16(Math.Ceiling(count / 40));
            ViewBag.last_page = last_page;
            ViewBag.page = pageNumber;
            ViewBag.listgame = listLoaiGame;
            ViewBag.loaiacc = listAcc;
            if (Request.UrlReferrer != null)
            {
                TempData["PreviousUrl"] = Request.UrlReferrer.ToString();
            }
            return View(data);
        }

    }
}