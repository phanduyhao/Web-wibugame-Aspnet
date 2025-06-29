using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using accgame.Filters;
using accgame.Mahoa;
using Google.Apis.PeopleService.v1.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Google.Apis.Requests.BatchRequest;

namespace accgame.Controllers
{
    [CheckSessionID]
    [NotifyUserFilter]

    public class napthecaoController : Controller
    {
        // GET: napthecao
        public ActionResult Index()
        {
            using (var db = new accgameEntities())
            {
                ViewBag.napthecao = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                Session["url"] = "/napthecao";

                var ptNapThe = db.CaiDats.Where(m => m.MaCaiDat == "phan_tram_nap_the").FirstOrDefault();
                ViewBag.ptNapThe = ptNapThe?.NoiDung ?? "80";
                if (idnd == 0)
                {
                    return Redirect("/dangnhap");
                }
                var nguoidung = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).FirstOrDefault();
                if (nguoidung != null && nguoidung.Block == true)
                {
                    return RedirectToAction("Blocked", "Home");

                }
                var napthe = db.NapTiens.Where(m => m.IDNguoiDung == idnd && m.request_id != null).OrderByDescending(m => m.IdNapTien).ToList();
                ViewBag.isThongBaoNapTheCao = db.CaiDats.Where(m => m.MaCaiDat == "is_thong_bao_napthecao").Select(m => m.NoiDung).FirstOrDefault();
                ViewBag.thongBaoNapTheCao = db.CaiDats.Where(m => m.MaCaiDat == "thong_bao_napthecao")
                                                        .Select(m => m.NoiDung)
                                                        .FirstOrDefault();
                return View(napthe);
            }
        }
        public ActionResult Test()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> thecao(string telco, string code, string serial, string amount)
        {
            //ba4cd99d467fee97f58a368067c370b5 - partner_key 
            //50377816362 - partner_id
            using (var db = new accgameEntities())
            {
                var checkThe = db.NapTiens.Where(m => m.serial == serial && m.code == code).FirstOrDefault();
                if (checkThe != null)
                {
                    TempData["loi"] = "Thẻ đã có trong hệ thống";
                    return RedirectToAction("Index");
                }
                var caidat = db.CaiDats;
                var parnerId = caidat.Where(m => m.MaCaiDat == "parner_id").FirstOrDefault();
                var parnerKey = caidat.Where(m => m.MaCaiDat == "parner_key").FirstOrDefault();
                string partner_key = parnerKey?.NoiDung ?? "";
                string partner_id = parnerId?.NoiDung ?? "";
                var idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                if (idnd == 0)
                {
                    Session["Urlold"] = "/napthecao";
                    return Redirect("/dangnhap");
                }
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://gachthe1s.com/chargingws/v2");
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(telco), "telco");
                content.Add(new StringContent(code), "code");
                content.Add(new StringContent(serial), "serial");
                content.Add(new StringContent(amount), "amount");
                string request_id = DateTime.Now.ToString("yyMMddhhmmssfff");
                content.Add(new StringContent(request_id), "request_id");
                content.Add(new StringContent(partner_id), "partner_id");
                string sign = MyString.md5(partner_key + code + serial);
                content.Add(new StringContent(sign), "sign");
                content.Add(new StringContent("charging"), "command");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                ViewBag.response = await response.Content.ReadAsStringAsync();

                // add db
                var naptien = db.NapTiens.OrderByDescending(m => m.IdNapTien).FirstOrDefault();

                NapTien napthe = new NapTien();
                napthe.IDNguoiDung = idnd;
                napthe.request_id = request_id;
                napthe.code = code;
                napthe.serial = serial;
                napthe.amount = Convert.ToInt32(amount);
                napthe.telco = telco;
                napthe.Noidung = "Đang chờ";
                napthe.thoigian = DateTime.Now;
                if (naptien != null)
                {
                    napthe.IdNapTien = naptien.IdNapTien + 1;
                }
                db.NapTiens.Add(napthe);
                db.SaveChanges();
                TempData["loi"] = "Gửi thẻ cào thành công!";
                return RedirectToAction("Index");
            }

        }
        public async Task<ActionResult> check(string telco, string code, string serial, string amount)
        {
            //ba4cd99d467fee97f58a368067c370b5 - partner_key 
            //50377816362 - partner_id
            using (var db = new accgameEntities())
            {
                var searchCard = db.NapTiens.Where(m => m.code == code && m.serial == serial).FirstOrDefault();
                if (searchCard == null)
                {
                    TempData["loi"] = "Không tìm thấy thẻ cào này!";
                    return RedirectToAction("Index");
                }
                if (searchCard.trangthai != null)
                {
                    TempData["loi"] = "Thẻ cào này đã được duyệt!";
                    return RedirectToAction("Index");
                }
                var caidat = db.CaiDats;
                var parnerId = caidat.Where(m => m.MaCaiDat == "parner_id").FirstOrDefault();
                var parnerKey = caidat.Where(m => m.MaCaiDat == "parner_key").FirstOrDefault();
                string partner_key = parnerKey?.NoiDung ?? "";
                string partner_id = parnerId?.NoiDung ?? "";
                var idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                if (idnd == 0)
                {
                    Session["Urlold"] = "/napthecao";
                    return Redirect("/dangnhap");
                }
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://gachthe1s.com/chargingws/v2");
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(telco), "telco");
                content.Add(new StringContent(code), "code");
                content.Add(new StringContent(serial), "serial");
                content.Add(new StringContent(amount), "amount");
                string request_id = searchCard.request_id;
                content.Add(new StringContent(request_id), "request_id");
                content.Add(new StringContent(partner_id), "partner_id");
                string sign = MyString.md5(partner_key + code + serial);
                content.Add(new StringContent(sign), "sign");
                content.Add(new StringContent("check"), "command");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();

                // Phân tích chuỗi JSON thành đối tượng ApiResponse
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseContent);
                var nguoidung = db.NguoiDungs.Find(searchCard.IDNguoiDung);
                int tienTruocNap = Convert.ToInt32(nguoidung.Tien);
                var setnapthe = db.NapTiens.Find(searchCard.IdNapTien);

                var ptNapThe = db.CaiDats.Where(m => m.MaCaiDat == "phan_tram_nap_the").FirstOrDefault();
                var numptNapThe = ptNapThe?.NoiDung ?? "80";
                double tyleChuyen = Convert.ToInt16(numptNapThe) / 100.00;
                
                if (apiResponse.status == 1)
                {
                    int tienCong = Convert.ToInt32((Convert.ToDouble(searchCard.amount) * tyleChuyen));
                    nguoidung.Tien += tienCong;
                    nguoidung.TienNap = Convert.ToInt32(nguoidung.TienNap) + tienCong;
                    nguoidung.TienNapThang = Convert.ToInt32(nguoidung.TienNapThang) + tienCong;

                    setnapthe.trangthai = true;
                    setnapthe.Noidung = "Thẻ đúng";
                    setnapthe.SoTien = tienCong;
                    setnapthe.TruocNap = tienTruocNap;
                    setnapthe.SauNap = tienTruocNap + tienCong;
                    //Biến động số dư:
                    var biendongds = new BienDongSoDu();
                    biendongds.IDNguoiDung = nguoidung.IDNguoiDung;
                    biendongds.SoTien = tienCong;
                    biendongds.TruTien = false;
                    biendongds.LoiNhan = "Nạp CARD";
                    biendongds.ThoiGian = DateTime.Now;
                    biendongds.TienTruoc = tienTruocNap;
                    biendongds.TienSau = tienTruocNap + tienCong;
                    db.BienDongSoDus.Add(biendongds);
                    //
                    db.SaveChanges();
                    TempData["loi"] = "Thẻ cào đúng, tiền đã được cộng!";
                    return RedirectToAction("Index");
                }

                if (searchCard.trangthai == null && apiResponse.status != 99)
                {
                    
                    if (apiResponse.status == 2)
                    {
                        setnapthe.trangthai = false;
                        int tienCong = Convert.ToInt32((Convert.ToDouble(apiResponse.amount) * 0.3));
                        setnapthe.trangthai = true;
                        setnapthe.Noidung = "Thẻ sai mệnh giá";
                        nguoidung.Tien += tienCong;
                        nguoidung.TienNap = Convert.ToInt32(nguoidung.TienNap) + tienCong;
                        nguoidung.TienNapThang = Convert.ToInt32(nguoidung.TienNapThang) + tienCong;
                        setnapthe.SoTien = tienCong;
                        setnapthe.TruocNap = tienTruocNap;
                        setnapthe.SauNap = tienTruocNap + tienCong;
                        //Biến động số dư:
                        var biendongds = new BienDongSoDu();
                        biendongds.IDNguoiDung = nguoidung.IDNguoiDung;
                        biendongds.SoTien = tienCong;
                        biendongds.TruTien = false;
                        biendongds.LoiNhan = "Nạp CARD";
                        biendongds.ThoiGian = DateTime.Now;
                        biendongds.TienTruoc = tienTruocNap;
                        biendongds.TienSau = tienTruocNap + tienCong;
                        db.BienDongSoDus.Add(biendongds);
                        //
                        db.SaveChanges();
                        TempData["loi"] = "Thẻ sai mệnh giá!";
                        return RedirectToAction("Index");
                    }
                    else if (apiResponse.status == 3)
                    {
                        setnapthe.trangthai = false;
                        setnapthe.Noidung = "Thẻ lỗi";
                        db.SaveChanges();
                        TempData["loi"] = "Thẻ lỗi";
                        return RedirectToAction("Index");
                    }
                    else if (apiResponse.status == 4)
                    {
                        setnapthe.trangthai = false;
                        setnapthe.Noidung = "Bảo trì";
                        db.SaveChanges();
                        TempData["loi"] = "Server nạp bảo trì";
                        return RedirectToAction("Index");
                    }
                    else if (apiResponse.status == 99)
                    {
                        setnapthe.trangthai = false;
                        setnapthe.Noidung = "Đang chờ";
                        db.SaveChanges();

                        TempData["loi"] = "Thẻ đang chờ xử lý, vui lòng đợi ít phút";
                        return RedirectToAction("Index");
                    }
                    db.SaveChanges();
                    TempData["loi"] = "Gửi thẻ thất bại, lỗi: " + apiResponse.message;
                    return RedirectToAction("Index");
                }
                TempData["loi"] = "Gửi thẻ thất bại";
                return RedirectToAction("Index");
            }

        }

        public ActionResult callback(string id, string status, string message, string request_id, string declared_value, string value, string amount, string code, string serial, string telco, string trans_id, string callback_sign)
        {


            using (var db = new accgameEntities())
            {
                var key_web = db.CaiDats.Where(m => m.MaCaiDat == "key_web").FirstOrDefault();
                if (key_web == null)
                {
                    return Content("error");
                }
                else
                {
                    if (key_web.NoiDung != id)
                    {
                        return Content("Sai key");
                    }
                }


                var ptNapThe = db.CaiDats.Where(m => m.MaCaiDat == "phan_tram_nap_the").FirstOrDefault();
                var numptNapThe = ptNapThe?.NoiDung ?? "80";
                double tyleChuyen = Convert.ToInt16(numptNapThe) / 100.00;
                int tienCong = Convert.ToInt32((Convert.ToDouble(value) * tyleChuyen));
                var napthe = db.NapTiens.Where(m => m.request_id == request_id).FirstOrDefault();
                if (napthe == null)
                {
                    return Content("Không tìm thấy");
                }

                var setnapthe = db.NapTiens.Find(napthe.IdNapTien);
                var nguoidung = db.NguoiDungs.Find(napthe.IDNguoiDung);
                int tienTruocNap = Convert.ToInt32(nguoidung.Tien);
                if (napthe.trangthai == null && status == "1")
                {
                    nguoidung.Tien += tienCong;
                    nguoidung.TienNap = Convert.ToInt32(nguoidung.TienNap) + tienCong;
                    nguoidung.TienNapThang = Convert.ToInt32(nguoidung.TienNapThang) + tienCong;

                    setnapthe.trangthai = true;
                    setnapthe.Noidung = "Thẻ đúng";
                    setnapthe.SoTien = tienCong;
                    setnapthe.TruocNap = tienTruocNap;
                    setnapthe.SauNap = tienTruocNap + tienCong;
                    //Biến động số dư:
                    var biendongds = new BienDongSoDu();
                    biendongds.IDNguoiDung = nguoidung.IDNguoiDung;
                    biendongds.SoTien = tienCong;
                    biendongds.TruTien = false;
                    biendongds.LoiNhan = "Nạp CARD";
                    biendongds.ThoiGian = DateTime.Now;
                    biendongds.TienTruoc = tienTruocNap;
                    biendongds.TienSau = tienTruocNap + tienCong;
                    db.BienDongSoDus.Add(biendongds);
                    //
                    // Tạo thông báo HTML
                    string sotienDinhDang = tienCong.ToString("N0").Replace(",", ".") + "đ";
                    int sodu = nguoidung.TienNap ?? 0; // Sử dụng toán tử ?? để xử lý nullable
                    int tienhientai = tienTruocNap + tienCong;
                    string soduhientai = tienhientai.ToString("N0").Replace(",", ".") + "đ";

                    string noiDungThongBao = $@"
                    <p>Bạn đã nạp thành công số tiền: <strong>{sotienDinhDang}</strong>.</p>
                    <p>Số dư hiện tại: <strong>{soduhientai}</strong>.</p>";


                    // Tạo một NotifyUser mới
                    var notify = new NotifyUser
                    {
                        IdNguoidung = napthe.IDNguoiDung,
                        Contents = noiDungThongBao,
                        IsRead = false,
                        IsAdminPost = false,
                        Thoigian = DateTime.Now // Nếu bạn có cột lưu thời gian
                    };

                    // Thêm vào bảng NotifyUser
                    db.NotifyUsers.Add(notify);
                    db.SaveChanges();
                    return Content(Convert.ToString(tienCong));
                }
                if (napthe.trangthai == null && status != "99")
                {
                    setnapthe.trangthai = false;
                    if (status == "2")
                    {
                        setnapthe.trangthai = true;
                        setnapthe.Noidung = "Thẻ sai mệnh giá";
                        nguoidung.Tien += Convert.ToInt32((double.Parse(value) * 0.3));
                        nguoidung.TienNap = Convert.ToInt32(nguoidung.TienNap) + Convert.ToInt32((double.Parse(value) * 0.3));
                        nguoidung.TienNapThang = Convert.ToInt32(nguoidung.TienNapThang) + Convert.ToInt32((double.Parse(value) * 0.3));
                        setnapthe.SoTien = Convert.ToInt32((double.Parse(value) * 0.3));
                        setnapthe.TruocNap = tienTruocNap;
                        setnapthe.SauNap = tienTruocNap + Convert.ToInt32((double.Parse(value) * 0.3));
                        //Biến động số dư:
                        var biendongds = new BienDongSoDu();
                        biendongds.IDNguoiDung = nguoidung.IDNguoiDung;
                        biendongds.SoTien = Convert.ToInt32((double.Parse(value) * 0.3));
                        biendongds.TruTien = false;
                        biendongds.LoiNhan = "Nạp CARD";
                        biendongds.ThoiGian = DateTime.Now;
                        biendongds.TienTruoc = tienTruocNap;
                        biendongds.TienSau = tienTruocNap + Convert.ToInt32((double.Parse(value) * 0.3));
                        db.BienDongSoDus.Add(biendongds);
                        //
                    }
                    else if (status == "3")
                    {
                        setnapthe.Noidung = "Thẻ lỗi";
                    }
                    else if (status == "4")
                    {
                        setnapthe.Noidung = "Bảo trì";
                    }
                    db.SaveChanges();
                    return Content(Convert.ToString(tienCong));
                }
                return Content(Convert.ToString(tienCong));
            }

        }
    }
    public class ApiResponse
    {
        public int trans_id { get; set; }
        public string request_id { get; set; }
        public int status { get; set; }
        public string message { get; set; }
        public string telco { get; set; }
        public string code { get; set; }
        public string serial { get; set; }
        public int declared_value { get; set; }
        public int? value { get; set; }
        public int amount { get; set; }
    }

}