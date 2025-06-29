using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using accgame.Context;
using PagedList;
using accgame.Filters;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    
    public class quanlygiaodichController : Controller
    {

        // GET: quantri/quanlygiaodich
        public ActionResult Index(int? page, DateTime? timeStart = null, DateTime? timeEnd = null, string search = "")
        {
            using (var db = new accgameEntities())
            {
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                Session["url"] = "/nguoidung/biendongsodu";
                if (idnd == 0)
                {
                    return Redirect("/dangnhap");
                }
                ViewBag.timeStart = timeStart;
                ViewBag.timeEnd = timeEnd;
                ViewBag.search = search;
                int pageSize = 20;
                int pageNumber = page ?? 1;

                // Set default values for timeStart and timeEnd if they are null
                if (!timeStart.HasValue)
                {
                    timeStart = DateTime.Now.AddYears(-10); // 10 years ago
                }
                if (!timeEnd.HasValue)
                {
                    timeEnd = DateTime.Now;
                }

                // Filter transactions based on time range
                var biendongsd = db.BienDongSoDus
                    .Where(m => m.IDNguoiDung == idnd && m.ThoiGian >= timeStart && m.ThoiGian <= timeEnd && m.LoiNhan.Contains(search)).Include(m => m.NguoiDung)
                    .OrderByDescending(m => m.IDBienDongSoDu)
                    .ToPagedList(pageNumber, pageSize); // Assuming you have PagedList extension method available

                return View(biendongsd);
            }
        }

    }


}