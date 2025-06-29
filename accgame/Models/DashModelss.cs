using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace accgame.Models
{
    public class DashModelss
    {
        public int thanhVien { get; set; }
        /*        public int tienThanhVien { get; set; }
                public int tienCTV { get; set; }
                public int tongTienDaNap { get; set; }*/
        public decimal? tienThanhVien { get; set; }
        public decimal? tienCTV { get; set; }
        public decimal? tongTienDaNap { get; set; }
        public int bankNap { get; set; }
        public int theNap { get; set; }
        public int taiKhoanGame { get; set; }
        public int taiKhoanGameDaBan { get; set; }
        public int tongDonNap { get; set; }
        public int tongDonNapHoanThanh { get; set; }
        public int tongDonCayThue { get; set; }
        public int tongDonCTHoanThanh { get; set; }

        public int doanhThuHomNay  { get; set; }
        public int tienNapHomNay { get; set; }
        public int napBankHomNay { get; set; }
        public int napTheHomNay { get; set; }
        public int taiKhoanDaDang { get; set; }
        public int donBanNickHomNay { get; set; }

        public int doanhThuThang { get; set; }
        public int tienNapThang { get; set; }
        public int napBankThang { get; set; }
        public int napTheThang { get; set; }
        public int taiKhoanDaDangThang { get; set; }
        public int donBanNickThang { get; set; }

        public int tongDoanhThu { get; set; }
    }
}