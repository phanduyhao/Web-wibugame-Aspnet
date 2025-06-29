using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using accgame.Filters;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    
    public class caidatvongquayController : Controller
    {
        // GET: quantri/caidatvongquay
        public ActionResult Index()
        {
            ViewBag.caidatvongquay = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            using (var db = new accgameEntities())
            {
                var caidat = db.CaiDatVongQuays.FirstOrDefault();
                return View(caidat);
            }
        }
        [HttpPost]
        public ActionResult Index(CaiDatVongQuay caiDatVongQuay)
        {
            ViewBag.caidatvongquay = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            using (var db = new accgameEntities())
            {
                var caidat = db.CaiDatVongQuays.FirstOrDefault();
                if(caidat == null)
                {
                    db.CaiDatVongQuays.Add(caiDatVongQuay);
                    db.SaveChanges();
                }
                else
                {
                    var cdvq = db.CaiDatVongQuays.Find(caidat.IDCaiDatVongQuay);
                    cdvq.TyLe1 = caiDatVongQuay.TyLe1;
                    cdvq.TyLe2 = caiDatVongQuay.TyLe2;
                    cdvq.TyLe3 = caiDatVongQuay.TyLe3;
                    cdvq.TyLe4 = caiDatVongQuay.TyLe4;
                    cdvq.TyLe5 = caiDatVongQuay.TyLe5;
                    cdvq.TyLe6 = caiDatVongQuay.TyLe6;
                    cdvq.Gia = caiDatVongQuay.Gia;
                    db.SaveChanges();
                }
                
                caidat = db.CaiDatVongQuays.FirstOrDefault();
                return View(caidat);
            }
        }
    }
}