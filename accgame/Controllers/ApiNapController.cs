using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;

namespace accgame.Controllers
{
    public class ApiNapController : Controller
    {
        
        [HttpPost]
        public ActionResult callBack(string so_tien, string ten_bank, string id_khach, string ten_khach, string sdt_khach, string ma_gd, string noi_dung, string ma_baoMat)
        {
            try
            {
                
                so_tien = so_tien.ToLower().Replace("vnd", "").Replace(",", "").Replace(".", "").Trim();
                int soTien = Convert.ToInt32(so_tien);
                int idUser = Convert.ToInt32(id_khach.Trim());
                using (var db = new accgameEntities())
                {
                    var key_web = db.CaiDats.Where(m => m.MaCaiDat == "key_web").FirstOrDefault();
                    if (key_web == null || ma_baoMat != key_web.NoiDung)
                    {
                        return Content("error");
                    }

                    var nguoiDung = db.NguoiDungs.Find(idUser);
                    int truocNap = Convert.ToInt32(nguoiDung.Tien);
                    nguoiDung.Tien = truocNap + soTien;
                    nguoiDung.TienNap = Convert.ToInt32(nguoiDung.TienNap) + soTien;
                    nguoiDung.TienNapThang = Convert.ToInt32(nguoiDung.TienNapThang) + soTien;

                    var napTien = new NapTien();
                    napTien.IDNguoiDung = idUser;
                    napTien.SoTien = soTien;
                    napTien.TruocNap = truocNap;
                    napTien.SauNap = nguoiDung.Tien;
                    napTien.thoigian = DateTime.Now;
                    if (ten_bank == "momo")
                    {
                        napTien.Noidung = "Nạp MOMO";
                    }
                    else
                    {
                        napTien.Noidung = "Nạp ATM";
                    }
                    napTien.Magd = ma_gd;

                    db.NapTiens.Add(napTien);
                    db.SaveChanges();

                    var biendongds = new BienDongSoDu();
                    biendongds.IDNguoiDung = idUser;
                    biendongds.SoTien = Convert.ToInt32(so_tien);
                    biendongds.TruTien = false;
                    if (ten_bank == "momo")
                    {
                        biendongds.LoiNhan = "Nạp MOMO";
                    }
                    else
                    {
                        biendongds.LoiNhan = "Nạp ATM";
                    }
                    biendongds.ThoiGian = DateTime.Now;
                    biendongds.TienTruoc = truocNap;
                    biendongds.TienSau = nguoiDung.Tien;
                    db.BienDongSoDus.Add(biendongds);
                    db.SaveChanges();
                }
                return Content("success");
            }
            catch
            {
                return Content("error");
            }
        }
    }
}