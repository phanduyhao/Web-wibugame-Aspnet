using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.UI;
using accgame.Context;
using accgame.Filters;
using PagedList;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    

    public class quanlynguoidungController : Controller
    {
        private accgameEntities db = new accgameEntities();

        // GET: quantri/quanlynguoidung
        public ActionResult Index(int? page, string search, int? ctv)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);

            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }

            int pageNumber = page ?? 1;
            int newpage = pageNumber - 1;
            int itemsPerPage = 40;
            ViewBag.search = search;
            ViewBag.ctvt = ctv;

            IQueryable<NguoiDung> query = db.NguoiDungs; // Bắt đầu truy vấn

            if (ctv == 1) // Lọc Cộng Tác Viên
            {
                ViewBag.ctv = "active";
                query = query.Where(m => m.LeverAdmin != 0 && m.LeverAdmin != null);
            }
            else if (!string.IsNullOrEmpty(search)) // Lọc theo tìm kiếm
            {
                ViewBag.nguoidung = "active";
                query = query.Where(m => m.TenNguoiDung.Contains(search) || m.Email.Contains(search) || m.IDNguoiDung.ToString().Contains(search));
            }
            else
            {
                ViewBag.nguoidung = "active";
            }

            // Lấy số lượng user để tính số trang
            int count = query.Count();
            int last_page = (int)Math.Ceiling((double)count / itemsPerPage);
            ViewBag.last_page = last_page;
            ViewBag.page = pageNumber;

            // Phân trang và lấy dữ liệu
            var users = query.OrderByDescending(m => m.Tien).Skip(newpage * itemsPerPage).Take(itemsPerPage).ToList();

            // Lấy danh sách ẩn Acc Vip
            ViewBag.AnAccVipDict = db.AnAccs.ToDictionary(a => a.IdUser, a => a.AnAccVip);

            return View(users);
        }


        // GET: quantri/quanlynguoidung/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.nguoidung = "active";

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
            NguoiDung nguoiDung = db.NguoiDungs.Find(id);
            var levelND = nguoiDung.LeverAdmin;
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }
            var accMua = db.Accs.Where(m => m.IDNguoiMua == id).ToList().Count();
            ViewBag.accMua = accMua;
            var accRdMua = db.AccRandoms.Where(m => m.IDNguoiMua == id).ToList().Count();
            ViewBag.accRdMua = accRdMua;
            var LuotCayThua = db.CayThues.Where(m => m.IDNguoiDung == id).ToList().Count();
            ViewBag.LuotCayThua = LuotCayThua;
            var LuotNapGame = db.NapGames.Where(m => m.IDNguoiDung == id).ToList().Count();
            ViewBag.LuotNapGame = LuotNapGame;
            ViewBag.levelND = levelND;
            ViewBag.mkc2 = nguoiDung.MatKhau2;
            return View(nguoiDung);
        }

        // POST: quantri/quanlynguoidung/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDNguoiDung,TenNguoiDung,MatKhau,Email,Tien,NgayThamGia,LeverAdmin,MatMa,ThoiVang,TienNap,HoaHong,TienNapThang,Phantramhoahong, CtvCollab, MatKhau2, Block, BlockContent")] NguoiDung nguoiDung)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            NguoiDung setNguoiDung = db.NguoiDungs.Find(nguoiDung.IDNguoiDung);
            setNguoiDung.TenNguoiDung = nguoiDung.TenNguoiDung;

            if (nguoiDung.MatKhau != null)
            {
                setNguoiDung.MatKhau = nguoiDung.MatKhau;
                setNguoiDung.SessionID_Mobile = Guid.NewGuid().ToString();
                setNguoiDung.SessionID_Desktop = Guid.NewGuid().ToString();
            }
            setNguoiDung.Email = nguoiDung.Email;
            setNguoiDung.Tien = nguoiDung.Tien;
            setNguoiDung.LeverAdmin = nguoiDung.LeverAdmin;
            setNguoiDung.TienNap = nguoiDung.TienNap;
            setNguoiDung.Phantramhoahong = nguoiDung.Phantramhoahong;
            setNguoiDung.TienNapThang = nguoiDung.TienNapThang;
            setNguoiDung.CtvCollab = nguoiDung.CtvCollab;
            if(nguoiDung.MatKhau2 != null)
            {
                setNguoiDung.MatKhau2 = nguoiDung.MatKhau2;
            }
            setNguoiDung.Block = nguoiDung.Block;
            setNguoiDung.BlockContent = nguoiDung.Block.HasValue && nguoiDung.Block.Value ? nguoiDung.BlockContent : null;
            db.SaveChanges();

            var accMua = db.Accs.Where(m => m.IDNguoiMua == nguoiDung.IDNguoiDung).ToList().Count();
            ViewBag.accMua = accMua;
            var accRdMua = db.AccRandoms.Where(m => m.IDNguoiMua == nguoiDung.IDNguoiDung).ToList().Count();
            ViewBag.accRdMua = accRdMua;
            var LuotCayThua = db.CayThues.Where(m => m.IDNguoiDung == nguoiDung.IDNguoiDung).ToList().Count();
            ViewBag.LuotCayThua = LuotCayThua;
            var LuotNapGame = db.NapGames.Where(m => m.IDNguoiDung == nguoiDung.IDNguoiDung).ToList().Count();
            ViewBag.LuotNapGame = LuotNapGame;
            return View(nguoiDung);
        }

        [HttpPost]
        public JsonResult CheckMatKhau2(string matKhau2Current)
        {
            // Lấy ID người dùng từ Session
            int userId = Convert.ToInt32(Session["IDNguoiDung"]);
            var user = db.NguoiDungs.Find(userId);

            // Kiểm tra mật khẩu cấp 2 hiện tại có đúng không
            if (user != null && user.MatKhau2 == matKhau2Current) // Kiểm tra mật khẩu cấp 2
            {
                return Json(new { isValid = true });
            }
            else
            {
                return Json(new { isValid = false });
            }
        }


        [HttpPost]
        public ActionResult AdminCongTien(int id, int sotien, string lido, bool congtien)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Json(new { message = "error" });
            }
            //phan quyen
            var nguoidung = db.NguoiDungs.Find(id);
            if (nguoidung == null)
            {
                return Json(new { message = "Người dùng không tồn tại" });
            }
            var layThoiGian = DateTime.Now;
            var tienNguoiDung = Convert.ToInt32(nguoidung.Tien);
            if (congtien == true)
            {
                nguoidung.Tien = tienNguoiDung + sotien;
            }
            else
            {
                nguoidung.Tien = tienNguoiDung - sotien;
            }
            var bienDongSoDu = new BienDongSoDu();
            bienDongSoDu.SoTien = sotien;
            bienDongSoDu.LoiNhan = congtien == true ? "Admin cộng tiền" : "Admin trừ tiền";
            bienDongSoDu.ThoiGian = layThoiGian;
            bienDongSoDu.TruTien = !congtien;
            bienDongSoDu.IDNguoiDung = id;
            bienDongSoDu.TienTruoc = tienNguoiDung;
            if (congtien == true)
            {
                bienDongSoDu.TienSau = tienNguoiDung + sotien;
            }
            else
            {
                bienDongSoDu.TienSau = tienNguoiDung - sotien;
            }
            db.BienDongSoDus.Add(bienDongSoDu);

            var adminCongTien = new AdminCongTien();
            adminCongTien.SoTien = sotien;
            adminCongTien.IDNguoiCong = idnd;
            adminCongTien.IDNguoiDung = id;
            adminCongTien.Lydo = lido;
            adminCongTien.ThoiGian = layThoiGian;
            adminCongTien.CongTien = congtien;
            adminCongTien.Status = true;
            adminCongTien.SoDuTruocCong = tienNguoiDung;
            if (congtien == true)
            {
                adminCongTien.SoDuSauCong = tienNguoiDung + sotien;
            }
            else
            {
                adminCongTien.SoDuSauCong = tienNguoiDung - sotien;
            }
            db.AdminCongTiens.Add(adminCongTien);
            db.SaveChanges();
            return Json(new { message = "success" });
        }

        public ActionResult BienDongSoDu(int id, int start = 0, int length = 10, string search = "", int draw = 0, string sortColumn = "IDBienDongSoDu", string sortDirection = "asc")
        {
            IQueryable<BienDongSoDu> bienDongSoDuQuery = db.BienDongSoDus.Where(m => m.IDNguoiDung == id);

            if (!string.IsNullOrEmpty(search))
            {
                bienDongSoDuQuery = bienDongSoDuQuery.Where(m => m.LoiNhan.Contains(search));
            }

            // Default sort order
            bienDongSoDuQuery = sortDirection == "asc" ? bienDongSoDuQuery.OrderBy(m => m.IDBienDongSoDu) : bienDongSoDuQuery.OrderByDescending(m => m.IDBienDongSoDu);

            int totalRecords = bienDongSoDuQuery.Count();
            int recordsFiltered = totalRecords; // Update this if there's actual filtering logic

            // Ensure sorting before Skip and Take
            if (!string.IsNullOrEmpty(sortColumn))
            {
                bienDongSoDuQuery = sortDirection == "asc"
                    ? bienDongSoDuQuery.OrderBy(m => m.IDBienDongSoDu)
                    : bienDongSoDuQuery.OrderBy(m => m.IDBienDongSoDu);
            }

            var data = bienDongSoDuQuery.Select(m => new
            {
                IDBienDongSoDu = m.IDBienDongSoDu,
                SoTien = m.SoTien,
                LoiNhan = m.LoiNhan,
                TienTruoc = m.TienTruoc,
                TienSau = m.TienSau,
                ThoiGian = m.ThoiGian
            }).OrderByDescending(m => m.IDBienDongSoDu).Skip(start).Take(length).ToList();

            var result = new
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = recordsFiltered,
                data = data
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListCongTien(int? page, string search, int? rows)
        {
            ViewBag.nguoidung = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1 && level != 3) || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            ViewBag.search = search;
            int pageSize = rows ?? 10; 
            int pageNumber = (page ?? 1);
            var data = db.AdminCongTiens.OrderByDescending(m => m.IdAdminCongTien).Include(m => m.NguoiDung).Include(m => m.NguoiDung1).ToPagedList(pageNumber, pageSize);

            if (search != null & search != "")
            {
                data = db.AdminCongTiens.Where(m => m.Lydo.Contains(search) || m.NguoiDung.TenNguoiDung.Contains(search) || m.NguoiDung1.TenNguoiDung.Contains(search)).OrderByDescending(m => m.IdAdminCongTien).Include(m => m.NguoiDung).Include(m => m.NguoiDung1).ToPagedList(pageNumber, pageSize);

            }
            ViewBag.Rows = pageSize;

            return View(data);
        }

        [HttpPost]
        public ActionResult PostCongTien(int id)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Json(new { message = "error" });
            }
            //phan quyen
            var adminCongTien = db.AdminCongTiens.Where(m => m.IdAdminCongTien == id).FirstOrDefault();
            if (adminCongTien == null || adminCongTien.Status == true)
            {
                return Json(new { message = "error" });
            }
            var nguoidung = db.NguoiDungs.Find(adminCongTien.IDNguoiDung);
            if (nguoidung == null)
            {
                return Json(new { message = "Người dùng không tồn tại" });
            }
            var layThoiGian = DateTime.Now;
            var tienNguoiDung = Convert.ToInt32(nguoidung.Tien);

            var bienDongSoDu = new BienDongSoDu();
            bienDongSoDu.SoTien = adminCongTien.SoTien;
            bienDongSoDu.LoiNhan = adminCongTien.CongTien == true ? "Admin cộng tiền" : "Admin trừ tiền";
            bienDongSoDu.ThoiGian = layThoiGian;
            bienDongSoDu.TruTien = adminCongTien.CongTien;
            bienDongSoDu.IDNguoiDung = nguoidung.IDNguoiDung;
            bienDongSoDu.TienTruoc = tienNguoiDung;
            if (adminCongTien.CongTien == true)
            {
                bienDongSoDu.TienSau = tienNguoiDung + adminCongTien.SoTien;
                nguoidung.Tien += adminCongTien.SoTien;
            }
            else
            {
                bienDongSoDu.TienSau = tienNguoiDung - adminCongTien.SoTien;
                nguoidung.Tien -= adminCongTien.SoTien;
            }
            db.BienDongSoDus.Add(bienDongSoDu);
            var setAdminCongTien = db.AdminCongTiens.Find(adminCongTien.IdAdminCongTien);
            setAdminCongTien.Status = true;
            db.SaveChanges();
            return Json(new { message = "success" });
        }

        [HttpPost]
        public ActionResult HuyDon(int id)
        {
            var congTienRecord = db.AdminCongTiens.Find(id);
            if (congTienRecord != null)
            {
                congTienRecord.HuyDon = true;
                db.SaveChanges();
            }

            return Json(new { message = "success" });
        }

        [HttpPost]
        public JsonResult CheckAdminPassword(string password)
        {
            try
            {
                // Lấy ID của admin từ session
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                var admin = db.NguoiDungs.Find(idnd);

                // Kiểm tra mật khẩu admin (giả sử MatKhau2 là mật khẩu cấp cao của admin)
                if (admin != null && admin.MatKhau2 == password)
                {
                    // Mật khẩu đúng
                    return Json(new { success = true });
                }
                else
                {
                    // Mật khẩu sai
                    return Json(new { success = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateBlockStatus(int id, bool block, string blockContent)
        {
            var user = db.NguoiDungs.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            user.Block = block;
            user.BlockContent = blockContent;
            db.SaveChanges();

            return RedirectToAction("Edit", new { id = id });
        }

        [HttpPost]
        public JsonResult AnAccVip(bool AnAccVip, int IdUser)
        {
            int level = Convert.ToInt16(Session["levelAdmin"]);

            using (var db = new accgameEntities())
            {
                var anAcc = db.AnAccs.FirstOrDefault(a => a.IdUser == IdUser);

                if (anAcc == null)
                {
                    anAcc = new AnAcc
                    {
                        IdUser = IdUser,
                        AnAccVip = AnAccVip,
                        IsAdminAn = (level == 1) ? true : (bool?)null
                    };
                    db.AnAccs.Add(anAcc);
                }
                else
                {
                    anAcc.AnAccVip = AnAccVip;
                    anAcc.IsAdminAn = (level == 1) ? true : (bool?)null;
                }

                var accList = db.Accs.Where(a => a.IDNguoiDung == IdUser).ToList();
                foreach (var acc in accList)
                {
                    acc.An = AnAccVip;
                }

                db.SaveChanges();
            }

            return Json(new { success = true, level = level });
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
