using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace accgame.Controllers
{
    public class ThongTinController : Controller
    {
        // GET: ThongTin
        public ActionResult Check_ut()
        {
            return View();
        }

        public ActionResult Bao_hanh()
        {
            return View();
        }
    }
}