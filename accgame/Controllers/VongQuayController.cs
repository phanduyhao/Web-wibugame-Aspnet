using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using accgame.Filters;
using PagedList;

namespace accgame.Controllers
{
    [CheckSessionID]

    public class VongQuayController : Controller
    {
        // GET: VongQuay
        public ActionResult Index(int? page)
        {
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            using (var db = new accgameEntities())
            {
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                Session["url"] = "/vongquay/";
                if (idnd == 0)
                {
                    return Redirect("/dangnhap");
                }
                ViewBag.gia = db.CaiDatVongQuays.FirstOrDefault().Gia;
                var vongquay = db.VongQuays.Where(m => m.IDNguoiDung == idnd).OrderByDescending(m => m.IDVongQuay).ToPagedList(pageNumber, pageSize);
                return View(vongquay);
            }
        }

        [HttpPost]
        public ActionResult postvq()
        {
            using(var db = new accgameEntities())
            {
                var mesage = "";
                var caidat = db.CaiDatVongQuays.FirstOrDefault();
                int GiaVq = Convert.ToInt32(caidat.Gia);
                if (caidat == null)
                {
                    mesage = "Chưa thiết lập vòng quay, hãy thông báo cho QTV!";
                    return Json(new { id = 0, mesage = mesage, error = 1 });
                }
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                Session["url"] = "/vongquay/Index";
                int id = 0;
                
                if (idnd == 0)
                {
                    mesage = "Bạn cần phải đăng nhập trước!";
                    return Json(new { id = id, mesage = mesage, error = 1 });
                }
                var nguoidung = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).FirstOrDefault();
                if (Convert.ToInt32(nguoidung.Tien) < GiaVq)
                {
                    mesage = "Không đủ tiền, hãy nạp thêm tiền để có thể quay thưởng!";
                    return Json(new { id = id, mesage = mesage, error = 2 });
                };
                System.Random random = new System.Random();
                double randomNumber = random.NextDouble() * 100;
                var setNguoiDung = db.NguoiDungs.Find(idnd);
                var tienTruoc = setNguoiDung.Tien;
                
                VongQuay vongQuay = new VongQuay();
                if (randomNumber < caidat.TyLe1)
                {
                    mesage = "Chúc mừng bạn đã trúng thưởng Char 5 * + WP";
                    id = 0;
                    vongQuay.GiaTien = GiaVq;
                    vongQuay.LoaiGiai = 1;
                    var giaithuong = db.GiaiThuongVongQuays.Where(m => m.LoaiGiai == 1 && m.DaBan != true).FirstOrDefault();
                    if(giaithuong == null)
                    {
                        mesage = "Đã hết giải thưởng, hãy đợi Admin bổ sung giải thưởng bạn nhé!";
                        return Json(new { id = id, mesage = mesage, error = 3 });
                    }
                    var setGT = db.GiaiThuongVongQuays.Find(giaithuong.IDGiaiThuongVongQuay);
                    setGT.DaBan = true;
                    setGT.ThoiGianBan = DateTime.Now;
                    vongQuay.NoiDung = "Char 5 * + WP" + " - " + giaithuong.ThongTin;
                    vongQuay.IDNguoiDung = idnd;
                    vongQuay.ThoiGian = DateTime.Now;
                    db.VongQuays.Add(vongQuay);
                    db.SaveChanges();
                }
                else if (randomNumber < caidat.TyLe1 + caidat.TyLe2)
                {
                    mesage = "Chúc mừng bạn đã trúng thưởng Char limited";
                    vongQuay.GiaTien = GiaVq;
                    vongQuay.LoaiGiai = 2;
                    id = 1;
                    var giaithuong = db.GiaiThuongVongQuays.Where(m => m.LoaiGiai == 2 && m.DaBan != true).FirstOrDefault();
                    if (giaithuong == null)
                    {
                        mesage = "Đã hết giải thưởng, hãy đợi Admin bổ sung giải thưởng bạn nhé!";
                        return Json(new { id = id, mesage = mesage, error = 3 });
                    }
                    var setGT = db.GiaiThuongVongQuays.Find(giaithuong.IDGiaiThuongVongQuay);
                    setGT.DaBan = true;
                    setGT.ThoiGianBan = DateTime.Now;
                    vongQuay.NoiDung = "Char limited" + " - " + giaithuong.ThongTin;
                    vongQuay.IDNguoiDung = idnd;
                    vongQuay.ThoiGian = DateTime.Now;
                    db.VongQuays.Add(vongQuay);
                    db.SaveChanges();
                }
                else if (randomNumber < caidat.TyLe1 + caidat.TyLe2 + caidat.TyLe3)
                {

                    mesage = "Chúc mừng bạn đã trúng thưởng One Char 5*";
                    vongQuay.GiaTien = GiaVq;
                    vongQuay.LoaiGiai = 3;
                    id = 2;
                    var giaithuong = db.GiaiThuongVongQuays.Where(m => m.LoaiGiai == 3 && m.DaBan != true).FirstOrDefault();
                    if (giaithuong == null)
                    {
                        mesage = "Đã hết giải thưởng, hãy đợi Admin bổ sung giải thưởng bạn nhé!";
                        return Json(new { id = id, mesage = mesage, error = 3 });
                    }
                    var setGT = db.GiaiThuongVongQuays.Find(giaithuong.IDGiaiThuongVongQuay);
                    setGT.DaBan = true;
                    setGT.ThoiGianBan = DateTime.Now;
                    vongQuay.NoiDung = "One Char 5*" + " - " + giaithuong.ThongTin;
                    vongQuay.IDNguoiDung = idnd;
                    vongQuay.ThoiGian = DateTime.Now;
                    db.VongQuays.Add(vongQuay);
                    db.SaveChanges();
                }
                else if (randomNumber < caidat.TyLe1 + caidat.TyLe2 + caidat.TyLe3 + caidat.TyLe4)
                {
                    mesage = "Chúc mừng bạn đã trúng thưởng >2 Char 5*";
                    vongQuay.GiaTien = GiaVq;
                    vongQuay.LoaiGiai = 4;
                    id = 3;
                    var giaithuong = db.GiaiThuongVongQuays.Where(m => m.LoaiGiai == 4 && m.DaBan != true).FirstOrDefault();
                    if (giaithuong == null)
                    {
                        mesage = "Đã hết giải thưởng, hãy đợi Admin bổ sung giải thưởng bạn nhé!";
                        return Json(new { id = id, mesage = mesage, error = 3 });
                    }
                    var setGT = db.GiaiThuongVongQuays.Find(giaithuong.IDGiaiThuongVongQuay);
                    setGT.DaBan = true;
                    setGT.ThoiGianBan = DateTime.Now;
                    vongQuay.NoiDung = ">2 Char 5*" + " - " + giaithuong.ThongTin;
                    vongQuay.IDNguoiDung = idnd;
                    vongQuay.ThoiGian = DateTime.Now;
                    db.VongQuays.Add(vongQuay);
                    db.SaveChanges();
                }
                else if (randomNumber < caidat.TyLe1 + caidat.TyLe2 + caidat.TyLe3 + caidat.TyLe4 + caidat.TyLe5)
                {
                    mesage = "Chúc mừng bạn đã trúng thưởng Acc Reroll";
                    vongQuay.GiaTien = GiaVq;
                    vongQuay.LoaiGiai = 5;
                    id = 4;
                    var giaithuong = db.GiaiThuongVongQuays.Where(m => m.LoaiGiai == 5 && m.DaBan != true).FirstOrDefault();
                    if (giaithuong == null)
                    {
                        mesage = "Đã hết giải thưởng, hãy đợi Admin bổ sung giải thưởng bạn nhé!";
                        return Json(new { id = id, mesage = mesage, error = 3 });
                    }
                    var setGT = db.GiaiThuongVongQuays.Find(giaithuong.IDGiaiThuongVongQuay);
                    setGT.DaBan = true;
                    setGT.ThoiGianBan = DateTime.Now;
                    vongQuay.NoiDung = "Acc Reroll" + " - " + giaithuong.ThongTin;
                    vongQuay.IDNguoiDung = idnd;
                    vongQuay.ThoiGian = DateTime.Now;
                    db.VongQuays.Add(vongQuay);
                    db.SaveChanges();
                }
                else if (randomNumber < caidat.TyLe1 + caidat.TyLe2 + caidat.TyLe3 + caidat.TyLe4 + caidat.TyLe5 + caidat.TyLe6)
                {
                    mesage = "Chúc mừng bạn đã trúng thưởng Acc Reroll Vip";
                    vongQuay.GiaTien = GiaVq;
                    vongQuay.LoaiGiai = 6;
                    id = 5;
                    var giaithuong = db.GiaiThuongVongQuays.Where(m => m.LoaiGiai == 6 && m.DaBan != true).FirstOrDefault();
                    if (giaithuong == null)
                    {
                        mesage = "Đã hết giải thưởng, hãy đợi Admin bổ sung giải thưởng bạn nhé!";
                        return Json(new { id = id, mesage = mesage, error = 3 });
                    }
                    var setGT = db.GiaiThuongVongQuays.Find(giaithuong.IDGiaiThuongVongQuay);
                    setGT.DaBan = true;
                    setGT.ThoiGianBan = DateTime.Now;
                    vongQuay.NoiDung = "Acc Reroll Vip" + " - " + giaithuong.ThongTin;
                    vongQuay.IDNguoiDung = idnd;
                    vongQuay.ThoiGian = DateTime.Now;
                    db.VongQuays.Add(vongQuay);
                    db.SaveChanges();
                }
                setNguoiDung.Tien = setNguoiDung.Tien - GiaVq;
                //Biến động số dư:
                var biendongds = new BienDongSoDu();
                biendongds.IDNguoiDung = idnd;
                biendongds.SoTien = GiaVq;
                biendongds.TruTien = true;
                biendongds.ThoiGian = DateTime.Now;
                biendongds.LoiNhan = "VQMM";
                biendongds.TienTruoc = tienTruoc;
                biendongds.TienSau = setNguoiDung.Tien;
                db.BienDongSoDus.Add(biendongds);
                //
                //Biến động số dư admin:
                var admin = db.NguoiDungs.Where(m => m.LeverAdmin == 1).FirstOrDefault(); ;
                if (admin != null)
                {
                    var TienTruocpAdmin = admin.Tien;
                    var biendongdsad = new BienDongSoDu();
                    biendongdsad.IDNguoiDung = admin.IDNguoiDung;
                    biendongdsad.SoTien = GiaVq;
                    biendongdsad.TruTien = false;
                    biendongdsad.ThoiGian = DateTime.Now;
                    biendongdsad.LoiNhan = "Khách VQMMM";

                    
                    var setAdmin = db.NguoiDungs.Find(admin.IDNguoiDung);
                    if (setAdmin != null)
                    {
                        setAdmin.Tien += GiaVq;
                    }
                    biendongdsad.TienTruoc = TienTruocpAdmin;
                    biendongdsad.TienSau = setAdmin.Tien;
                    db.BienDongSoDus.Add(biendongdsad);
                }

                //
                db.SaveChanges();
                return Json(new { id = id, mesage = mesage, error = 0 });
            }
           
        }
    }
}