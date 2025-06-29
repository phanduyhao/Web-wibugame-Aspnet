using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using accgame.Filters;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    
    public class quanlygoinapgameController : Controller
    {
        private accgameEntities db = new accgameEntities();

        // GET: quantri/quanlygoinapgame
        public ActionResult Index()
        {
            ViewBag.qlgng = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }

            //phan quyen
            return View(db.GoiNaps.Where(m => m.Xoa != true).Include(m => m.LoaiGame1).OrderBy(m => m.LoaiGame).ThenBy(m => m.SoThuTu).ToList());
        }
        public ActionResult Create()
        {
            ViewBag.qlgnv = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            var loaiGame = db.LoaiGames.ToList();
            ViewBag.loaiGame = loaiGame;
            return View();
        }

        // POST: quantri/quanlygoinhiemvu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDGoiNap, TenGoi, Gia, SoThuTu, LoaiGame, LoaiNap, X2LanDau, ThoiGianAn")] GoiNap goiNap, HttpPostedFileBase Anh, string ImageUploadLink)
        {
            if (ModelState.IsValid)
            {
                // Xử lý ảnh
                if (Anh != null && Anh.ContentLength > 0)
                {
                    // Lấy tên file và lưu vào thư mục
                    var fileName = Path.GetFileName(Anh.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/GoiNapGame/"), fileName);
                    Anh.SaveAs(path);

                    // Lưu đường dẫn ảnh vào cơ sở dữ liệu
                    goiNap.Anh = "/Content/Images/GoiNapGame/" + fileName;
                }
                else if (!string.IsNullOrEmpty(ImageUploadLink))
                {
                    // Lưu URL ảnh từ liên kết
                    goiNap.Anh = ImageUploadLink;
                }
                // Nếu không có file hoặc link ảnh, giữ nguyên ảnh cũ
                else
                {
                    // Không thay đổi giá trị của AnhDanhMuc
                }
                db.GoiNaps.Add(goiNap);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var loaiGame = db.LoaiGames.ToList();
            ViewBag.loaiGame = loaiGame;
            return View(goiNap);
        }


        // GET: quantri/quanlygoinhiemvu/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GoiNap goiNap = db.GoiNaps.Find(id);
            if (goiNap == null)
            {
                return HttpNotFound();
            }
            var loaiGame = db.LoaiGames.ToList();
            ViewBag.loaiGame = loaiGame;

            return View(goiNap);
        }

        // POST: quantri/quanlygoinhiemvu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDGoiNap, TenGoi, Gia, SoThuTu, LoaiGame, LoaiNap, Anh, X2LanDau, ThoiGianAn")] GoiNap goiNap, HttpPostedFileBase Anh, string ImageUploadLink)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem Gói Nạp đã tồn tại trong cơ sở dữ liệu chưa
                var existingGoiNap = db.GoiNaps.Find(goiNap.IDGoiNap);

                if (existingGoiNap != null)
                {
                    // Xử lý ảnh nếu có ảnh mới được tải lên
                    if (Anh != null && Anh.ContentLength > 0)
                    {
                        // Lấy tên file và lưu vào thư mục
                        var fileName = Path.GetFileName(Anh.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/Images/GoiNapGame/"), fileName);
                        Anh.SaveAs(path);

                        // Lưu đường dẫn ảnh vào cơ sở dữ liệu
                        existingGoiNap.Anh = "/Content/Images/GoiNapGame/" + fileName;
                    }
                    else if (!string.IsNullOrEmpty(ImageUploadLink))
                    {
                        // Lưu URL ảnh từ liên kết
                        existingGoiNap.Anh = ImageUploadLink;
                    }
                    else
                    {
                        // Nếu không có ảnh mới, giữ nguyên ảnh cũ (không thay đổi)
                        existingGoiNap.Anh = existingGoiNap.Anh;
                    }

                    // Cập nhật các trường còn lại của gói nạp
                    existingGoiNap.TenGoi = goiNap.TenGoi;
                    existingGoiNap.ThoigianAn = goiNap.ThoigianAn;
                    existingGoiNap.Gia = goiNap.Gia;
                    existingGoiNap.SoThuTu = goiNap.SoThuTu;
                    existingGoiNap.LoaiGame = goiNap.LoaiGame;
                    existingGoiNap.LoaiNap = goiNap.LoaiNap;
                    existingGoiNap.X2LanDau = goiNap.X2LanDau;
                    // Đánh dấu đối tượng này là đã được sửa đổi
                    db.Entry(existingGoiNap).State = EntityState.Modified;
                }
                else
                {
                    // Nếu gói nạp không tồn tại, thêm mới
                    db.GoiNaps.Add(goiNap);
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();

                // Redirect đến trang Index sau khi lưu thành công
                return RedirectToAction("Index");
            }

            // Lấy dữ liệu các loại game để hiển thị trong View
            var loaiGame = db.LoaiGames.ToList();
            ViewBag.loaiGame = loaiGame;

            // Trả về View để người dùng chỉnh sửa lại
            return View(goiNap);
        }


        // GET: quantri/quanlygoinhiemvu/Delete/5
        public ActionResult Delete(int? id)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new accgameEntities())
            {
                GoiNap goiNap = db.GoiNaps.Find(id);
                if (goiNap == null)
                {
                    return HttpNotFound();
                }

                goiNap.Xoa = true;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public JsonResult angoinap(int id, bool hide)
        {
            using (var db = new accgameEntities())
            {
                // Tìm danh mục theo ID
                var danhMuc = db.GoiNaps.Find(id);
                if (danhMuc == null)
                {
                    return Json(new { message = "Danh mục không tồn tại!" });
                }

                // Cập nhật giá trị Hide
                danhMuc.An = hide;

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();

                // Trả về thông báo thành công
                return Json(new { message = "Cập nhật trạng thái thành công!" });
            }
        }


        [AllowAnonymous]
        public ActionResult CheckAutoHide()
        {
            using (var db = new accgameEntities())
            {
                var now = DateTime.Now;

                var goiCanAn = db.GoiNaps
                    .Where(g => g.ThoigianAn != null && g.An != true && g.ThoigianAn <= now)
                    .ToList();

                foreach (var g in goiCanAn)
                {
                    g.An = true;
                    db.Entry(g).State = EntityState.Modified;
                }

                db.SaveChanges();

                return Content("Đã kiểm tra và cập nhật trạng thái ẩn gói nạp.");
            }
               
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
