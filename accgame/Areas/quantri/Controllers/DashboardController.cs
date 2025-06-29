using accgame.Context;
using accgame.Controllers;
using accgame.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using accgame.Filters;

namespace accgame.Areas.quantri.Controllers
{
    [CheckSessionID]
    
    public class DashboardController : Controller
    {
        // GET: quantri/Dashboard
        public ActionResult Index()
        {
            //phan quyen
            using (var db = new accgameEntities())
            {

                ViewBag.dashboard = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                int level = Convert.ToInt16(Session["levelAdmin"]);
                var ND = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).FirstOrDefault();

                if ((level != 1 && level != 2 && level != 3) || idnd == 0)
                {
                    return Redirect("/");
                }
                var thanhVien = db.NguoiDungs.Where(m => m.LeverAdmin != 1 && m.LeverAdmin != 2).ToList();
                var tkThuong = db.Accs.Where(m => m.IDNguoiDung == idnd && m.Xoa != true).ToList();
                var cayThue = db.CayThues.Where(m => m.IDNguoiNhan == idnd).ToList();

                var datetime = DateTime.Now;
                var ngay = datetime.Day;
                var thang = datetime.Month;
                var nam = datetime.Year;
                DateTime dauThang = Convert.ToDateTime(thang + "/01/" + nam);
                DateTime dauNgay = Convert.ToDateTime(thang.ToString("00") + "/" + ngay.ToString("00") + "/" + nam.ToString("0000"));

                var bienDongSoDu = db.BienDongSoDus.Where(m => m.IDNguoiDung == idnd && m.TruTien != true && m.LoiNhan != "Nạp CARD" && m.LoiNhan != "Nạp MOMO" && m.LoiNhan != "Nạp ATM").ToList();

                var nguoiDung = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).FirstOrDefault();
                ViewBag.loaiAcc = db.LoaiAccs.Where(m => m.DacBiet != true).ToList(); 
                ViewBag.loaiAccRd = db.LoaiAccs.Where(m => m.DacBiet == true).ToList();

                DashModelss data = new DashModelss()
                {
                    tongDoanhThu = Convert.ToInt32(bienDongSoDu.Where(m => m.TruTien != true).Sum(m => m.SoTien)),
                    taiKhoanGame = tkThuong.Count(),
                    taiKhoanGameDaBan = tkThuong.Where(m => m.DaBan == true).Count(),
                    tongDonCayThue = cayThue.Count(),
                    tongDonCTHoanThanh = cayThue.Where(m => m.HoanThanh == true).Count(),

                    doanhThuHomNay = Convert.ToInt32(bienDongSoDu.Where(m => m.ThoiGian >= dauNgay && m.TruTien != true).ToList().Sum(m => m.SoTien)),
                    taiKhoanDaDang = tkThuong.Where(m => m.ThoiGianDang > dauNgay).ToList().Count(),
                    donBanNickHomNay = tkThuong.Where(m => m.ThoiGianBan > dauNgay).ToList().Count(),

                    doanhThuThang = Convert.ToInt32(bienDongSoDu.Where(m => m.ThoiGian >= dauThang && m.TruTien != true).ToList().Sum(m => m.SoTien)),
                    taiKhoanDaDangThang = tkThuong.Where(m => m.ThoiGianDang > dauThang).ToList().Count(),
                    donBanNickThang = tkThuong.Where(m => m.ThoiGianBan > dauThang).ToList().Count(),
                };
                return View(data);
            }
        }

        public ActionResult tongThongKe()
        {
            ViewBag.dashboard = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1 && level != 2 && level != 3) || idnd == 0)
            {
                return Redirect("/");
            }
            //phan quyen
            using (var db = new accgameEntities())
            {
                var thanhVien = db.NguoiDungs.Where(m => m.LeverAdmin != 1 && m.LeverAdmin != 2).ToList();
                var ctv = db.NguoiDungs.Where(m => m.LeverAdmin == 2).ToList();
                var tatCaThanhVien = db.NguoiDungs.ToList();
                var naptien = db.NapTiens.ToList();
                var tkReroll = db.AccRandoms.Where(m => m.Xoa != true).ToList();
                var tkThuong = db.Accs.Where(m => m.Xoa != true).ToList();
                var cayThue = db.CayThues.ToList();

                var datetime = DateTime.Now;
                var ngay = datetime.Day;
                var thang = datetime.Month;
                var nam = datetime.Year;
                DateTime dauThang = Convert.ToDateTime(thang + "/01/" + nam);
                DateTime dauNgay = Convert.ToDateTime(thang.ToString("00") + "/" + ngay.ToString("00") + "/" + nam.ToString("0000"));

                var bienDongSoDu = db.BienDongSoDus.Where(m => m.IDNguoiDung == idnd).ToList();
                DashModelss data = new DashModelss()
                {
                    thanhVien = thanhVien.Count(),
                    tienThanhVien = Convert.ToInt32(thanhVien.Sum(m => m.Tien)),
                    tienCTV = Convert.ToInt32(ctv.Sum(m => m.Tien)),
                    tongTienDaNap = Convert.ToInt32(tatCaThanhVien.Sum(m => m.TienNap)),
                    bankNap = naptien.Where(m => m.Magd != null).Count(),
                    theNap = naptien.Where(m => m.Magd == null).Count(),
                    taiKhoanGame = tkReroll.Count() + tkThuong.Count(),
                    taiKhoanGameDaBan = tkReroll.Where(m => m.DaBan == true).Count() + tkThuong.Where(m => m.DaBan == true).Count(),
                    tongDonNap = naptien.Count(),
                    tongDonNapHoanThanh = naptien.Where(m => m.Magd == null).Count() + naptien.Where(m => m.Magd != null).Where(m => m.SoTien != null).Count(),
                    tongDonCayThue = cayThue.Count(),
                    tongDonCTHoanThanh = cayThue.Where(m => m.HoanThanh == true).Count(),

                    doanhThuHomNay = Convert.ToInt32(bienDongSoDu.Where(m => m.ThoiGian >= dauNgay && m.TruTien != true).ToList().Sum(m => m.SoTien)),
                    tienNapHomNay = Convert.ToInt32(naptien.Where(m => m.thoigian >= dauNgay).ToList().Sum(m => m.SoTien)),
                    napBankHomNay = Convert.ToInt32(naptien.Where(m => m.thoigian >= dauNgay && m.Magd != null).ToList().Sum(m => m.SoTien)),
                    napTheHomNay = Convert.ToInt32(naptien.Where(m => m.thoigian >= dauNgay && m.Magd == null).ToList().Sum(m => m.SoTien)),
                    taiKhoanDaDang = tkThuong.Where(m => m.ThoiGianDang > dauNgay).ToList().Count(),
                    donBanNickHomNay = tkThuong.Where(m => m.ThoiGianBan > dauNgay).ToList().Count(),

                    doanhThuThang = Convert.ToInt32(bienDongSoDu.Where(m => m.ThoiGian >= dauThang && m.TruTien != true).ToList().Sum(m => m.SoTien)),
                    tienNapThang = Convert.ToInt32(naptien.Where(m => m.thoigian >= dauThang).ToList().Sum(m => m.SoTien)),
                    napBankThang = Convert.ToInt32(naptien.Where(m => m.thoigian >= dauThang && m.Magd != null).ToList().Sum(m => m.SoTien)),
                    napTheThang = Convert.ToInt32(naptien.Where(m => m.thoigian >= dauThang && m.Magd == null).ToList().Sum(m => m.SoTien)),
                    taiKhoanDaDangThang = tkThuong.Where(m => m.ThoiGianDang > dauThang).ToList().Count(),
                    donBanNickThang = tkThuong.Where(m => m.ThoiGianBan > dauThang).ToList().Count(),
                };
                return View(data);
            }
        }


        [HttpPost]
        public ActionResult loadTaiKhoan(string slug_loai_acc)
        {
            ViewBag.dashboard = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1 && level != 2 && level != 3) || idnd == 0)
            {
                return Json(new { status = "error" });
            }
            
            //phan quyen
            using (var db = new accgameEntities())
            {
                var user = db.NguoiDungs.Find(idnd);
                var apiUrl = HttpContext.Application["url_api"]?.ToString();

                string content = "";
                // api version 2
                string url = apiUrl + "api/account/acc-chua-ban.php?username=" + user.TenNguoiDung + "&password=" + user.MatKhau + "&type_category=" + slug_loai_acc;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responeStream = response.GetResponseStream())
                using (StreamReader streamReader = new StreamReader(responeStream))
                {
                    content = streamReader.ReadToEnd();
                }
                listAccs dataJson = JsonConvert.DeserializeObject<listAccs>(content);
                
                var loaiAcc = db.LoaiAccs.Where(m => m.slug == slug_loai_acc).FirstOrDefault();
                var loaiGame = db.LoaiGames.Where(m => m.IDLoaiGame == loaiAcc.IDLoaiGame).FirstOrDefault();
                var dangCoLoaiAcc = 0;
                if (dataJson != null && dataJson.status == "success" && dataJson.data != null)
                {
                    for (int i = 0; i < dataJson.data.Count; i++)
                    {
                        var oldId = Convert.ToInt32(dataJson.data[i].id);
                        var searchAcc = db.Accs.FirstOrDefault(m => m.IDOldAcc == oldId);
                        if (searchAcc == null)
                        {
                            var newAcc = new Acc();
                            newAcc.TenAcc = dataJson.data[i].detail.dataAcc[0].value;
                            newAcc.MaTaiKhoan = Mahoa.MyString.vietTat(loaiGame.TenLoaiGame) + "o" + oldId;
                            newAcc.Gia = Convert.ToInt32(dataJson.data[i].cash);
                            newAcc.IDOldAcc = oldId;
                            newAcc.IDNguoiDung = idnd;
                            var imageUrls = JsonConvert.DeserializeObject<List<string>>(dataJson.data[i].image);
                            if (imageUrls != null && imageUrls.Count > 0)
                            {
                                var firstImageUrl = imageUrls[0].Replace("https://wibugame.vn/upload", "https://upload.wibugame.vn");
                                newAcc.AnhDaiDien = firstImageUrl;
                            }
                            else
                            {
                                newAcc.AnhDaiDien = null;  // Hoặc một giá trị mặc định khác
                            }
                            newAcc.IDLoaiAcc = loaiAcc.IDLoaiAcc;
                            newAcc.LoaiGame = loaiGame.IDLoaiGame;
                            switch (dataJson.data[i].server)
                            {
                                case "1":
                                    newAcc.Server = "asia";
                                    break;
                                case "2":
                                    newAcc.Server = "europe";
                                    break;
                                case "3":
                                    newAcc.Server = "america";
                                    break;
                                case "4":
                                    newAcc.Server = "tw_hk_mo";
                                    break;
                            }
                            foreach (var item in dataJson.data[i].detail.dataAcc)
                            {
                                switch (item.name)
                                {
                                    case "taikhoan":
                                    case "tendangnhap":
                                        newAcc.TaiKhoan = item.value;
                                        break;
                                    case "matkhau":
                                        newAcc.MatKhau = item.value;
                                        break;
                                    case "mota":
                                        newAcc.TenAcc = item.value;
                                        break;
                                    case "tuong5sao":
                                        newAcc.NhanVat = item.value;
                                        break;
                                    case "vukhi5sao":
                                        newAcc.VuKhi = item.value;
                                        break;
                                    case "ar":
                                        newAcc.CapKhaiPha = item.value;
                                        break;
                                }
                            }

                            db.Accs.Add(newAcc);
                            db.SaveChanges();
                            dangCoLoaiAcc += 1;
                        }
                    }
                    var setLoaiAcc = db.LoaiAccs.Find(loaiAcc.IDLoaiAcc);
                    if (setLoaiAcc != null)
                    {
                        setLoaiAcc.DangBan += dangCoLoaiAcc;
                        db.SaveChanges();
                    }
                    return Json(new { status = "success" });
                }
                return Json(new { status = "Dữ liệu không hợp lệ" });
            }

        }

        [HttpPost]
        public ActionResult loadDanhMucRd(string slug_loai_acc)
        {
            ViewBag.dashboard = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1 && level != 2 && level != 3) || idnd == 0)
            {
                return Json(new { status = "error" });
            }

            //phan quyen
            using (var db = new accgameEntities())
            {


                var user = db.NguoiDungs.Find(idnd);
                var apiUrl = HttpContext.Application["url_api"]?.ToString();

                string danhMucContent = "";
                string danhMucUrl = apiUrl + "api/danh-muc-game-rd.php";
                HttpWebRequest requestDm = (HttpWebRequest)WebRequest.Create(danhMucUrl);
                requestDm.AutomaticDecompression = DecompressionMethods.GZip;
                using (HttpWebResponse response = (HttpWebResponse)requestDm.GetResponse())
                using (Stream responeStream = response.GetResponseStream())
                using (StreamReader streamReader = new StreamReader(responeStream))
                {
                    danhMucContent = streamReader.ReadToEnd();
                }

                listDanhMucrd dataJsonRd = JsonConvert.DeserializeObject<listDanhMucrd>(danhMucContent);
                var loaiAcc = db.LoaiAccs.Where(m => m.slug == slug_loai_acc).FirstOrDefault();
                var loaiGame = db.LoaiGames.Where(m => m.IDLoaiGame == loaiAcc.IDLoaiGame).FirstOrDefault();

                if(dataJsonRd != null && dataJsonRd.status == "success" && dataJsonRd.data != null)
                {
                    for (int i = 0; i < dataJsonRd.data.Count; i++)
                    {
                        var oldIdRd = Convert.ToInt32(dataJsonRd.data[i].id);
                        string slugDm = dataJsonRd.data[i].type_category;
                        var searchDm = db.DanhMucs.FirstOrDefault(m => m.slug == slugDm);
                        
                        if (searchDm == null)
                        {
                            var newDm = new DanhMuc();
                            newDm.LoaiGame = loaiGame.IDLoaiGame;
                            newDm.TenDanhMuc = dataJsonRd.data[i].detail.name_product;
                            newDm.Gia = int.TryParse(dataJsonRd.data[i].detail.cash, out int Gia) ? Gia : 0;
                            newDm.DaBan = int.TryParse(dataJsonRd.data[i].detail.sale, out int daBan) ? daBan : 0;
                            newDm.HienCo = 0;
                            if (dataJsonRd.data[i].detail != null && dataJsonRd.data[i].detail.thumb != null)
                            {
                                newDm.AnhDanhMuc = dataJsonRd.data[i].detail.thumb.Replace("wibugame.vn/upload", "upload.wibugame.vn");
                            }
                            newDm.IDLoaiAcc = loaiAcc.IDLoaiAcc;
                            newDm.slug = dataJsonRd.data[i].type_category;
                            foreach (var item in dataJsonRd.data[i].detail.data)
                            {
                                switch (item.name)
                                {
                                    case "server":
                                        newDm.Server = item.value;
                                        break;
                                    case "ar":
                                        newDm.Ar = item.value;
                                        newDm.DanhMucLevel = item.value;
                                        break;
                                }
                            }
                            db.DanhMucs.Add(newDm);
                            db.SaveChanges();

                        }
                    }
                    return Json(new { status = "success" });
                }
                else
                {
                    return Json(new { status = "Dữ liệu không hợp lệ" });
                }
              
            }

        }


        [HttpPost]
        public ActionResult loadTaiKhoanRd(int id)
        {
            ViewBag.dashboard = "active";
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ((level != 1 && level != 2 && level != 3) || idnd == 0)
            {
                return Json(new { status = "error" });
            }

            //phan quyen
            using (var db = new accgameEntities())
            {
                var DanhMuc = db.DanhMucs.Where(m => m.IDDanhMuc == id).FirstOrDefault();
                if (DanhMuc == null)
                {
                    return Json(new { status = "Không tìm thấy danh mục này" });
                }
                var slug_loai_acc = DanhMuc.slug;
                var loaiAcc = db.LoaiAccs.Where(m => m.IDLoaiAcc == DanhMuc.IDLoaiAcc).FirstOrDefault();
                var user = db.NguoiDungs.Find(idnd);
                var apiUrl = HttpContext.Application["url_api"]?.ToString();

                string content = "";
                // api version 2
                string url = apiUrl + "api/account/acc-chua-ban.php?username=" + user.TenNguoiDung + "&password=" + user.MatKhau + "&type_category=" + slug_loai_acc;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responeStream = response.GetResponseStream())
                using (StreamReader streamReader = new StreamReader(responeStream))
                {
                    content = streamReader.ReadToEnd();
                }
                listAccs dataJson = JsonConvert.DeserializeObject<listAccs>(content);
                var dangCoLoaiAcc = 0;
                if (dataJson != null && dataJson.status == "success" && dataJson.data != null)
                {
                    for (int j = 0; j < dataJson.data.Count; j++)
                    {
                        var oldId = Convert.ToInt32(dataJson.data[j].id);
                        var searchAcc = db.Accs.FirstOrDefault(m => m.IDOldAcc == oldId);
                        if (searchAcc == null)
                        {
                            var newAcc = new AccRandom();
                            newAcc.IDOldAcc = oldId;
                            newAcc.IDNguoiDung = idnd;

                            newAcc.IDDanhMuc = Convert.ToInt32(DanhMuc.IDDanhMuc);
                            newAcc.IDOldAcc = Convert.ToInt32(dataJson.data[j].id);
                            var danhMuc = db.DanhMucs.Find(DanhMuc.IDDanhMuc);
                            if (danhMuc != null)
                            {
                                danhMuc.HienCo += 1;
                                db.SaveChanges();
                            }
                            foreach (var item in dataJson.data[j].detail.dataAcc)
                            {
                                switch (item.name)
                                {
                                    case "taikhoan":
                                    case "tendangnhap":
                                        newAcc.TaiKhoan = item.value;
                                        break;
                                    case "matkhau":
                                        newAcc.MatKhau = item.value;
                                        break;
                                }
                            }

                            db.AccRandoms.Add(newAcc);
                            db.SaveChanges();
                            dangCoLoaiAcc += 1;
                        }
                    }
                    var setLoaiAcc = db.LoaiAccs.Find(loaiAcc.IDLoaiAcc);
                    if (setLoaiAcc != null)
                    {
                        setLoaiAcc.DangBan += dangCoLoaiAcc;
                        db.SaveChanges();
                    }
                    return Json(new { status = "success" });
                }
                else
                {
                    return Json(new { status = "Dữ liệu không hợp lệ" });
                }

            }

        }
    }


    public class listAccs
    {
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("data")]
        public List<listDataAcc> data { get; set; }
    }
    public class listDataAcc
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("detail")]
        public detailDataAcc detail { get; set; }

        [JsonProperty("image")]
        public string image { get; set; }
        [JsonProperty("cash")]
        public string cash { get; set; }
        [JsonProperty("server")]
        public string server { get; set; }
        [JsonProperty("created_at")]
        public string created_at { get; set; }
    }

    public class detailDataAcc
    {
        [JsonProperty("data")]
        public List<dataAcc> dataAcc { get; set; }

    }
    public class dataAcc
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("value")]
        public string value { get; set; }

    }

    public class listDanhMucrd
    {
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("data")]
        public List<danhmucrd> data { get; set; }
    }

    public class danhmucrd
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("stt")]
        public string stt { get; set; }
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("product")]
        public string product { get; set; }
        [JsonProperty("type_category")]
        public string type_category { get; set; }

        [JsonProperty("detail")]
        public detailDanhMucRd detail { get; set; }
    }

    public class detailDanhMucRd
    {
        
        [JsonProperty("name_product")]
        public string name_product { get; set; }
        [JsonProperty("thumb")]
        public string thumb { get; set; }

        [JsonProperty("cash")]
        public string cash { get; set; }
        [JsonProperty("sale")]
        public string sale { get; set; }

        [JsonProperty("data")]
        public List<dataAccRd> data { get; set; }

    }

    public class dataAccRd
    {
        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("value")]
        public string value { get; set; }

    }
}