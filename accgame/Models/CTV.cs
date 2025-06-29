using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace accgame.Models
{
    public class CTV
    {
        public List<ThongKe> thongKes { get; set; }
    }
    public class ThongKe
    {
        public int IDNguoiDung { get; set; }
        public string TenNguoiDung { get; set; }
        public string Email { get; set; }

        public long? DoanhThuHomNay { get; set; }
        public long? DoanhThuThang { get; set; }
        public long? TongDoanhThu { get; set; }
        public int? AccBanHomNay { get; set; }
        public int? AccBanThangNay { get; set; }
        public int? TongAccBan { get; set; }
        public int? SoAccDaDang { get; set; }
        public int? CayThueHoanThanh { get; set; }

        public double? TyLeHoaHong { get; set; }

    }
}