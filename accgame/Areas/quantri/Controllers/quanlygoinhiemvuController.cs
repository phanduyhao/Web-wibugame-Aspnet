using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using accgame.Filters;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    
    public class quanlygoinhiemvuController : Controller
    {
        private accgameEntities db = new accgameEntities();

        // GET: quantri/quanlygoinhiemvu
        public ActionResult Index()
        {
            ViewBag.qlgnv = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if (level != 1 || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            return View(db.GoiNhiemVus.Where(m => m.Xoa != true).Include(m => m.LoaiGame1).OrderBy(m => m.LoaiGame).ThenByDescending(m => m.IDGoiNhiemVu).ToList());
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
        public ActionResult Create([Bind(Include = "IDGoiNhiemVu,TenGoiNhiemVu,GiaTien,LoaiGame")] GoiNhiemVu goiNhiemVu)
        {
            if (ModelState.IsValid)
            {
                db.GoiNhiemVus.Add(goiNhiemVu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var loaiGame = db.LoaiGames.ToList();
            ViewBag.loaiGame = loaiGame;
            return View(goiNhiemVu);
        }

        // GET: quantri/quanlygoinhiemvu/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GoiNhiemVu goiNhiemVu = db.GoiNhiemVus.Find(id);
            if (goiNhiemVu == null)
            {
                return HttpNotFound();
            }
            var loaiGame = db.LoaiGames.ToList();
            ViewBag.loaiGame = loaiGame;
            return View(goiNhiemVu);
        }

        // POST: quantri/quanlygoinhiemvu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDGoiNhiemVu,TenGoiNhiemVu,GiaTien,LoaiGame")] GoiNhiemVu goiNhiemVu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(goiNhiemVu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(goiNhiemVu);
        }

        // GET: quantri/quanlygoinhiemvu/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GoiNhiemVu goiNhiemVu = db.GoiNhiemVus.Find(id);
            if (goiNhiemVu == null)
            {
                return HttpNotFound();
            }
            return View(goiNhiemVu);
        }

        // POST: quantri/quanlygoinhiemvu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GoiNhiemVu goiNhiemVu = db.GoiNhiemVus.Find(id);
            goiNhiemVu.Xoa = true;
            db.SaveChanges();
            return RedirectToAction("Index");
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
