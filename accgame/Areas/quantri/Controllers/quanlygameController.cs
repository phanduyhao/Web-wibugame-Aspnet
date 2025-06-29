using accgame.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.LocDau;
using Newtonsoft.Json;
using accgame.Controllers;
using System.IO;
using System.Net;
using accgame.Filters;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    
    public class quanlygameController : Controller
    {
        // GET: quantri/quanlygame
        public ActionResult Index()
        {
            using (var db = new accgameEntities())
            {
                ViewBag.game = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }

                //phan quyen
                var dsgame = db.LoaiGames.OrderBy(m => m.STT).ToList();
                if (Request.UrlReferrer != null)
                {
                    TempData["PreviousUrl"] = Request.UrlReferrer.ToString();
                }
                return View(dsgame);
            }
        }
        [HttpPost]
        public ActionResult Index( string TenLoaiGame, string Image, int STT, bool Hide, bool DichVuCayThue, string AnhCayThue, bool DichVuNapGame, string AnhNapGame, bool DichVuCode, string AnhCode)
        {
            using (var db = new accgameEntities())
            {
                ViewBag.game = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }
                
                //phan quyen
                
                LoaiGame loaiGame = new LoaiGame();
                loaiGame.TenLoaiGame = TenLoaiGame;
                var checkLoaiGame = db.LoaiGames.Where(m => m.TenLoaiGame == TenLoaiGame).FirstOrDefault();
                if(checkLoaiGame != null)
                {
                    ViewBag.loi = TenLoaiGame +  "Đã có game này trong hệ thống";
                    var dsgame = db.LoaiGames.OrderBy(m => m.STT).ToList();
                    return View(dsgame);
                }
                loaiGame.Image = Image;
                loaiGame.STT = STT;
                loaiGame.Hide = Hide;
                loaiGame.Slug = LocDau.KhongDau.utf8Convert1(TenLoaiGame).Replace(" ", "-");
                loaiGame.AnhCayThue = AnhCayThue;
                loaiGame.AnhNapGame = AnhNapGame;
                loaiGame.DichVuNapGame = DichVuNapGame;
                loaiGame.DichVuCayThue = DichVuCayThue;
                loaiGame.DichVuCode = DichVuCode;
                loaiGame.AnhCode = AnhCode;

                db.LoaiGames.Add(loaiGame);
                db.SaveChanges();
                TempData["SuccessThem"] = true;
                return Redirect("/quantri/quanlygame");
            }
        }

        public ActionResult sua(int id)
        {
            ViewBag.game = "active";
            using (var db = new accgameEntities())
            {
                var loaiGame = db.LoaiGames.Where(m => m.IDLoaiGame == id).FirstOrDefault();

                return View(loaiGame);
            }
        }
        [HttpPost]
        public ActionResult sua(LoaiGame loaiGame)
        {
            ViewBag.game = "active";
            using (var db = new accgameEntities())
            {
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }
                //phan quyen
                LoaiGame setLoaiGame = db.LoaiGames.Find(loaiGame.IDLoaiGame);
                var checkLoaiGame = db.LoaiGames.Where(m => m.TenLoaiGame == loaiGame.TenLoaiGame && m.IDLoaiGame != loaiGame.IDLoaiGame).FirstOrDefault();
                if (checkLoaiGame != null)
                {
                    ViewBag.notify = "Đã có game này trong hệ thống";
                    return View(setLoaiGame);
                }
                
                setLoaiGame.Image = loaiGame.Image;
                setLoaiGame.TenLoaiGame = loaiGame.TenLoaiGame;
                setLoaiGame.STT = loaiGame.STT;
                setLoaiGame.Hide = loaiGame.Hide;
                setLoaiGame.AnhNapGame = loaiGame.AnhNapGame;
                setLoaiGame.AnhCayThue = loaiGame.AnhCayThue;
                setLoaiGame.AnhCode = loaiGame.AnhCode;
                setLoaiGame.DichVuCayThue = loaiGame.DichVuCayThue;
                setLoaiGame.DichVuNapGame = loaiGame.DichVuNapGame;
                setLoaiGame.DichVuCode = loaiGame.DichVuCode;
                db.SaveChanges();
                ViewBag.notify = "Chỉnh sửa thành công!";
                TempData["SuccessSua"] = true;

                string previousUrl = TempData["PreviousUrl"] as string;
                if (!string.IsNullOrEmpty(previousUrl))
                {
                    return Redirect(previousUrl);
                }
                return View(setLoaiGame);
            }
        }
        [HttpPost]
        public ActionResult loadGameOld()
        {
            using (var db = new accgameEntities())
            {
                var apiUrl = HttpContext.Application["url_api"]?.ToString();

                string content = "";
                // api version 2
                string url = apiUrl + "api/game.php";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responeStream = response.GetResponseStream())
                using (StreamReader streamReader = new StreamReader(responeStream))
                {
                    content = streamReader.ReadToEnd();
                }
                Game dataJson = JsonConvert.DeserializeObject<Game>(content);
                if (dataJson != null && dataJson.status == "success")
                {
                    for (int i = 0; i < dataJson.data.Count; i++)
                    {
                        var keyProduct = dataJson.data[i].key_product;
                        var searchLoaiGame = db.LoaiGames.FirstOrDefault(m => m.Slug == keyProduct);
                        if (searchLoaiGame == null)
                        {
                            var loaiGame = new LoaiGame();
                            loaiGame.TenLoaiGame = dataJson.data[i].name;
                            loaiGame.Slug = dataJson.data[i].key_product;
                            loaiGame.Image = dataJson.data[i].image.Replace("wibugame.vn", "wibugame.devshop.vn");
                            loaiGame.STT = 1;
                            db.LoaiGames.Add(loaiGame);
                            db.SaveChanges();
                        }

                    }
                    return Json(new { status = "success" });
                }
                return Json(new { status = "error" });
            }
                

            
        }
    }

    public class Game
    {
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("data")]
        public List<dataLoadGame> data { get; set; }
    }
    public class dataLoadGame
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("key_product")]
        public string key_product { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("image")]
        public string image { get; set; }
        
        [JsonProperty("created_at")]
        public string created_at { get; set; }
    }
}