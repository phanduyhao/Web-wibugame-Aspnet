using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using PagedList;
using accgame.Models;
using System.Data.Entity;

namespace accgame.Controllers
{
    public class DanhSachController : Controller
    {
        [NotifyUserFilter]

        public ActionResult Accgame(string id)
        {

            using (var db = new accgameEntities())
            {
                var loaigame = db.LoaiGames.Where(m => m.Slug == id && m.Hide != true).FirstOrDefault();
                if (loaigame == null)
                {
                    return View("error");
                }
                var loaiAcc = db.LoaiAccs.Where(m => m.Hide != true && m.IDLoaiAccCha == null && m.IDLoaiGame == loaigame.IDLoaiGame).OrderBy(m => m.STT).ThenByDescending(m => m.IDLoaiAcc).ToList();
                ViewBag.tenGame = loaigame.TenLoaiGame;

                return View(loaiAcc);
            }
        }
        // GET: DanhSach
        public ActionResult loaigame (int? page, string[] nhanvat, string[] vukhi, string CapKhaiPha, int? timtheogia, string timtheoma, int? sapxep, string id, string server) 
        {
            
            int pageSize = 18;
            int pageNumber = (page ?? 1);
            ViewBag.nhanVat = nhanvat ?? new string[0];
            ViewBag.vukhi = vukhi ?? new string[0];
            ViewBag.CapKhaiPha = CapKhaiPha;
            ViewBag.timtheogia = timtheogia ?? 0;
            ViewBag.timtheoma = timtheoma;
            ViewBag.sapxep = sapxep;
            ViewBag.id = id;
            ViewBag.server = server;
            using (var db = new accgameEntities())
            {
                var loaiGame = db.LoaiGames.Where(m => m.Slug == id).FirstOrDefault();
                ViewBag.slug = loaiGame.Slug;
                ViewBag.loaigame = loaiGame.TenLoaiGame;
                if(loaiGame == null){
                    return View("error");
                }
                var dulieu = db.Accs.Where(m => m.Xoa != true && m.DaBan != true && m.LoaiGame == loaiGame.IDLoaiGame && m.TaiKhoanNoiBo != true && m.An != true).AsQueryable();
                string getSearchNv = "";
                string getSearchVk = "";
                if (server != null && server != "")
                {
                    dulieu = dulieu.Where(d => d.Server == server);
                }
                if (nhanvat != null)
                {
                    dulieu = dulieu.Where(d => nhanvat.All(item => d.NhanVat.Contains(item)));
                    getSearchNv = string.Join(",", nhanvat);
                }
                if (vukhi != null)
                {
                    dulieu = dulieu.Where(d => vukhi.All(item => d.VuKhi.Contains(item)));
                    getSearchVk = string.Join(",", vukhi);
                }
                if (CapKhaiPha != null && CapKhaiPha != "")
                {
                    dulieu = dulieu.Where(d => CapKhaiPha.Contains(d.CapKhaiPha)) ;
                }
                if (timtheoma != null && timtheoma != "")
                {
                    dulieu = dulieu.Where(d => d.MaTaiKhoan.Contains(timtheoma));
                }

                if (timtheogia.HasValue)
                {
                    if (timtheogia.Value == 1)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 20000);
                    }
                    else if (timtheogia.Value == 2)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 100000 && d.Gia > 20000);
                    }
                    else if (timtheogia.Value == 3)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 200000 && d.Gia > 100000);
                    }
                    else if (timtheogia.Value == 4)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 300000 && d.Gia > 200000);
                    }
                    else if (timtheogia.Value == 5)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 400000 && d.Gia > 300000);
                    }
                    else if (timtheogia.Value == 6)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 500000 && d.Gia > 400000);
                    }
                    else if (timtheogia.Value == 7)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 700000 && d.Gia > 500000);
                    }
                    else if (timtheogia.Value == 8)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 1000000 && d.Gia > 700000);
                    }
                    else if (timtheogia.Value == 9)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 2000000 && d.Gia > 1000000);
                    }
                    else if (timtheogia.Value == 10)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 3000000 && d.Gia > 2000000);
                    }
                    else if (timtheogia.Value == 11)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 4000000 && d.Gia > 3000000);
                    }
                    else if (timtheogia.Value == 12)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 6000000 && d.Gia > 4000000);
                    }
                    else if (timtheogia.Value == 13)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 8000000 && d.Gia > 6000000);
                    }
                    else if (timtheogia.Value == 14)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 10000000 && d.Gia > 8000000);
                    }
                    else if (timtheogia.Value == 15)
                    {
                        dulieu = dulieu.Where(d => d.Gia > 10000000);
                    }
                }
                dulieu = dulieu.OrderByDescending(m => m.IDAcc);
                if (sapxep.HasValue)
                {
                    if (sapxep == 1)
                    {
                        dulieu = dulieu.OrderByDescending(d => d.Gia).ThenByDescending(m => m.IDAcc);
                    }
                    else if (sapxep == 0)
                    {
                        dulieu = dulieu.OrderBy(d => d.Gia).ThenByDescending(m => m.IDAcc);
                    }
                }
                HomeModels data = new HomeModels()
                {
                    listPageAcc = dulieu.ToPagedList(pageNumber, pageSize),
                    listVuKhi = db.VuKhis.Where(m => m.LoaiGame == loaiGame.IDLoaiGame).ToList(),
                    listNhanVat = db.NhanVats.Where(m => m.LoaiGame == loaiGame.IDLoaiGame).ToList(),
                    nhanvat = nhanvat,
                    vukhi = vukhi,
                    CapKhaiPha = CapKhaiPha,
                    timtheogia = timtheogia,
                    timtheoma = timtheoma,
                    sapxep = sapxep,
                };
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                var CtvCollab = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).Select(m => m.CtvCollab).FirstOrDefault();
                ViewBag.CtvCollab = CtvCollab;
                var OnOffCtvCollab = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).Select(m => m.OnOffCtvCollab).FirstOrDefault();
                ViewBag.OnOffCtvCollab = OnOffCtvCollab;
                var CtvCollabCauHinh = db.CaiDats.Where(m => m.MaCaiDat == "is_ctv_collab").FirstOrDefault();
                ViewBag.CtvCollabCauHinh = CtvCollabCauHinh != null ? CtvCollabCauHinh.NoiDung : null;
                ViewBag.isSaleTet = db.CaiDats.Where(m => m.MaCaiDat == "is_sale_tet").Select(m => m.NoiDung).FirstOrDefault();
                ViewBag.phanTramTet = int.TryParse(db.CaiDats.Where(m => m.MaCaiDat == "phan_tram_sale_tet")
                                                        .Select(m => m.NoiDung)
                                                        .FirstOrDefault(), out var phanTramTet)
                                      ? phanTramTet
                                      : 0;
                return View(data);
            }
        }

        public ActionResult loaiAcc(int? page, string[] nhanvat, string[] vukhi, string CapKhaiPha, int? timtheogia, string timtheoma, int? sapxep, string id, string server)
        {
            int pageSize = 18;
            int pageNumber = (page ?? 1);
            ViewBag.nhanVat = nhanvat ?? new string[0];
            ViewBag.vukhi = vukhi ?? new string[0];
            ViewBag.CapKhaiPha = CapKhaiPha;
            ViewBag.timtheogia = timtheogia ?? 0;
            ViewBag.timtheoma = timtheoma;
            ViewBag.sapxep = sapxep;
            ViewBag.id = id;
            ViewBag.server = server;
            using (var db = new accgameEntities())
            {
                var loaiAcc = db.LoaiAccs.Where(m => m.slug == id).FirstOrDefault();
                ViewBag.slug = loaiAcc.slug;
                ViewBag.loaiAcc = loaiAcc.TenLoaiAcc;
                if (loaiAcc == null)
                {
                    return View("error");
                }
                var dulieu = db.Accs.Where(m => m.RanDom != true && m.Xoa != true && m.DaBan != true && m.IDLoaiAcc == loaiAcc.IDLoaiAcc && m.TaiKhoanNoiBo != true && m.AccKhoiDau != true && m.An != true).AsQueryable();
                if (server != null && server != "")
                {
                    dulieu = dulieu.Where(d => d.Server == server);
                }
                string getSearchNv = "";
                string getSearchVk = "";

                if (nhanvat != null)
                {
                    dulieu = dulieu.Where(d => nhanvat.All(item => d.NhanVat.Contains(item)));
                    getSearchNv = string.Join(",", nhanvat);
                }
                if (vukhi != null)
                {
                    dulieu = dulieu.Where(d => vukhi.All(item => d.VuKhi.Contains(item)));
                    getSearchVk = string.Join(",", vukhi);
                }
                if (CapKhaiPha != null && CapKhaiPha != "")
                {
                    dulieu = dulieu.Where(d => CapKhaiPha.Contains(d.CapKhaiPha));
                }
                if (timtheoma != null && timtheoma != "")
                {
                    dulieu = dulieu.Where(d => d.MaTaiKhoan.Contains(timtheoma));
                }

                if (timtheogia.HasValue)
                {
                    if (timtheogia.Value == 1)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 20000);
                    }
                    else if (timtheogia.Value == 2)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 100000 && d.Gia > 20000);
                    }
                    else if (timtheogia.Value == 3)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 200000 && d.Gia > 100000);
                    }
                    else if (timtheogia.Value == 4)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 300000 && d.Gia > 200000);
                    }
                    else if (timtheogia.Value == 5)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 400000 && d.Gia > 300000);
                    }
                    else if (timtheogia.Value == 6)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 500000 && d.Gia > 400000);
                    }
                    else if (timtheogia.Value == 7)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 700000 && d.Gia > 500000);
                    }
                    else if (timtheogia.Value == 8)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 1000000 && d.Gia > 700000);
                    }
                    else if (timtheogia.Value == 9)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 2000000 && d.Gia > 1000000);
                    }
                    else if (timtheogia.Value == 10)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 3000000 && d.Gia > 2000000);
                    }
                    else if (timtheogia.Value == 11)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 4000000 && d.Gia > 3000000);
                    }
                    else if (timtheogia.Value == 12)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 6000000 && d.Gia > 4000000);
                    }
                    else if (timtheogia.Value == 13)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 8000000 && d.Gia > 6000000);
                    }
                    else if (timtheogia.Value == 14)
                    {
                        dulieu = dulieu.Where(d => d.Gia <= 10000000 && d.Gia > 8000000);
                    }
                    else if (timtheogia.Value == 15)
                    {
                        dulieu = dulieu.Where(d => d.Gia > 10000000);
                    }
                }
                dulieu = dulieu.OrderByDescending(m => m.IDAcc);
                if (sapxep.HasValue)
                {
                    if (sapxep == 1)
                    {
                        dulieu = dulieu.OrderByDescending(d => d.Gia).ThenByDescending(m => m.IDAcc);
                    }
                    else if (sapxep == 0)
                    {
                        dulieu = dulieu.OrderBy(d => d.Gia).ThenByDescending(m => m.IDAcc);
                    }
                }
                HomeModels data = new HomeModels()
                {
                    listPageAcc = dulieu.ToPagedList(pageNumber, pageSize),
                    listVuKhi = db.VuKhis.Where(m => m.LoaiGame == loaiAcc.IDLoaiGame).ToList(),
                    listNhanVat = db.NhanVats.Where(m => m.LoaiGame == loaiAcc.IDLoaiGame).ToList(),
                    nhanvat = nhanvat,
                    vukhi = vukhi,
                    CapKhaiPha = CapKhaiPha,
                    timtheogia = timtheogia,
                    timtheoma = timtheoma,
                    sapxep = sapxep,
                };
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                var CtvCollab = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).Select(m => m.CtvCollab).FirstOrDefault();
                ViewBag.CtvCollab = CtvCollab;
                var OnOffCtvCollab = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).Select(m => m.OnOffCtvCollab).FirstOrDefault();
                ViewBag.OnOffCtvCollab = OnOffCtvCollab;
                var CtvCollabCauHinh = db.CaiDats.Where(m => m.MaCaiDat == "is_ctv_collab").FirstOrDefault();
                ViewBag.CtvCollabCauHinh = CtvCollabCauHinh != null ? CtvCollabCauHinh.NoiDung : null;
                ViewBag.isSaleTet = db.CaiDats.Where(m => m.MaCaiDat == "is_sale_tet").Select(m => m.NoiDung).FirstOrDefault();
                ViewBag.phanTramTet = int.TryParse(db.CaiDats.Where(m => m.MaCaiDat == "phan_tram_sale_tet")
                                                        .Select(m => m.NoiDung)
                                                        .FirstOrDefault(), out var phanTramTet)
                                      ? phanTramTet
                                      : 0;
                return View(data);
            }
        }
    }
}