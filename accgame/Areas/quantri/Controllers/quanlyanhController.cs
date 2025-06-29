using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using accgame.Models;
using ImageMagick;
using PagedList;
using accgame.Filters;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    
    public class quanlyanhController : Controller
    {
        // GET: quantri/quanlyanh
        public ActionResult Index(int? page)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }

            ViewBag.qla = "active";
            int pageSize = 20;
            int pageNumber = page ?? 1;
            ViewBag.page = pageNumber;
            using (var db = new accgameEntities())
            {
                var listAnh_Acc = db.Anh_Acc.Include(m => m.NguoiDung).OrderByDescending(m => m.ThoiGian).ToPagedList(pageNumber, pageSize);
                return View(listAnh_Acc);
            }
                
        }
        [HttpPost]
        public ActionResult Index(int? page, HttpPostedFileBase ImageUpload)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }

            var filename = "Anhacc_" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmssfff")) + ".webp";
            var savePath = Path.Combine(Server.MapPath("/Content/images/") + filename);
            int pageSize = 20;
            int pageNumber = page ?? 1;
            ViewBag.page = pageNumber;
            ViewBag.qla = "active";
            //ImageUpload.SaveAs(savePath);
            using (var memoryStream = new MemoryStream())
            {

                ImageUpload.InputStream.CopyTo(memoryStream);
                byte[] fileBytes = memoryStream.ToArray();

                using (var image = new MagickImage(fileBytes))
                {
                    if (image.Width > 1000)
                    {
                        int newWidth = 1000;
                        int newHeight = (int)(image.Height * (1000.0 / image.Width));
                        image.Resize(newWidth, newHeight);
                    }
                    image.Quality = 75;
                    image.Format = MagickFormat.WebP;
                    image.Write(savePath);
                }
            }
            using (var db = new accgameEntities())
            {
                var anhAcc = new Anh_Acc();
                anhAcc.DuongDanAnh = "/Content/images/" + filename;
                anhAcc.ThoiGian = DateTime.Now;
                anhAcc.IDNguoiDung = idnd;
                db.Anh_Acc.Add(anhAcc);
                db.SaveChanges();
                ViewBag.notify = "Thêm ảnh mơi thành công!";
                var listAnh_Acc = db.Anh_Acc.Include(m => m.NguoiDung).OrderByDescending(m => m.ThoiGian).ToPagedList(pageNumber, pageSize);
                return View(listAnh_Acc);
            }

        }
    }
}