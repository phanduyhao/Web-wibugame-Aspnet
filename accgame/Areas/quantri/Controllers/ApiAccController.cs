using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using accgame.Filters;

namespace accgame.Areas.quantri.Controllers
{
    public class ApiAccController : Controller
    {
        // GET: quantri/ApiAcc
        public ActionResult Index(int? page, string search)
        {
            using (var db = new accgameEntities())
            {
                var listAcc = db.ApiAccs.ToList();

                int pageNumber = (page ?? 1);
                int newpage = pageNumber - 1;

                ViewBag.apiacc = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }
                //phan quyen

                var data = db.ApiAccs.OrderByDescending(m => m.Time).Skip(newpage * 40).Take(40).ToList();
                decimal count = db.ApiAccs.Count();
                if (search != null)
                {
                    data = db.ApiAccs.Where(m=> m.Name.Contains(search)).OrderByDescending(m => m.Time).Skip(newpage * 40).Take(40).ToList();
                    count = db.ApiAccs.Where(m => m.Name.Contains(search)).Count();
                }
                int last_page = Convert.ToInt16(Math.Ceiling(count / 40));
                ViewBag.last_page = last_page;
                ViewBag.page = pageNumber;
                return View(data);
            }
        }

        public ActionResult key()
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            ViewBag.apiacc = "active";
            using (var db = new accgameEntities())
            {
                var listKey = db.SettingApis.ToList();
                return View(listKey);
            }
                
        }

        public ActionResult themmoi(string DomainWebsite)
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            using (var db = new accgameEntities())
            { 
                var settingApi = new SettingApi();
                settingApi.DomainWebsite = DomainWebsite;
                string apiKey = GenerateKey();
                settingApi.ApiKey = apiKey;
                db.SettingApis.Add(settingApi);
                db.SaveChanges();
                return RedirectToAction("key");            
            }

        }

        private string GenerateKey()
        {
            Random random = new Random();
            int part1 = random.Next(100, 1000);
            int part2 = random.Next(100, 1000);
            int part3 = random.Next(100, 1000);
            int part4 = random.Next(100, 1000);
            return $"{part1}-{part2}-{part3}-{part4}";
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
            using (var db = new accgameEntities())
            {
                var settingApi = db.SettingApis.Find(id);
                db.SettingApis.Remove(settingApi);
                db.SaveChanges();
            }
            return RedirectToAction("key");
        }
    }
}