using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using accgame.Filters;
using Newtonsoft.Json;

namespace accgame.Controllers
{

    public class napACBController : Controller
    {
        accgameEntities db = new accgameEntities();

        // GET: napACB
        [CheckSessionID]

        public ActionResult index()
        {
            using (var db = new accgameEntities())
            {
                ViewBag.napatm = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                Session["url"] = "/napACB";
                if (idnd == 0)
                {
                    return Redirect("/dangnhap");
                }
                //phan quyen
                var nguoidung = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).FirstOrDefault();
                if (nguoidung != null && nguoidung.Block == true)
                {
                    return RedirectToAction("Blocked", "Home");

                }
                ViewBag.img = "https://img.vietqr.io/image/970436-1013842254-qr_only.png?amount=0&addInfo=nap%20" + nguoidung.IDNguoiDung + "&accountName=NGUYEN%20VAN%20TUAN%20VI";

                ViewBag.noidung = "nap " + nguoidung.IDNguoiDung;
                return View();
            }
        }
        public ActionResult load()
        {
            string content = "";
            string ct = "";
            string stringToken = "LpoxVwSWZRcY-fbRQUM-jOYA-fLKp-FUBR";
            string url = "https://api.sieuthicode.net/historyapiacb/" + stringToken;
            HttpWebRequest s = (HttpWebRequest)WebRequest.Create(url);
            s.AutomaticDecompression = DecompressionMethods.GZip;
            using (HttpWebResponse response = (HttpWebResponse)s.GetResponse())
            using (Stream responeStream = response.GetResponseStream())
            using (StreamReader streamReader = new StreamReader(responeStream))
            {
                ct = streamReader.ReadToEnd();
            }
            /*            RootobjectACB dtaJson = JsonConvert.DeserializeObject<RootobjectACB>(ct);*/

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream responeStream = response.GetResponseStream())
            using (StreamReader streamReader = new StreamReader(responeStream))
            {
                content = streamReader.ReadToEnd();
            }
            RootobjectACB dataJson = JsonConvert.DeserializeObject<RootobjectACB>(content);

            int kt = Convert.ToInt32(0);
            int soxu = 0;
            int sotien = 0;
            ViewBag.kq = "";
            var naptien = db.NapTiens.ToList();
            for (int i = 0; i < dataJson.data.Length; i++)
            {
                try
                {
                    string searchMagdTranid = dataJson.data[i].activeDatetime;
                    var searchNap = db.NapTiens.Where(m => m.Magd == searchMagdTranid).FirstOrDefault();
                    if (searchNap != null)
                    {
                        continue;
                    }
                    string[] usname = dataJson.data[i].description.ToLower().Replace("nap", "#").Split(new char[] { '#' });
                    if (usname.Length > 1)
                    {
                        string usn = Convert.ToString(usname[1]);
                        string[] layiduser = usn.ToLower().Replace("gz", "#").Split(new char[] { '#' });
                        int iduser = Convert.ToInt32(layiduser[0]);
                        var idus = db.NguoiDungs.Where(m => m.IDNguoiDung == iduser).FirstOrDefault();
                        if (idus != null)
                        {
                            NapTien nap = new NapTien();
                            sotien = int.Parse(dataJson.data[i].amount.Replace(",", ""));
                            nap.IDNguoiDung = idus.IDNguoiDung;
                            nap.Magd = dataJson.data[i].activeDatetime;
                            nap.SoTien = Convert.ToInt32(sotien);
                            nap.thoigian = DateTime.Now;
                            nap.Noidung = "Nạp ACB";
                            db.NapTiens.Add(nap);

                            db.SaveChanges();
                            kt = 0;
                            var USER = db.NguoiDungs.Find(idus.IDNguoiDung);
                            var tienTruoc = USER.Tien;
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
                            biendongds.LoiNhan = "Nạp ATM";
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
            return PartialView("_Recordnap");
        }
        public ActionResult kiemtra(RootobjectACB data)
        {

            return Json(new { Message = "Thành công", JsonRequestBehavior.AllowGet });
        }
    }
    public class RootobjectACB
    {
        public string time { get; set; }
        public string codeStatus { get; set; }
        public string messageStatus { get; set; }
        public string description { get; set; }
        public string took { get; set; }
        public DatumACB[] data { get; set; }
    }

    public class DatumACB
    {
        public string amount { get; set; }
        public string accountName { get; set; }
        public string receiverName { get; set; }
        public string transactionNumber { get; set; }
        public string description { get; set; }
        public string bankName { get; set; }
        public string isOnline { get; set; }
        public string postingDate { get; set; }
        public string accountOwner { get; set; }
        public string type { get; set; }
        public string receiverAccountNumber { get; set; }
        public string currency { get; set; }
        public string account { get; set; }
        public string activeDatetime { get; set; }
        public string effectiveDate { get; set; }
        public string redisTook { get; set; }
    }
}