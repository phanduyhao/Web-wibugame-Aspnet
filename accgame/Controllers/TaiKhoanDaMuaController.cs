using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using accgame.Areas.quantri.Controllers;
using accgame.Context;
using accgame.Filters;
using accgame.Models;
using Newtonsoft.Json;

namespace accgame.Controllers
{
    [NotifyUserFilter]

    public class TaiKhoanDaMuaController : Controller
    {

        private bool IsUserBlocked()
        {
            var idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            if (idnd == 0)
            {
                return true;
            }

            using (var db = new accgameEntities())
            {
                var nguoidung = db.NguoiDungs.Find(idnd);
                if (nguoidung != null && nguoidung != null && nguoidung.Block == true)
                {
                    return true;
                }
            }
            return false;
        }

        public ActionResult Index()
        {
            if (IsUserBlocked())
            {
                return RedirectToAction("Blocked", "Home");
            }

            using (var db = new accgameEntities())
            {
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                if (idnd == 0)
                {
                    return Redirect("/");
                }

                ViewBag.tkdm = "active";
                ViewBag.sotkthuong = db.Accs.Where(m => m.DaBan == true && m.IDNguoiMua == idnd).Count();

                // Tính tổng giá trị tài khoản thường đã mua
                var accThuongDamua = db.Accs.Where(m => m.DaBan == true && m.IDNguoiMua == idnd).ToList();

                // Tính tổng giá trị cho các tài khoản có GiaDaMua != null
                var giaThuongDaMuaCo = accThuongDamua.Where(m => m.GiaDaMua != null).Sum(m => m.GiaDaMua.Value);

                // Tính tổng giá trị cho các tài khoản có GiaDaMua == null
                var giaThuongDaMuaNull = accThuongDamua.Where(m => m.GiaDaMua == null).Sum(m => m.Gia);

                // Tổng giá trị
                ViewBag.giatritkthuong = giaThuongDaMuaCo + giaThuongDaMuaNull;

                ViewBag.sotkreroll = db.AccRandoms.Where(m => m.DaBan == true && m.IDNguoiMua == idnd).Count();

                // Tính tổng giá trị tài khoản reroll/random đã mua
                var accdamua = db.AccRandoms.Where(m => m.DaBan == true && m.IDNguoiMua == idnd).ToList();

                // Tính tổng giá trị cho các tài khoản có GiaDaMua != null
                var giaDaMuaCo = accdamua.Where(m => m.GiaDaMua != null).Sum(m => m.GiaDaMua.Value);

                // Tính tổng giá trị cho các tài khoản có GiaDaMua == null
                var giaDaMuaNull = accdamua.Where(m => m.GiaDaMua == null).Sum(m => m.DanhMuc.Gia);

                // Tổng giá trị
                ViewBag.giatritkreroll = giaDaMuaCo + giaDaMuaNull;

                var CtvCollab = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).Select(m => m.CtvCollab).FirstOrDefault();
                ViewBag.CtvCollab = CtvCollab;

                var OnOffCtvCollab = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).Select(m => m.OnOffCtvCollab).FirstOrDefault();
                ViewBag.OnOffCtvCollab = OnOffCtvCollab;

                var CtvCollabCauHinh = db.CaiDats.Where(m => m.MaCaiDat == "is_ctv_collab").FirstOrDefault();
                ViewBag.CtvCollabCauHinh = CtvCollabCauHinh != null ? CtvCollabCauHinh.NoiDung : null;

                return View();
            }
        }

        public ActionResult taikhoanthuong()
        {
            if (IsUserBlocked())
            {
                return RedirectToAction("Blocked", "Home");
            }

            using (var db = new accgameEntities())
            {
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                if (idnd == 0)
                {
                    return Redirect("/");
                }
                //
                var taikhoan = db.Accs.Where(m => m.DaBan == true && m.IDNguoiMua == idnd && m.Xoa != true).OrderByDescending(m => m.ThoiGianBan).ThenByDescending(m => m.IDAcc).ToList();
                var CtvCollab = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).Select(m => m.CtvCollab).FirstOrDefault();
                ViewBag.CtvCollab = CtvCollab;
                var OnOffCtvCollab = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).Select(m => m.OnOffCtvCollab).FirstOrDefault();
                ViewBag.OnOffCtvCollab = OnOffCtvCollab;
                var CtvCollabCauHinh = db.CaiDats.Where(m => m.MaCaiDat == "is_ctv_collab").FirstOrDefault();
                ViewBag.CtvCollabCauHinh = CtvCollabCauHinh != null ? CtvCollabCauHinh.NoiDung : null;
                return View(taikhoan);
            }
               
        }

        [HttpPost]
        public ActionResult loadAccOld()
        {
            int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
            int level = Convert.ToInt16(Session["levelAdmin"]);
            if ( idnd == 0)
            {
                return Json(new { status = "error" });
            }
            using (var db = new accgameEntities())
            {
                var user = db.NguoiDungs.Find(idnd);
                var apiUrl = HttpContext.Application["url_api"]?.ToString();

                string content = "";
                // api version 2
                if(user == null){
                    return Json(new { status = "error" });
                }
                string url = apiUrl + "api/account/history_buy.php?username=" + user.TenNguoiDung + "&password=" + user.MatKhau;
                if (user.GoogleAccount == true)
                {
                    url = apiUrl + "api/account/history_buy_email.php?email=" + user.TenNguoiDung;
                }
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responeStream = response.GetResponseStream())
                using (StreamReader streamReader = new StreamReader(responeStream))
                {
                    content = streamReader.ReadToEnd();
                }
                listHtr dataJson = JsonConvert.DeserializeObject<listHtr>(content);
                if (dataJson != null && dataJson.status == "success")
                {
                    for (int i = 0; i < dataJson.data.Count; i++)
                    {
                        int idOld = Convert.ToInt32(dataJson.data[i].id_acc);
                        var existingAcc = db.Accs.FirstOrDefault(m => m.IDOldAcc == idOld);

                        if (existingAcc == null)
                        {
                            int giaAcc = Convert.ToInt32(dataJson.data[i].cash);
                            var newAcc = new Acc
                            {
                                TenAcc = dataJson.data[i].detail.name_product,
                                IDOldAcc = idOld,
                                IDNguoiMua = idnd,
                                ThoiGianBan = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt32(dataJson.data[i].created_at)).DateTime,
                                DaBan = true,
                                Gia = giaAcc,
                            };

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
                                }
                            }

                            try
                            {
                                db.Accs.Add(newAcc);
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    return Json(new { status = "success" });
                }
                return Json(new { status = "error" });
            }



        }

        public ActionResult taikhoanreroll()
        {
            if (IsUserBlocked())
            {
                return RedirectToAction("Blocked", "Home");
            }
            using (var db = new accgameEntities())
            {
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                if (idnd == 0)
                {
                    return Redirect("/");
                }
                //
                HomeModels data = new HomeModels()
                {
                    /*                    listAccRandom = db.AccRandoms.Where(m => m.IDNguoiMua == idnd && m.DaBan == true && m.Xoa != true).Include(m => m.DanhMuc).OrderByDescending(m => m.ThoiGianBan).ToList(),
                    */
                    listAccRandomDamua = db.AccRandomDamuas.Where(m => m.AccRandom.IDNguoiMua == idnd).Include(m => m.AccRandom).Include(d => d.AccRandom.DanhMuc).Include(d => d.AccRandom.DanhMuc.LoaiAcc).OrderByDescending(m => m.AccRandom.ThoiGianBan).ToList(),
                };
                var CtvCollab = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).Select(m => m.CtvCollab).FirstOrDefault();
                ViewBag.CtvCollab = CtvCollab;
                var OnOffCtvCollab = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).Select(m => m.OnOffCtvCollab).FirstOrDefault();
                ViewBag.OnOffCtvCollab = OnOffCtvCollab;
                var CtvCollabCauHinh = db.CaiDats.Where(m => m.MaCaiDat == "is_ctv_collab").FirstOrDefault();
                ViewBag.CtvCollabCauHinh = CtvCollabCauHinh != null ? CtvCollabCauHinh.NoiDung : null;
                return View(data);
            }
        }
    }
    public class listHtr
    {
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("data")]
        public List<listHtrData> data { get; set; }
    }
    public class listHtrData
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("id_acc")]
        public string id_acc { get; set; }

        [JsonProperty("type_category")]
        public string type_category { get; set; }
        [JsonProperty("detail")]
        public detailHtr detail { get; set; }
  
        [JsonProperty("created_at")]
        public string created_at { get; set; }
        [JsonProperty("cash")]
        public string cash { get; set; }
    }

    public class detailHtr
    {
        [JsonProperty("name_product")]
        public string name_product { get; set; }
        [JsonProperty("data")]

        public List<dataAcc> dataAcc { get; set; }

    }
    public class dataAcc
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("value")]
        public string value { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
    }

}