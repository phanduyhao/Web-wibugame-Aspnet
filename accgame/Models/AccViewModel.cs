using System;
namespace accgame.Models
{
    public class AccViewModel
    {
        public int IDAcc { get; set; }
        public string TenAcc { get; set; }
        public string Server { get; set; }
        public string MaTaiKhoan { get; set; }
        public string ChiTiet { get; set; }
        public int? Gia { get; set; }
        public int? IDNguoiDung { get; set; }
        public int? IDNguoiMua { get; set; }
        public DateTime? ThoiGianDang { get; set; }
        public DateTime? ThoiGianBan { get; set; }
        public bool? DaBan { get; set; }
        public string TaiKhoan { get; set; }

    }

}

