using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using accgame.Filters;
using accgame.Models;
using PagedList;
using PagedList.Mvc;

namespace accgame.Controllers
{
    [CheckSessionID]

    public class MuaCodeController : Controller
    {
        // GET: MuaCode
        public ActionResult Index(int? page, string id)
        {
            using (var db = new accgameEntities())
            {
                int pageSize = 20;
                int pageNumber = (page ?? 1);
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                Session["url"] = "/MuaCode";
                if (idnd == 0)
                {
                    return Redirect("/dangnhap");
                }
                ViewBag.slug = id;
                var loaiGame = db.LoaiGames.Where(x => x.Slug == id).FirstOrDefault();
                if(loaiGame == null)
                {
                    return Redirect("/");
                }
                ViewBag.tieude = loaiGame.TenLoaiGame;
                ViewBag.AnhCode = loaiGame.AnhCode;
                HomeModels data = new HomeModels()
                {
                    listLoaiCode = db.LoaiCodes.Where(m => m.Xoa != true && m.CodeNoiBo != true && m.LoaiGame == loaiGame.IDLoaiGame).ToList(),
                    listCode = db.Codes.Where(m => m.DaBan == true && m.IDNguoiDung == idnd).Include(m => m.LoaiCode).OrderByDescending(m => m.ThoiGianMua).ThenByDescending(m => m.IDCode).ToPagedList(pageNumber, pageSize),
                };
                return View(data);
            }
        }
        public ActionResult phantrang(int? page)
        {
            using (var db = new accgameEntities())
            {
                int pageSize = 20;
                int pageNumber = (page ?? 1);
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                Session["url"] = "/MuaCode";
                if (idnd == 0)
                {
                    return Content("Bạn chưa đăng nhập!");
                }
                HomeModels data = new HomeModels()
                {
                    listCode = db.Codes.Where(m => m.DaBan == true && m.IDNguoiDung == idnd).Include(m => m.LoaiCode).OrderByDescending(m => m.IDCode).ToPagedList(pageNumber, pageSize),
                };
                return PartialView("phantrang", data);
            }
        }
        public ActionResult mua(int soluong, int idloaicode)
        {

            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            Session["url"] = "/MuaCode";
            if (idnd == 0)
            {
                return Content("Bạn phải <a class=\"btn btn-danger rouded-pill\" href=\"/dangnhap\">Đăng nhập</a> mới có thể mua code!");
            }
            using (var db = new accgameEntities())
            {
                var loaicode = db.LoaiCodes.Where(m => m.IDLoaiCode == idloaicode && m.Xoa != true).FirstOrDefault();
                if(loaicode == null)
                {
                    return Content("Không tìm thấy loại code này!");
                }
                if (Convert.ToInt16(soluong) < 1)
                {
                    return Content("Số lượng phải lớn hơn 1!");
                }
                var code = db.Codes.Where(m => m.DaBan != true && m.Xoa != true && m.IDLoaiCode == idloaicode).ToList().Take(soluong);
                if(code.Count() == 0)
                {
                    return Content("Đã hết loại code này, xin mời chọn loại code khác!");
                }
                var getNguoiDung = db.NguoiDungs.Find(idnd);
                var tienTruoc = Convert.ToInt32( getNguoiDung.Tien);
                
                if(Convert.ToInt32(getNguoiDung.Tien) < loaicode.Gia * soluong)
                {
                    return Content("Không đủ tiền! Bạn hãy <a class=\"btn btn-danger rouded-pill\" href=\"/naptien/atm\">Nạp tiền</a> để mua mã code này");
                }
                if(loaicode.CodeNoiBo != true)
                {
                    foreach (var item in code)
                    {
                        var setcode = db.Codes.Find(item.IDCode);
                        var setNguoiDung = db.NguoiDungs.Find(idnd);
                        setcode.IDNguoiDung = idnd;
                        setcode.DaBan = true;
                        setcode.ThoiGianMua = DateTime.Now;
                        setcode.MaCode = item.MaCode;
                        setNguoiDung.Tien = getNguoiDung.Tien - loaicode.Gia;
                        db.SaveChanges();
                    }
                }
                else
                {
                    int level = Convert.ToInt16(Session["levelAdmin"]);
                    if (level != 1 && level != 2 && level != 3)
                    {
                        return Content("Bạn không thể mua loại code này!");
                    }
                    foreach (var item in code)
                    {
                        var setcode = db.Codes.Find(item.IDCode);
                        var setNguoiDung = db.NguoiDungs.Find(idnd);
                        setcode.IDNguoiDung = idnd;
                        setcode.DaBan = true;
                        setcode.ThoiGianMua = DateTime.Now;
                        setcode.MaCode = item.MaCode;
                        setNguoiDung.Tien = getNguoiDung.Tien - loaicode.Gia;
                        db.SaveChanges();
                    }
                }

                //Biến động số dư:
                var biendongds = new BienDongSoDu();
                biendongds.IDNguoiDung = idnd;
                biendongds.SoTien = loaicode.Gia * soluong;
                biendongds.TruTien = true;
                biendongds.ThoiGian = DateTime.Now;
                biendongds.LoiNhan = "Mua code (" + soluong +")";
                biendongds.TienTruoc = tienTruoc;
                biendongds.TienSau = tienTruoc - loaicode.Gia * soluong;
                db.BienDongSoDus.Add(biendongds);
                //
                //Biến động số dư admin:
                var admin = db.NguoiDungs.Where(m => m.LeverAdmin == 1).FirstOrDefault();
                if (admin != null)
                {
                    var tienTruocAdmin = admin.Tien;
                    var biendongdsad = new BienDongSoDu();
                    biendongdsad.IDNguoiDung = admin.IDNguoiDung;
                    biendongdsad.SoTien = loaicode.Gia * soluong;
                    biendongdsad.TruTien = false;
                    biendongdsad.ThoiGian = DateTime.Now;
                    biendongdsad.LoiNhan = "Khách mua code (" + soluong + ")";
                    var setAdmin = db.NguoiDungs.Find(admin.IDNguoiDung);
                    if (setAdmin != null)
                    {
                        setAdmin.Tien += (loaicode.Gia * soluong);
                    }
                    biendongdsad.TienTruoc = tienTruocAdmin;
                    biendongdsad.TienSau = setAdmin.Tien;
                    db.BienDongSoDus.Add(biendongdsad);
                }

                //
                db.SaveChanges();
            }
            return Content("Mua code thành công!");
        }
    }
}