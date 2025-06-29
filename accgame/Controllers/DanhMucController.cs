using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Areas.quantri.Controllers;
using accgame.Context;
using accgame.Filters;
using accgame.Models;

namespace accgame.Controllers
{
    [NotifyUserFilter]

    public class DanhMucController : Controller
    {
        public ActionResult LoaiAcc(string id)
        {

            using (var db = new accgameEntities())
            {
                var loaiAcc = db.LoaiAccs.Where(m => m.slug == id).Include(m => m.LoaiGame).Where(m => m.Hide != true).FirstOrDefault();
                if (loaiAcc == null)
                {
                    return View("error");
                }
                var danhmuc = db.DanhMucs.Where(m => m.Xoa != true && m.TaiKhoanNoiBo != true && m.IDLoaiAcc == loaiAcc.IDLoaiAcc && m.Hide != true).OrderBy(m => m.STT).ThenByDescending(m => m.IDDanhMuc).ToList();
                var thongbaomuaacc = db.CaiDats
                          .Where(m => m.MaCaiDat == "info_buy_acc")
                          .Select(m => m.NoiDung)
                          .FirstOrDefault();

                var IdLoaiAcc = loaiAcc.IDLoaiAcc;
                var listLoaiAcc = db.LoaiAccs.Where(m => m.Hide != true && m.IDLoaiAccCha == IdLoaiAcc).ToList();
                ViewBag.listLoaiAcc = listLoaiAcc;
                ViewBag.Mota = loaiAcc.MoTa as string;
                ViewBag.tenLoai = loaiAcc.TenLoaiAcc;
                ViewBag.logoGame = loaiAcc.LoaiGame.Image;
                ViewBag.thongbaomuacc = thongbaomuaacc;
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                var CtvCollabCauHinh = db.CaiDats.Where(m => m.MaCaiDat == "is_ctv_collab").FirstOrDefault();
                ViewBag.CtvCollabCauHinh = CtvCollabCauHinh != null ? CtvCollabCauHinh.NoiDung : null;
                var CtvCollab = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).Select(m => m.CtvCollab).FirstOrDefault();
                ViewBag.CtvCollab = CtvCollab;
                var OnOffCtvCollab = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).Select(m => m.OnOffCtvCollab).FirstOrDefault();
                ViewBag.OnOffCtvCollab = OnOffCtvCollab;
                return View(danhmuc);
            }
        }

        [CheckSessionID]


        public ActionResult muataikhoan(int id, int soluong)
        {

            using (var db = new accgameEntities())
            {
                var idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                var nguoidung = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).FirstOrDefault();

                if (nguoidung != null && nguoidung.Block == true)
                {
                    ViewBag.thongbao = 10;
                    return PartialView("_Rendermuataikhoan");
                }
                var CtvCollab = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).Select(m => m.CtvCollab).FirstOrDefault();
                var OnOffCtvCollab = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).Select(m => m.OnOffCtvCollab).FirstOrDefault();
                ViewBag.thongbao = 0;
                if (idnd == 0)
                {
                    ViewBag.thongbao = 1;
                    return PartialView("_Rendermuataikhoan");
                }
                var danhmuc = db.DanhMucs.Where(m => m.IDDanhMuc == id && m.Hide != true).FirstOrDefault();

                var accrandom = db.AccRandoms.Where(m => m.IDDanhMuc == danhmuc.IDDanhMuc && m.Xoa != true && m.DaBan != true).FirstOrDefault();

                var tienTruoc = Convert.ToInt32(nguoidung.Tien);
                var gia = danhmuc.Gia;
                var CtvCollabCauHinh = db.CaiDats.Where(m => m.MaCaiDat == "is_ctv_collab").FirstOrDefault();
                if (CtvCollabCauHinh.NoiDung != null && CtvCollab == true && OnOffCtvCollab == true)
                {
                    if (danhmuc.GiaCtvCollab != null)
                    {
                        gia = danhmuc.GiaCtvCollab;
                    }
                    else
                    {
                        gia = danhmuc.Gia;
                    }
                }
                else
                {
                    gia = danhmuc.Gia;
                }
                if (soluong < 0)
                {
                    ViewBag.thongbao = 5;
                    return PartialView("_Rendermuataikhoan");
                }
                if (Convert.ToInt32(nguoidung.Tien) < Convert.ToInt32(gia * soluong))
                {
                    ViewBag.tien = Convert.ToInt32(nguoidung.Tien);
                    ViewBag.thongbao = 2;
                    return PartialView("_Rendermuataikhoan");
                }
                var listAcc = db.AccRandoms.Where(m => m.IDDanhMuc == danhmuc.IDDanhMuc && m.Xoa != true && m.DaBan != true).ToList();
                if (listAcc.Count() == 0)
                {
                    ViewBag.thongbao = 3;
                    return PartialView("_Rendermuataikhoan");
                }
                if (listAcc.Count() < soluong)
                {
                    ViewBag.thongbao = 4;
                    ViewBag.count = listAcc.Count();
                    return PartialView("_Rendermuataikhoan");
                }

                // Tạo thông báo HTML
                string noiDungThongBao = $@"
                        Bạn đã mua thành công ACC <strong>{accrandom.TaiKhoan}</strong>. 
                        <a href='/taikhoandamua/TaiKhoanReroll' class='text-info' target='_blank'>Click để xem thông tin acc</a>.
                        <br>Để đảm bảo an toàn vui lòng đổi thông tin tài khoản sau khi hoàn tất giao dịch. 
                        Cảm ơn quý khách đã sử dụng dịch vụ của chúng tôi, chúc quý khách một ngày tốt lành!
                    ";

                // Tạo một NotifyUser mới
                var notify = new NotifyUser
                {
                    IdNguoidung = idnd,
                    Contents = noiDungThongBao,
                    IsRead = false,
                    IsAdminPost = false,
                    Thoigian = DateTime.Now // Nếu bạn có cột lưu thời gian
                };

                // Thêm vào bảng NotifyUser
                db.NotifyUsers.Add(notify);
                db.SaveChanges();
                var layNguoiBan = db.NguoiDungs.Where(m => m.IDNguoiDung == danhmuc.IDNguoiDung).FirstOrDefault();
                var tienTruocNguoiBan = Convert.ToInt32(layNguoiBan?.Tien ?? 0);

                if (danhmuc.TaiKhoanNoiBo != true)
                {
                    var accs = db.AccRandoms
                        .Where(m => m.IDDanhMuc == danhmuc.IDDanhMuc && m.Xoa != true && m.DaBan != true)
                        .Take(soluong)
                        .ToList();
                    var acc = db.AccRandoms.Where(m => m.IDDanhMuc == danhmuc.IDDanhMuc && m.Xoa != true && m.DaBan != true).FirstOrDefault();
                    var setacc = db.AccRandoms.Find(acc.IDAccRandom);
                    for (int i = 0; i < soluong; i++)
                    {
                        var accItem = accs[i];
                        setacc = db.AccRandoms.Find(accItem.IDAccRandom);
                        setacc.DaBan = true;
                        setacc.IDNguoiMua = idnd;
                        setacc.ThoiGianBan = DateTime.Now;
                        setacc.GiaDaMua = gia;
                        var setdanhmuc = db.DanhMucs.Find(id);
                        setdanhmuc.DaBan = Convert.ToInt32(setdanhmuc.DaBan) + 1;
                        setdanhmuc.HienCo = Convert.ToInt32(setdanhmuc.HienCo) - 1;
                        if (setdanhmuc.IDLoaiAcc != null)
                        {
                            var loaiAcc = db.LoaiAccs.Find(setdanhmuc.IDLoaiAcc);
                            if (loaiAcc != null)
                            {
                                loaiAcc.DangBan = Convert.ToInt32(loaiAcc.DangBan) - 1;
                                loaiAcc.DaBan = Convert.ToInt32(loaiAcc.DaBan) + 1;
                                db.SaveChanges();
                            }
                        }
                        var setNguoiDung = db.NguoiDungs.Find(idnd);
                        setNguoiDung.Tien = Convert.ToInt32(setNguoiDung.Tien) - gia;
                        db.SaveChanges();
                        var accRandomDamua = new AccRandomDamua();
                        accRandomDamua.IdAccRandom = setacc.IDAccRandom;
                        accRandomDamua.Danhmuc = setacc.DanhMuc.TenDanhMuc;
                        accRandomDamua.Ghichu = setacc.DanhMuc.GhiChuAcc;
                        accRandomDamua.Server = setacc.DanhMuc.Server;
                        db.AccRandomDamuas.Add(accRandomDamua);
                    }
                    //bdsd ctv
                    if (setacc.IDNguoiDung != null)
                    {
                        var NguoiBan = db.NguoiDungs.Find(setacc.IDNguoiDung);
                        tienTruocNguoiBan = Convert.ToInt32(NguoiBan.Tien);
                        var hhNguoiBan = Convert.ToInt32(gia * (NguoiBan.Phantramhoahong / 100.00) * soluong);
                        var biendongCTV = new BienDongSoDu();
                        biendongCTV.IDNguoiDung = NguoiBan.IDNguoiDung;
                        biendongCTV.SoTien = hhNguoiBan;
                        biendongCTV.TruTien = false;
                        biendongCTV.ThoiGian = DateTime.Now;
                        biendongCTV.LoiNhan = "khách mua random (reroll)";
                        biendongCTV.TienTruoc = tienTruocNguoiBan;
                        NguoiBan.Tien += hhNguoiBan;
                        biendongCTV.TienSau = NguoiBan.Tien;
                        db.BienDongSoDus.Add(biendongCTV);

                        db.SaveChanges();
                    }
                }
                else
                {
                    int level = Convert.ToInt16(Session["levelAdmin"]);
                    if (level != 1 && level != 2 && level != 3)
                    {
                        ViewBag.thongbao = 3;
                        return PartialView("_Rendermuataikhoan");
                    }
                    for (int i = 0; i < soluong; i++)
                    {
                        var acc = db.AccRandoms.Where(m => m.IDDanhMuc == danhmuc.IDDanhMuc && m.Xoa != true && m.DaBan != true).FirstOrDefault();
                        var setacc = db.AccRandoms.Find(acc.IDAccRandom);
                        setacc.DaBan = true;
                        setacc.IDNguoiMua = idnd;
                        setacc.ThoiGianBan = DateTime.Now;
                        var setdanhmuc = db.DanhMucs.Find(id);
                        setdanhmuc.DaBan = Convert.ToInt32(setdanhmuc.DaBan) + 1;
                        setdanhmuc.HienCo = Convert.ToInt32(setdanhmuc.HienCo) - 1;
                        var setNguoiDung = db.NguoiDungs.Find(idnd);
                        setNguoiDung.Tien = Convert.ToInt32(setNguoiDung.Tien) - gia;
                        if (danhmuc.IDNguoiDung != null)
                        {
                            var setNguoiBan = db.NguoiDungs.Find(danhmuc.IDNguoiDung);

                            setNguoiBan.Tien = Convert.ToInt32(setNguoiBan.Tien) + Convert.ToInt32(gia * (setNguoiBan.Phantramhoahong / 100.00));
                        }
                        db.SaveChanges();

                        var accRandomDamua = new AccRandomDamua();
                        accRandomDamua.IdAccRandom = setacc.IDAccRandom;
                        accRandomDamua.Danhmuc = setacc.DanhMuc.TenDanhMuc;
                        accRandomDamua.Ghichu = setacc.DanhMuc.GhiChuAcc;
                        accRandomDamua.Server = setacc.DanhMuc.Server;
                        db.AccRandomDamuas.Add(accRandomDamua);

                    }
                  
                }
                //Biến động số dư:
                var biendongds = new BienDongSoDu();
                biendongds.IDNguoiDung = nguoidung.IDNguoiDung;
                biendongds.SoTien = gia * soluong;
                biendongds.TruTien = true;
                biendongds.ThoiGian = DateTime.Now;
                biendongds.LoiNhan = "Mua " + soluong + " random (reroll)";
                biendongds.TienTruoc = tienTruoc;
                biendongds.TienSau = tienTruoc - (gia * soluong);
                db.BienDongSoDus.Add(biendongds);
                //
                //Biến động số dư admin:
                var admin = db.NguoiDungs.Where(m => m.LeverAdmin == 1).FirstOrDefault();
                if (admin != null)
                {
                    var tienTruocAdmin = admin.Tien;
                    var biendongdsad = new BienDongSoDu();
                    biendongdsad.IDNguoiDung = admin.IDNguoiDung;
                    var soTienCongAdmin = gia * soluong;
                    if (danhmuc.IDNguoiDung != null)
                    {
                        var NguoiBan = db.NguoiDungs.Find(danhmuc.IDNguoiDung);
                        var hhNguoiBan = Convert.ToInt32(gia * (Convert.ToInt32(NguoiBan.Phantramhoahong) / 100.00) * soluong);
                        soTienCongAdmin = (gia * soluong) - hhNguoiBan;
                    }
                    biendongdsad.SoTien = soTienCongAdmin;
                    biendongdsad.TruTien = false;
                    biendongdsad.ThoiGian = DateTime.Now;
                    biendongdsad.LoiNhan = "Khách mua " + soluong + " random (reroll)";
                    biendongdsad.TienTruoc = tienTruocAdmin;
                    var setSdmin = db.NguoiDungs.Find(admin.IDNguoiDung);
                    if (setSdmin != null)
                    {
                        setSdmin.Tien += soTienCongAdmin;
                    }
                    biendongdsad.TienSau = soTienCongAdmin + tienTruocAdmin;
                    db.BienDongSoDus.Add(biendongdsad);
                }



                //
                db.SaveChanges();
                return PartialView("_Rendermuataikhoan");
            }

        }

        public ActionResult ListDanhMucCollab(string id)
        {
           
            using (var db = new accgameEntities())
            {
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);

                var ND = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).FirstOrDefault();
                if (ND.CtvCollab == null)
                {
                    return Redirect("/");
                }
                var CtvCollab = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).Select(m => m.CtvCollab).FirstOrDefault();
                ViewBag.CtvCollab = CtvCollab;
                var CtvCollabCauHinh = db.CaiDats.Where(m => m.MaCaiDat == "is_ctv_collab").FirstOrDefault();
                ViewBag.CtvCollabCauHinh = CtvCollabCauHinh != null ? CtvCollabCauHinh.NoiDung : null;
                var listLoaiGame = db.LoaiGames.Where(m => m.Hide != true).OrderBy(m => m.STT).ToList();
                var OnOffCtvCollab = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).Select(m => m.OnOffCtvCollab).FirstOrDefault();
                ViewBag.OnOffCtvCollab = OnOffCtvCollab;
                HomeModels data = new HomeModels()
                {
                    listLoaiGame = listLoaiGame,
                    listLoaiAcc = db.LoaiAccs.Where(m => m.Hide != true).ToList(),
                };
                return View(data);
            }
        }


    }
}