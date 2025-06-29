using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.HtmlControls;
using accgame.Context;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Data.Entity;

namespace accgame.Controllers
{
    public class ApiController : Controller
    {
        // GET: ApiAcc
        public ActionResult getAcc()
        {
            using (var db = new accgameEntities())
            {
                var accList = db.Accs
                    .Where(m => m.IDNguoiDung == 684 && m.AccVip == true && m.DaBan != true
                            && m.AccKhoiDau != true && m.Xoa != true && m.TaiKhoanNoiBo != true
                            && (m.PhanTram != null && m.PhanTram > 0))
                    .Select(m => new
                    {
                        IDAcc = m.IDAcc,
                        CapKhaiPha = m.CapKhaiPha,
                        Gia = m.PhanTram,
                        Server = m.Server,
                        MaTaiKhoan = m.MaTaiKhoan,
                        ChiTiet = m.ChiTiet,
                        NhanVat = m.NhanVat,
                        VuKhi = m.VuKhi,
                        LoaiGame = m.LoaiGame == 1 ? "honkai star rail" : "genshin impact",
                        AnhDaiDien = m.AnhDaiDien,
                        Anh_Acc = m.Anh_Acc.Select(a => new
                        {
                            DuongDanAnh = a.DuongDanAnh,
                        }).ToList()
                    })
                    .ToList();

                List<object> accObjects = new List<object>();

                foreach (var item in accList)
                {
                    var acc = new
                    {
                        ID = item.IDAcc,
                        Gia = item.Gia,
                        Server = item.Server,
                        MaTaiKhoan = item.MaTaiKhoan,
                        ChiTiet = item.ChiTiet,
                        NhanVat = item.NhanVat,
                        VuKhi = item.VuKhi,
                        LoaiGame = item.LoaiGame,
                        AnhDaiDien = item.AnhDaiDien,
                        CapKhaiPha = item.CapKhaiPha,
                        Anh_Acc = item.Anh_Acc.ToList(), // Truy cập vào danh sách Anh_Acc từ mỗi Acc
                    };
                    accObjects.Add(acc);
                }

                return Json(accObjects, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult buyAcc (int idAcc, string key, string website) 
        { 
            
            using (var db = new accgameEntities())
            {
                var settingApi = db.SettingApis.Where(m => m.ApiKey == key && m.DomainWebsite == website).FirstOrDefault();
                if (settingApi == null)
                {
                    return Json(new { message = "Invalid key" });
                }
                var checkAcc = db.Accs.Where(m => m.IDNguoiDung == 684 && m.AccVip == true && m.DaBan != true && m.AccKhoiDau != true && m.Xoa != true && m.TaiKhoanNoiBo != true && m.IDAcc == idAcc && (m.PhanTram != null && m.PhanTram > 0)).FirstOrDefault();
                if (checkAcc == null)
                {
                    return Json(new { message = "This account was not found" });
                }
                var setAcc = db.Accs.Find(checkAcc.IDAcc);
                setAcc.DaBan = true;
                setAcc.IDNguoiMua = 684;
                setAcc.ThoiGianBan = DateTime.Now;
                db.SaveChanges();
                var setApiAcc = new ApiAcc();
                setApiAcc.IDAcc = idAcc;
                setApiAcc.Website = website;
                setApiAcc.Name = checkAcc.TenAcc;
                setApiAcc.Price = checkAcc.PhanTram;
                setApiAcc.Time = DateTime.Now;
                db.ApiAccs.Add(setApiAcc);
                db.SaveChanges();

                var info = new
                {
                    ID = setAcc.IDAcc,
                    TenAcc = setAcc.TenAcc,
                    Server = setAcc.Server,
                    MaTaiKhoan = setAcc.MaTaiKhoan,
                    ChiTiet = setAcc.ChiTiet,
                    Gia = setAcc.PhanTram,
                    TaiKhoan = setAcc.TaiKhoan,
                    MatKhau = setAcc.MatKhau,
                    ThongTinKhac = setAcc.ThongTinKhac,
                    LoaiGame = setAcc.LoaiGame == 1 ? "honkai star rail" : "genshin impact",
                };
                return Json(new { message = "success", info });
            }
        }
    }
}
