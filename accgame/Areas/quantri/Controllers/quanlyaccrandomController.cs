using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using accgame.Models;
using accgame.Filters;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    
    public class quanlyaccrandomController : Controller
    {
        // GET: quantri/quanlyaccrandom
        accgameEntities db = new accgameEntities();
        public ActionResult Index(int? page, string search)
        {
            int pageNumber = (page ?? 1);
            int newpage = pageNumber - 1;

            ViewBag.qlard = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen

            var data = db.Accs.Where(m => m.Xoa != true && m.DaBan != true && m.TaiKhoanNoiBo != true && m.RanDom == true).OrderByDescending(m => m.IDAcc).Skip(newpage * 40).Take(40).ToList();
            decimal count = db.Accs.Where(m => m.Xoa != true && m.DaBan != true && m.TaiKhoanNoiBo != true).Count();
            if (search != null)
            {
                data = db.Accs.Where(m => m.Xoa != true && m.TaiKhoanNoiBo != true && m.TenAcc.Contains(search) && m.DaBan != true && m.TaiKhoanNoiBo != true).OrderByDescending(m => m.IDAcc).Skip(newpage * 40).Take(40).ToList();
                count = db.Accs.Where(m => m.Xoa != true && m.TaiKhoanNoiBo != true && m.TenAcc.Contains(search) && m.DaBan != true && m.TaiKhoanNoiBo != true).Count();
            }
            int last_page = Convert.ToInt16(Math.Ceiling(count / 40));
            ViewBag.last_page = last_page;
            ViewBag.page = pageNumber;
            return View(data);
        }

        public ActionResult timkiem(string search)
        {
            ViewBag.qlard = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen

            var data = db.Accs.Where(m => m.RanDom == true && m.Xoa != true && m.TaiKhoanNoiBo != true && (m.MaTaiKhoan.Contains(search) || m.TaiKhoan.Contains(search) || m.NguoiDung.TenNguoiDung.Contains(search)) && m.DaBan != true && m.TaiKhoanNoiBo != true).OrderByDescending(m => m.IDAcc).Take(40).ToList();

            return View(data);
        }

        public ActionResult themmoi()
        {
            ViewBag.qlard = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            HomeModels data = new HomeModels()
            {
                listNhanVat = db.NhanVats.Where(m => m.LoaiGame == 1).ToList(),
                listVuKhi = db.VuKhis.ToList()
            };
            return View(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult themmoi([Bind(Include = "TenAcc, LoaiGame, Server, TaiKhoan, MatKhau, Gia, ChiTiet, ThongTinKhac, CapKhaiPha, AccVip, AccKhoiDau")] Acc acc, string[] DSNhanVat, string[] DSVuKhi, HttpPostedFileBase ImageUpload, HttpPostedFileBase[] ImageUploads)
        {
            ViewBag.qlard = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            if (acc.Gia < 0)
            {
                acc.Gia = 0;
            }
            var searchAcc = db.Accs.OrderByDescending(m => m.IDAcc).FirstOrDefault();
            if (searchAcc == null)
            {
                if (acc.LoaiGame == 1)
                {
                    acc.MaTaiKhoan = "HK_" + 1000;
                }
                else
                {
                    acc.MaTaiKhoan = "GS_" + 1000;
                }
            }
            else
            {
                int ma = 1000 + searchAcc.IDAcc + 1;
                if (acc.LoaiGame == 1)
                {
                    acc.MaTaiKhoan = "HK_Random_" + ma;
                }
                else
                {
                    acc.MaTaiKhoan = "GS_Random" + ma;
                }
            }
            var filename = "Anhacc_" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmssfff")) + Path.GetExtension(ImageUpload.FileName);
            var SavePath = Path.Combine(Server.MapPath("/Content/images/") + filename);
            ImageUpload.SaveAs(SavePath);
            acc.AnhDaiDien = filename;

            acc.IDNguoiDung = idnd;
            acc.ThoiGianDang = DateTime.Now;
            acc.RanDom = true;
            db.Accs.Add(acc);
            db.SaveChanges();
            int i = 0;
            foreach (HttpPostedFileBase imgUL in ImageUploads)
            {
                if (imgUL != null)
                {
                    Anh_Acc anhAcc = new Anh_Acc();
                    var fn = "Anhacc_" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmssfff")) + i + Path.GetExtension(imgUL.FileName);
                    var SP = Path.Combine(Server.MapPath("/Content/images/") + fn);
                    imgUL.SaveAs(SP);
                    anhAcc.DuongDanAnh = fn;
                    anhAcc.IDAcc = acc.IDAcc;
                    db.Anh_Acc.Add(anhAcc);
                    i++;
                }
            }
            db.SaveChanges();
            return RedirectToAction("index");
        }

        public ActionResult xoaanh(int id)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            var anh_acc = db.Anh_Acc.Find(id);
            db.Anh_Acc.Remove(anh_acc);
            db.SaveChanges();
            return Json(new { Message = "Thành công", JsonRequestBehavior.AllowGet });
        }

        public ActionResult sua(int id)
        {
            ViewBag.qlard = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            HomeModels data = new HomeModels()
            {
                listNhanVat = db.NhanVats.ToList(),
                listVuKhi = db.VuKhis.ToList(),
                acc = db.Accs.Where(m => m.IDAcc == id).FirstOrDefault(),
                listAnh_Acc = db.Anh_Acc.Where(m => m.IDAcc == id).ToList()
            };
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult sua(int id, [Bind(Include = "TenAcc, LoaiGame, Server, TaiKhoan, MatKhau, Gia, ChiTiet, ThongTinKhac, CapKhaiPha, AccVip, AccKhoiDau")] Acc acc, string[] DSNhanVat, string[] DSVuKhi, HttpPostedFileBase ImageUpload, HttpPostedFileBase[] ImageUploads)
        {
            ViewBag.qlard = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            if (acc.Gia < 0)
            {
                acc.Gia = 0;
            }
            var setAcc = db.Accs.Find(id);
            setAcc.TenAcc = acc.TenAcc;
            setAcc.LoaiGame = acc.LoaiGame;
            setAcc.Server = acc.Server;
            setAcc.TaiKhoan = acc.TaiKhoan;
            setAcc.MatKhau = acc.MatKhau;
            setAcc.Gia = acc.Gia;
            setAcc.ChiTiet = acc.ChiTiet;
            setAcc.ThongTinKhac = acc.ThongTinKhac;

            if (ImageUpload != null)
            {
                var filename = "Anhacc_" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmssfff")) + Path.GetExtension(ImageUpload.FileName);
                var SavePath = Path.Combine(Server.MapPath("/Content/images/") + filename);
                ImageUpload.SaveAs(SavePath);
                setAcc.AnhDaiDien = filename;
                db.SaveChanges();
            }

            int i = 0;
            foreach (HttpPostedFileBase imgUL in ImageUploads)
            {
                if (imgUL != null)
                {
                    Anh_Acc anhAcc = new Anh_Acc();
                    var fn = "Anhacc_" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmssfff")) + i + Path.GetExtension(imgUL.FileName);
                    var SP = Path.Combine(Server.MapPath("/Content/images/") + fn);
                    imgUL.SaveAs(SP);
                    anhAcc.DuongDanAnh = fn;
                    anhAcc.IDAcc = setAcc.IDAcc;
                    db.Anh_Acc.Add(anhAcc);
                }
                i++;
            }
            db.SaveChanges();
            return RedirectToAction("index");
        }

        public ActionResult taikhoandaban(int? page, string search)
        {
            int pageNumber = (page ?? 1);
            int newpage = pageNumber - 1;

            ViewBag.qlard = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen

            var data = db.Accs.Where(m => m.RanDom == true && m.Xoa != true && m.DaBan == true).OrderByDescending(m => m.ThoiGianBan).ThenByDescending(m => m.IDAcc).Skip(newpage * 40).Take(40).ToList();
            decimal count = db.Accs.Where(m => m.RanDom == true && m.Xoa != true && m.DaBan == true).Count();
            if (search != null)
            {
                data = db.Accs.Where(m => m.RanDom == true && m.Xoa != true && m.DaBan == true && m.TenAcc.Contains(search)).OrderByDescending(m => m.ThoiGianBan).ThenByDescending(m => m.IDAcc).Skip(newpage * 40).Take(40).ToList();
                count = db.Accs.Where(m => m.RanDom == true && m.Xoa != true && m.DaBan == true && m.TenAcc.Contains(search)).Count();
            }
            int last_page = Convert.ToInt16(Math.Ceiling(count / 40));
            ViewBag.last_page = last_page;
            ViewBag.page = pageNumber;
            return View(data);
        }


        public ActionResult xoa(int id)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            var acc = db.Accs.Find(id);
            acc.Xoa = true;
            db.SaveChanges();
            return RedirectToAction("index");
        }
    }
}