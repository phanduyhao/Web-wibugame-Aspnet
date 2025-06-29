using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using accgame.Filters;
using accgame.Models;
using PagedList;

namespace accgame.Controllers
{
    [CheckSessionID]

    public class NoiBoController : Controller
    {
        
        // GET: NoiBo
        public ActionResult Index()
        {
            using (var db = new accgameEntities())
            {
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (idnd == 0 || (level != 1 && level != 2 && level != 3))
                {
                    return Redirect("/");
                }
                HomeModels data = new HomeModels()
                {
                    listAccHK = db.Accs.Where(m => m.LoaiGame == 1 && m.Xoa != true && m.DaBan != true && m.TaiKhoanNoiBo == true && m.An != true).OrderByDescending(m => m.IDAcc).Take(6).ToList(),
                    listDanhMuc = db.DanhMucs.Where(m => m.Xoa != true && m.TaiKhoanNoiBo == true && m.Hide != true).ToList(),
                    listLoaiCode = db.LoaiCodes.Where(m => m.Xoa != true && m.CodeNoiBo == true).ToList(),
                };
                return View(data);
            }
        }

        public ActionResult codenoibo(int? page)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (idnd == 0 || (level != 1 && level != 2 && level != 3))
            {
                Session["url"] = "/noibo/codenoibo";
                if (idnd == 0)
                {
                    return Redirect("/dangnhap");
                }
            }
            using (var db = new accgameEntities())
            {
                int pageSize = 20;
                int pageNumber = (page ?? 1);
                

                HomeModels data = new HomeModels()
                {
                    listLoaiCode = db.LoaiCodes.Where(m => m.Xoa != true && m.CodeNoiBo == true).ToList(),
                    listCode = db.Codes.Where(m => m.DaBan == true && m.IDNguoiDung == idnd && m.LoaiCode.CodeNoiBo == true).Include(m => m.LoaiCode).OrderByDescending(m => m.ThoiGianMua).ThenByDescending(m => m.IDCode).ToPagedList(pageNumber, pageSize),
                };
                return View(data);
            }
        }
    }
}