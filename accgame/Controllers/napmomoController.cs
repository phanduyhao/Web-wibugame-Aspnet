using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using accgame.Filters;

namespace accgame.Controllers
{
    [CheckSessionID]
    [NotifyUserFilter]

    public class napmomoController : Controller
    {

        // GET: napmomo
        public ActionResult Index()
        {
            using (var db = new accgameEntities())
            {
                ViewBag.napmomo = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                Session["url"] = "/napmomo";
                if (idnd == 0)
                {
                    return Redirect("/dangnhap");
                }
                /*              var nd = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).FirstOrDefault();
                              if (nd == null || nd.SessionID == null)
                              {
                                  return Redirect("/dangnhap");
                              }*/

                //phan quyen
                var nguoidung = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).FirstOrDefault();
                if (nguoidung != null && nguoidung.Block == true)
                {
                    return RedirectToAction("Blocked", "Home");

                }
                var caidat = db.CaiDats;
                var momo = caidat.Where(m => m.MaCaiDat == "momo").FirstOrDefault();
                var chummm = caidat.Where(m => m.MaCaiDat == "chu_tk_momo").FirstOrDefault();
                var nganhang = caidat.Where(m => m.MaCaiDat == "ngan_hang").FirstOrDefault();
                var stk = caidat.Where(m => m.MaCaiDat == "stk").FirstOrDefault();

                var bin = Load.NganHang.GetMaBin(nganhang?.NoiDung ?? "");

                ViewBag.chummm = chummm?.NoiDung ?? "";
                ViewBag.momo = momo?.NoiDung ?? "";
                ViewBag.img = "https://img.vietqr.io/image/" + bin + "-" + (stk?.NoiDung ?? "") + "-qr_only.png?amount=0&addInfo=nap%20" + nguoidung.IDNguoiDung;
                //ViewBag.img = "https://chart.googleapis.com/chart?chs=500x500&cht=qr&chl=2|99|0962718292|||0|0|0|nap " + nguoidung.IDNguoiDung + "|transfer_myqr";
                ViewBag.noidung = "nap " + nguoidung.IDNguoiDung;
                return View();
            }
        }

        public ActionResult load()
        {
            using (var db = new accgameEntities())
            {
                string token_mm = db.CaiDats.Where(m => m.MaCaiDat == "token_mm").FirstOrDefault()?.NoiDung ?? "";
                string content = "";
                string ct = "";
                string url = "http://api.web2m.com/historyapimomo/" + token_mm;
                HttpWebRequest s = (HttpWebRequest)WebRequest.Create(url);
                s.AutomaticDecompression = DecompressionMethods.GZip;
                using (HttpWebResponse response = (HttpWebResponse)s.GetResponse())
                using (Stream responeStream = response.GetResponseStream())
                using (StreamReader streamReader = new StreamReader(responeStream))
                {
                    ct = streamReader.ReadToEnd();
                }
                /*            Root dtaJson = JsonConvert.DeserializeObject<Root>(ct);*/

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responeStream = response.GetResponseStream())
                using (StreamReader streamReader = new StreamReader(responeStream))
                {
                    content = streamReader.ReadToEnd();
                }
                Root dataJson = JsonConvert.DeserializeObject<Root>(content);

                int kt = Convert.ToInt32(0);
                int soxu = 0;
                int sotien = 0;
                ViewBag.kq = "";
                var naptien = db.NapTiens.ToList();
                for (int i = 0; i < dataJson.momoMsg.TranList.Length; i++)
                {
                    try
                    {
                        string searchMagdTranid = dataJson.momoMsg.TranList[i].tranId;
                        var searchNap = db.NapTiens.Where(m => m.Magd == searchMagdTranid).FirstOrDefault();
                        if (searchNap != null)
                        {
                            continue;
                        }
                        string[] usname = dataJson.momoMsg.TranList[i].comment.ToLower().Replace("nap ", "#").Split(new char[] { '#' });
                        if (usname.Length > 1)
                        {
                            string usn = Convert.ToString(usname[1]);
                            //string[] layiduser = usn.ToLower().Replace("wibu", "#").Split(new char[] { '#' });
                            int iduser = Convert.ToInt32(usn);
                            var idus = db.NguoiDungs.Where(m => m.IDNguoiDung == iduser).FirstOrDefault();
                            if (idus != null)
                            {
                                NapTien nap = new NapTien();
                                sotien = int.Parse(dataJson.momoMsg.TranList[i].amount);
                                nap.IDNguoiDung = idus.IDNguoiDung;
                                nap.Magd = dataJson.momoMsg.TranList[i].tranId;
                                nap.SoTien = sotien;
                                nap.thoigian = DateTime.Now;
                                nap.Noidung = "Nạp MOMO";
                                db.NapTiens.Add(nap);
                                var USER = db.NguoiDungs.Find(idus.IDNguoiDung);
                                var tienTruoc = Convert.ToInt32(USER.Tien);
                                soxu = Convert.ToInt32(USER.Tien);
                                USER.TienNap = Convert.ToInt32(USER.TienNap) + sotien;
                                USER.Tien = soxu + sotien;
                                nap.TruocNap = soxu;
                                nap.SauNap = soxu + sotien;
                                db.SaveChanges();

                                //Biến động số dư:
                                var biendongds = new BienDongSoDu();
                                biendongds.IDNguoiDung = idus.IDNguoiDung;
                                biendongds.SoTien = sotien;
                                biendongds.TruTien = false;
                                biendongds.LoiNhan = "Nạp MOMO";
                                biendongds.ThoiGian = DateTime.Now;
                                biendongds.TienTruoc = tienTruoc;
                                biendongds.TienSau = USER.Tien;
                                db.BienDongSoDus.Add(biendongds);
                                db.SaveChanges();
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                int idus_nd = Convert.ToInt32(Session["IDNguoiDung"]);
                if (idus_nd != 0)
                {
                    ViewBag.xemxu = db.NguoiDungs.Where(m => m.IDNguoiDung == idus_nd).FirstOrDefault().Tien;
                }
                var data = new { message = "Thành công", success = true};
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            
        }

    }
    public class Root
    {
        public MomoMsg momoMsg { get; set; }
    }
    public class MomoMsg
    {
        public Dat[] TranList { get; set; }
    }

    public class Dat
    {
        public string tranId { get; set; }
        public string ID { get; set; }
        public string partnerId { get; set; }
        public string partnerName { get; set; }
        public string comment { get; set; }
        public string amount { get; set; }
        public string millisecond { get; set; }
    }
}