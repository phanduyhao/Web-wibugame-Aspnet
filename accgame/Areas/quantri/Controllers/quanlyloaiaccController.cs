using accgame.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using accgame.LocDau;
using System.Data.Entity;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using accgame.Filters;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    
    public class quanlyloaiaccController : Controller
    {
        // GET: quantri/loaiacc
        public ActionResult Index()
        {
            using (var db = new accgameEntities())
            {
                ViewBag.loaiacc = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }

                //phan quyen
                var dsacc = db.LoaiAccs.Include(m => m.LoaiGame).OrderBy(m => m.STT).ToList();
                ViewBag.loaigame = db.LoaiGames.Where(m => m.Hide != true).ToList();
                if (Request.UrlReferrer != null)
                {
                    TempData["PreviousUrl"] = Request.UrlReferrer.ToString();
                }
                return View(dsacc);
            }

        }
        [HttpPost]
        public ActionResult Index( string TenLoaiAcc, string MoTa, string Image, int STT, int DaBan, int IDLoaiGame, int? IDLoaiAccCha, bool DacBiet, bool Hide, string Server)
        {
            using (var db = new accgameEntities())
            {
                ViewBag.loaiacc = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }
                
                //phan quyen
                
                LoaiAcc loaiAcc = new LoaiAcc();
                loaiAcc.TenLoaiAcc = TenLoaiAcc;
                var checkLoaiAcc = db.LoaiAccs.Where(m => m.TenLoaiAcc == TenLoaiAcc).FirstOrDefault();
                if(checkLoaiAcc != null)
                {
                    ViewBag.loi = TenLoaiAcc + " - Đã có loại acc này trong hệ thống";
                    ViewBag.loaigame = db.LoaiGames.Where(m => m.Hide != true).ToList();
                    var dsacc = db.LoaiAccs.Include(m => m.LoaiGame).OrderBy(m => m.STT).ToList();
                    return View(dsacc);
                }
                loaiAcc.Image = Image;
                loaiAcc.STT = STT;
                loaiAcc.Hide = Hide;
                loaiAcc.slug = LocDau.KhongDau.utf8Convert1(TenLoaiAcc).Replace(" ", "-");
                loaiAcc.DaBan = DaBan;
                loaiAcc.DangBan = 0;
                loaiAcc.IDLoaiGame = IDLoaiGame;
                loaiAcc.DacBiet = DacBiet;
                loaiAcc.MoTa = MoTa;
                if (IDLoaiAccCha.HasValue)
                {
                    loaiAcc.IDLoaiAccCha = IDLoaiAccCha.Value;
                }

                db.LoaiAccs.Add(loaiAcc);
                db.SaveChanges();
                TempData["SuccessThem"] = true;
                return Redirect("/quantri/quanlyloaiacc");
            }
        }

        public ActionResult sua(int id)
        {
            ViewBag.loaiacc = "active";

            using (var db = new accgameEntities())
            {
                ViewBag.loaigame = db.LoaiGames.Where(m => m.Hide != true).ToList();
                ViewBag.loaiacc = db.LoaiAccs.Where(m => m.Hide != true ).ToList();
                var loaiAcc = db.LoaiAccs.Where(m => m.IDLoaiAcc == id).FirstOrDefault();
                return View(loaiAcc);
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult sua(LoaiAcc loaiAcc, HttpPostedFileBase ImageUpload)
        {
            ViewBag.loaiacc = "active";
            using (var db = new accgameEntities())
            {
                ViewBag.loaigame = db.LoaiGames.Where(m => m.Hide != true).ToList();
                ViewBag.loaiacc = db.LoaiAccs.Where(m => m.Hide != true).ToList();
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                if (level != 1 || idnd == 0)
                {
                    return Redirect("/");
                }
                //phan quyen
                LoaiAcc setLoaiAcc = db.LoaiAccs.Find(loaiAcc.IDLoaiAcc);
                var checkLoaiAcc = db.LoaiAccs.Where(m => m.TenLoaiAcc == loaiAcc.TenLoaiAcc && m.IDLoaiAcc != loaiAcc.IDLoaiAcc).FirstOrDefault();
                if (checkLoaiAcc != null)
                {
                    
                    ViewBag.notify = "Đã có game này trong hệ thống";
                    return View(setLoaiAcc);
                }

                setLoaiAcc.Image = loaiAcc.Image;
                setLoaiAcc.TenLoaiAcc = loaiAcc.TenLoaiAcc;
                setLoaiAcc.STT = loaiAcc.STT;
                setLoaiAcc.Hide = loaiAcc.Hide;
                setLoaiAcc.MoTa = loaiAcc.MoTa;

                setLoaiAcc.DaBan = loaiAcc.DaBan;
                setLoaiAcc.IDLoaiGame = loaiAcc.IDLoaiGame;
                setLoaiAcc.DacBiet = loaiAcc.DacBiet;
                setLoaiAcc.IDLoaiAccCha = loaiAcc.IDLoaiAccCha;
                // Xử lý file upload
                if (ImageUpload != null && ImageUpload.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ImageUpload.FileName);
                    var filePath = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                    ImageUpload.SaveAs(filePath);

                    // Lưu URL ảnh
                    setLoaiAcc.Image = "/Content/Images/" + fileName;
                }
                db.SaveChanges();
                ViewBag.notify = "Chỉnh sửa thành công!";
                TempData["SuccessSua"] = true;

                string previousUrl = TempData["PreviousUrl"] as string;
                if (!string.IsNullOrEmpty(previousUrl))
                {
                    return Redirect(previousUrl);
                }
                return View(setLoaiAcc);
            }
        }

        [HttpPost]
        public ActionResult loadLoaiAcc()
        {
            using (var db = new accgameEntities())
            {
                var apiUrl = HttpContext.Application["url_api"]?.ToString();

                string content = "";
                // api version 2
                string url = apiUrl + "api/danh-muc-game-thuong.php";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responeStream = response.GetResponseStream())
                using (StreamReader streamReader = new StreamReader(responeStream))
                {
                    content = streamReader.ReadToEnd();
                }
                LoaiAccs dataJson = JsonConvert.DeserializeObject<LoaiAccs>(content);
                var loaiGame = db.LoaiGames;
                if (dataJson != null && dataJson.status == "success")
                {
                    for (int i = 0; i < dataJson.datas.Count; i++)
                    {
                        var keyProduct = dataJson.datas[i].type_category;
                        var searchLoaiAcc = db.LoaiAccs.FirstOrDefault(m => m.slug == keyProduct);
                        if (searchLoaiAcc == null)
                        {
                            var loaiAcc = new LoaiAcc();
                            loaiAcc.TenLoaiAcc = dataJson.datas[i].detail.name_product;
                            loaiAcc.slug = dataJson.datas[i].type_category;
                            var thumbUrl = dataJson.datas[i].detail?.thumb?.Replace("wibugame.vn", "wibugame.devshop.vn") ?? "";

                            loaiAcc.Image = thumbUrl;
                            loaiAcc.STT = 1;
                            loaiAcc.DangBan = 0;
                            loaiAcc.DaBan = 0;
                            if (dataJson.datas[i].type == "SUBPRODUCT")
                            {
                                loaiAcc.DacBiet = true;
                            }
                            string product = dataJson.datas[i].product;
                            loaiAcc.IDLoaiGame = loaiGame.Where(m => m.Slug == product).FirstOrDefault()?.IDLoaiGame ?? null;
                            db.LoaiAccs.Add(loaiAcc);
                            db.SaveChanges();
                        }

                    }
                    return Json(new { status = "success" });
                }
                return Json(new { status = "error" });
            }



        }
    }

    public class LoaiAccs
    {
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("data")]
        public List<LoaiAccDatas> datas { get; set; }
    }
    public class LoaiAccDatas
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("stt")]
        public string stt { get; set; }
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("type_category")]
        public string type_category { get; set; }
        [JsonProperty("detail")]
        public LoaiAccDetail detail { get; set; }

        [JsonProperty("created_at")]
        public string created_at { get; set; }
        [JsonProperty("product")]
        public string product { get; set;}
    }

    public class LoaiAccDetail
    {
        [JsonProperty("name_product")]
        public string name_product { get; set; }
        [JsonProperty("thumb")]
        public string thumb { get; set; }
        [JsonProperty("cash")]
        public string cash { get; set; }
    }
}