using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using accgame.Context;
using Newtonsoft.Json;
using accgame.Load;
using accgame.Filters;

namespace accgame.Controllers
{
    [CheckSessionID]

    [NotifyUserFilter]

    public class NapTienController : Controller
    {

        // GET: NapTien
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ATM()
        {
            using (var db = new accgameEntities())
            {
                ViewBag.napatm = "active";
                int idnd = Convert.ToInt32(Session["IDNguoiDung"]);
                Session["url"] = "/NapTien/ATM";
                if (idnd == 0)
                {
                    return Redirect("/dangnhap");
                }
                /*           var nd = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).FirstOrDefault();
                           if (nd == null || nd.SessionID == null)
                           {
                               return Redirect("/dangnhap");
                           }*/
                var nguoidung = db.NguoiDungs.Where(m => m.IDNguoiDung == idnd).FirstOrDefault();
                if (nguoidung != null && nguoidung.Block == true)
                {
                    return RedirectToAction("Blocked", "Home");

                }
                //phan quyen
                var caidat = db.CaiDats;
                var stk = caidat.Where(m => m.MaCaiDat == "stk").FirstOrDefault();
                var chuNh = caidat.Where(m => m.MaCaiDat == "chu_tk_ngan_hang").FirstOrDefault();
                var nganhang = caidat.Where(m => m.MaCaiDat == "ngan_hang").FirstOrDefault();
                ViewBag.stk = stk?.NoiDung ?? "";
                ViewBag.chuNh = chuNh?.NoiDung ?? "";
                var bin = Load.NganHang.GetMaBin(nganhang?.NoiDung ?? "");
                ViewBag.tenNganHang = Load.NganHang.GetTenNganHang(nganhang?.NoiDung ?? "");
                ViewBag.img = "https://img.vietqr.io/image/" + bin + "-" + (stk?.NoiDung ?? "") + "-qr_only.png?amount=0&addInfo=ABC%20" + nguoidung.IDNguoiDung;
                ViewBag.noidung = "ABC " + nguoidung.IDNguoiDung;
                return View();
            }

        }

        public ActionResult loadATM()
        {

            using (var db = new accgameEntities())
            {
                string content = "";
                string url = "https://api.sieuthicode.net/historyapimbbank/dfwVXAMjkuay-HENBon-IXbc-ORzq-abzK";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responeStream = response.GetResponseStream())
                using (StreamReader streamReader = new StreamReader(responeStream))
                {
                    content = streamReader.ReadToEnd();
                }
                Rootobject dataJson = JsonConvert.DeserializeObject<Rootobject>(content);

                int kt = Convert.ToInt32(0);
                int soxu = 0;
                int sotien = 0;
                ViewBag.kq = "";
                var naptien = db.NapTiens.ToList();
                for (int i = 0; i < dataJson.TranList.Length; i++)
                {
                    try
                    {
                        string searchMagdTranid = dataJson.TranList[i].tranId;
                        var searchNap = db.NapTiens.Where(m => m.Magd == searchMagdTranid).FirstOrDefault();
                        if (searchNap != null)
                        {
                            continue;
                        }
                        string[] usname = dataJson.TranList[i].description.ToLower().Replace("nap", "#").Split(new char[] { '#' });
                        if (usname.Length > 1)
                        {
                            string usn = Convert.ToString(usname[1]);
                            string[] layiduser = usn.ToLower().Replace("gz", "#").Split(new char[] { '#' });
                            int iduser = Convert.ToInt32(layiduser[0]);
                            var idus = db.NguoiDungs.Where(m => m.IDNguoiDung == iduser).FirstOrDefault();
                            if (idus != null)
                            {
                                NapTien nap = new NapTien();
                                sotien = int.Parse(dataJson.TranList[i].creditAmount);
                                nap.IDNguoiDung = idus.IDNguoiDung;
                                nap.Magd = dataJson.TranList[i].tranId;
                                nap.SoTien = Convert.ToInt32(sotien);
                                nap.thoigian = DateTime.Now;
                                nap.Noidung = "Nạp ATM";
                                db.NapTiens.Add(nap);
                                
                                kt = 0;
                                var USER = db.NguoiDungs.Find(idus.IDNguoiDung);
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
                                biendongds.TienTruoc = soxu;
                                biendongds.TienSau = soxu + sotien;
                                db.BienDongSoDus.Add(biendongds);

                                // Tạo thông báo HTML
                                string sotienDinhDang = sotien.ToString("N0").Replace(",", ".") + "đ";
                              /*  string soduhientai = nap.SauNap.HasValue
                                    ? nap.SauNap.Value.ToString("N0").Replace(",", ".") + "đ"
                                    : "0đ";*/
                                int tienhientai = soxu + sotien;
                                string soduhientai = tienhientai.ToString("N0").Replace(",", ".") + "đ";
                                string noiDungThongBao = $@"

                                    <p>Bạn đã nạp thành công số tiền: <strong>{sotienDinhDang}</strong>.</p>
                                    <p>Số dư hiện tại: <strong>{soduhientai}</strong>.</p>";

                                // Tạo một NotifyUser mới
                                var notify = new NotifyUser
                                {
                                    IdNguoidung = iduser,
                                    Contents = noiDungThongBao,
                                    IsRead = false,
                                    IsAdminPost = false,
                                    Thoigian = DateTime.Now // Nếu bạn có cột lưu thời gian
                                };

                                // Thêm vào bảng NotifyUser
                                db.NotifyUsers.Add(notify);
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
                ViewBag.xemxu = -1;
                if (idus_nd != 0)
                {
                    ViewBag.xemxu = db.NguoiDungs.Where(m => m.IDNguoiDung == idus_nd).FirstOrDefault().Tien;
                }
                return PartialView("_Recordnap");
            }
            
        }
    }

    public class Rootobject
    {
        public string status { get; set; }
        public string message { get; set; }
        public Datum[] TranList { get; set; }
    }

    public class Datum
    {
        public string refNo { get; set; }
        public string tranId { get; set; }
        public string postingDate { get; set; }
        public string transactionDate { get; set; }
        public string accountNo { get; set; }
        public string creditAmount { get; set; }
        public string debitAmount { get; set; }
        public string currency { get; set; }
        public string description { get; set; }
        public string availableBalance { get; set; }
        public string beneficiaryAccount { get; set; }
    }
}