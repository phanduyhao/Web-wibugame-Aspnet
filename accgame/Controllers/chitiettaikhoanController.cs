using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using accgame.Models;
using accgame.Controllers;
using System.Data.Entity;
using accgame.Areas.quantri.Controllers;
using System.Collections;
using accgame.Filters;

namespace accgame.Controllers
{
    public class chitiettaikhoanController : Controller
    {
        [NotifyUserFilter]

        // GET: chitiettaikhoan
        public ActionResult xem(string id)
        {
            using (var db = new accgameEntities())
            {
                string maso = id;
                var getAcc = db.Accs.Include(m => m.LoaiGame1).Where(m => m.MaTaiKhoan == maso).FirstOrDefault();
                HomeModels data = new HomeModels()
                {
                    acc = getAcc,
                    listAnh_Acc = db.Anh_Acc.Where(m => m.IDAcc == getAcc.IDAcc).ToList(),
                };
                Session["url"] = "/chitiettaikhoan/xem/" + getAcc.MaTaiKhoan;
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

        [CheckSessionID]

        public ActionResult mua(int id, string code)
        {
            using (var db = new accgameEntities())
            {

                ViewBag.thongbao = 0;
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                var nd = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).FirstOrDefault();

                if (nd.Block == true)
                {
                    ViewBag.thongbao = 10;
                    return PartialView("_RenderMuaTaiKhoanThuong");
                }
                var CtvCollab = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).Select(m => m.CtvCollab).FirstOrDefault();
                var OnOffCtvCollab = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).Select(m => m.OnOffCtvCollab).FirstOrDefault();
                var CtvCollabCauHinh = db.CaiDats.Where(m => m.MaCaiDat == "is_ctv_collab").FirstOrDefault();
                if (idnd == 0)
                {
                    ViewBag.thongbao = 1;
                    return PartialView("_RenderMuaTaiKhoanThuong");
                }
                var acc = db.Accs.Find(id);

                var accgia = Convert.ToInt32(acc.Gia);
                var giactvcollab = accgia / 100 * Convert.ToInt32(acc.GiaCtvCollab);

                if (CtvCollabCauHinh.NoiDung != null && CtvCollab == true && OnOffCtvCollab == true)
                {
                    if (acc.GiaCtvCollab != null)
                    {
                        accgia = giactvcollab;
                    }
                    else
                    {
                        accgia = Convert.ToInt32(acc.Gia);
                    }
                }
                else
                {
                    accgia = Convert.ToInt32(acc.Gia);
                }
                int giaTien = accgia;
                if (acc == null)
                {
                    ViewBag.thongbao = 2;

                    return PartialView("_RenderMuaTaiKhoanThuong");

                }
                if (acc.DaBan == true)
                {
                    ViewBag.thongbao = 3;
                    return PartialView("_RenderMuaTaiKhoanThuong");
                }
                if (acc.Xoa == true)
                {
                    ViewBag.thongbao = 4;
                    return PartialView("_RenderMuaTaiKhoanThuong");
                }
                var nguoidung = db.NguoiDungs.Find(idnd);
                var TienTruocNd = Convert.ToInt32(nguoidung.Tien);
                if (acc.TaiKhoanNoiBo == true)
                {
                    if (nguoidung.LeverAdmin != 1 && nguoidung.LeverAdmin != 2 && nguoidung.LeverAdmin != 3)
                    {
                        ViewBag.thongbao = 6;
                        return PartialView("_RenderMuaTaiKhoanThuong");
                    }
                }

                if (Convert.ToInt32(nguoidung.Tien) < giaTien)
                {
                    ViewBag.tien = Convert.ToInt32(nguoidung.Tien);
                    ViewBag.thongbao = 5;
                    return PartialView("_RenderMuaTaiKhoanThuong");
                }
                if (code != null && code != "" && acc.MaGiamGia != null)
                {
                    if (code == acc.MaGiamGia)
                    {
                        giaTien = accgia - Convert.ToInt32(acc.SoTienGiam);
                    }
                    else
                    {
                        ViewBag.thongbao = 7;
                        return PartialView("_RenderMuaTaiKhoanThuong");
                    }
                }

                nguoidung.Tien = TienTruocNd - giaTien;
                acc.DaBan = true;
                acc.IDNguoiMua = idnd;
                acc.ThoiGianBan = DateTime.Now;
                acc.GiaDaMua = giaTien;
                db.SaveChanges();

                // Tạo thông báo HTML
                string noiDungThongBao = $@"
                        Bạn đã mua thành công ACC <strong>{acc.MaTaiKhoan}</strong>. 
                        <a href='/taikhoandamua/taikhoanthuong' class='text-info' target='_blank'>Click để xem thông tin acc</a>.
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
                var ctv = db.NguoiDungs.Where(m => m.IDNguoiDung == acc.IDNguoiDung).FirstOrDefault();
                var tienTruocCtv = ctv.Tien;
                var setCTV = db.NguoiDungs.Find(ctv.IDNguoiDung);
                /*int tiencongCTV = Convert.ToInt32(setCTV.Phantramhoahong / 100.00 * giaTien);*/
                int phanTram = acc.PhanTram ?? 0;
                /*int GiaBanDau;
                if (acc.GiaGoc.HasValue)
                {
                     GiaBanDau = acc.GiaGoc.Value - (acc.GiaGoc.Value * phanTram / 100);
                }
                else
                {
                     GiaBanDau = 0; // Hoặc một giá trị mặc định phù hợp
                }*/
                int tiencongCTV = Convert.ToInt32(setCTV.Phantramhoahong / 100.00 * (acc.GiaGoc - (acc.GiaGoc * phanTram / 100)));
                setCTV.Tien = Convert.ToInt32(setCTV.Tien) + tiencongCTV;

                var loaiAcc = db.LoaiAccs.Where(m => m.IDLoaiAcc == acc.IDLoaiAcc).FirstOrDefault();
                if (loaiAcc != null)
                {
                    var setLoaiAcc = db.LoaiAccs.Find(loaiAcc.IDLoaiAcc);
                    if (setLoaiAcc != null)
                    {
                        setLoaiAcc.DaBan += 1;
                        setLoaiAcc.DangBan -= 1;
                        db.SaveChanges();
                    }
                }

                //Biến động số dư:

                if (ctv != null)
                {
                    var biendongds = new BienDongSoDu();
                    biendongds.IDNguoiDung = ctv.IDNguoiDung;
                    biendongds.SoTien = tiencongCTV;
                    biendongds.TruTien = false;
                    biendongds.ThoiGian = DateTime.Now;
                    biendongds.LoiNhan = "Bán acc";
                    biendongds.TienTruoc = tienTruocCtv;
                    biendongds.TienSau = setCTV.Tien;
                    db.BienDongSoDus.Add(biendongds);
                }

                //
                //Biến động số dư nguoi mua:
                var biendongdsnm = new BienDongSoDu();
                biendongdsnm.IDNguoiDung = idnd;
                biendongdsnm.SoTien = giaTien;
                biendongdsnm.TruTien = true;
                biendongdsnm.ThoiGian = DateTime.Now;
                biendongdsnm.LoiNhan = "Mua acc";
                biendongdsnm.TienTruoc = TienTruocNd;
                biendongdsnm.TienSau = nguoidung.Tien;
                db.BienDongSoDus.Add(biendongdsnm);
                //
                //Biến động số dư admin:
                var admin = db.NguoiDungs.Where(m => m.LeverAdmin == 1).FirstOrDefault();
                if (admin != null)
                {
                    var adminTienTruoc = Convert.ToInt32(admin.Tien);
                    var biendongdsad = new BienDongSoDu();
                    biendongdsad.IDNguoiDung = admin.IDNguoiDung;
                    biendongdsad.SoTien = giaTien - tiencongCTV;
                    biendongdsad.TruTien = false;
                    biendongdsad.ThoiGian = DateTime.Now;
                    biendongdsad.LoiNhan = "Hoa hồng bán acc";
                    biendongdsad.TienTruoc = adminTienTruoc;
                    var setAdmin = db.NguoiDungs.Find(admin.IDNguoiDung);
                    if (setAdmin != null)
                    {
                        setAdmin.Tien += (giaTien - tiencongCTV);
                    }
                    biendongdsad.TienSau = setAdmin.Tien;
                    db.BienDongSoDus.Add(biendongdsad);

                }

                //
                db.SaveChanges();

                try
                {
                    var caidat = db.CaiDats;
                    var tenmien = caidat.Where(m => m.MaCaiDat == "ten_mien").FirstOrDefault();
                    var getTenmien = tenmien?.NoiDung ?? "";

                    string toEmail = Convert.ToString(acc.NguoiDung.Email);
                    string subject = "Tài khoản " + acc.TenAcc + " đã được mua!";
                    /*                    string body = "Tài khoản <strong>" + acc.TenAcc + "</strong> đã được mua! vào lúc " + acc.ThoiGianBan + " với giá " + acc.Gia + "VNĐ, bạn nhận được " + tiencongCTV + "VNĐ hãy vào ngay <a href=\"https://" + getTenmien + "\">https://" + getTenmien + "</a> để xem thông tin!";
                    */
                    string body = "Bạn bán được tài khoản có mã: ' " + acc.MaTaiKhoan + "',  tên TK ' <strong>" + acc.TenAcc + "</strong> ' " + " với giá ' " + accgia + "VNĐ ', bạn nhận được ' " + tiencongCTV + "VNĐ. '";

                    var service = new EmailService();
                    bool kq = service.Send(toEmail, subject, body);
                }
                catch (Exception ex)
                {
                    // Console.WriteLine("An error occurred: " + ex.Message);
                }
                return PartialView("_RenderMuaTaiKhoanThuong");
            }

        }
    }
}