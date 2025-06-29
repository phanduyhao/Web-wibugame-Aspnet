using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace accgame.Load
{

    public class NganHang
    {
        private static Dictionary<string, string> maBin = new Dictionary<string, string>
        {
            {"vietcombank", "970436"},
            {"acb", "970416"},
            {"mb_bank", "970422"},
            {"vietinbank", "970415"},
        };

        private static Dictionary<string, string> tenNganHang = new Dictionary<string, string>
        {
            {"vietcombank", "Vietcombank - Ngân hàng Ngoại thương"},
            {"acb", "ACB - Ngân hàng Á Châu"},
            {"mb_bank", "MB Bank - Ngân hàng Quân đội"},
            {"vietinbank", "Vietinbank - Ngân hàng Thương mại"},
        };

        public static string GetMaBin(string str)
        {
            if (maBin.ContainsKey(str.ToLower()))
            {
                return maBin[str];
            }
            else
            {
                return maBin["vietcombank"];
            }
        }

        public static string GetTenNganHang(string str)
        {
            if (tenNganHang.ContainsKey(str))
            {
                return tenNganHang[str];
            }
            else
            {
                return tenNganHang["vietcombank"];
            }
        }
    }
}